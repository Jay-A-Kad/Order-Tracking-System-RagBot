using Microsoft.Extensions.Configuration;
using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.AI;
using Azure.Search.Documents;
using System.Security.Cryptography;


var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

//get azure credentials
string searchEndpoint = config["AzureSearch:Endpoint"]!;
string searchAdminKey = config["AzureSearch:AdminKey"]!;

string currentDir = Directory.GetCurrentDirectory();
string[] filePath = Directory.GetFiles(Path.Combine(currentDir, "src/AI/PolicyDocs"), "*.md");

//chunking dictionary
var chunkDictionary = new Dictionary<string,string>();

//emeddings generator
string embeddingDeploymentName = config["AzureOpenAI:EmbeddingDeployment"]!;
string openAiEndpoint = config["AzureOpenAI:Endpoint"]!;
string openAiApiKey = config["AzureOpenAI:ApiKey"]!;

#pragma warning disable SKEXP0010
var embeddingKernel = Kernel.CreateBuilder()
      .AddAzureOpenAIEmbeddingGenerator(embeddingDeploymentName, openAiEndpoint, openAiApiKey)
      .Build();
#pragma warning restore SKEXP0010

//pull the embed gen services from kernel
var embeddingGenerator = embeddingKernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();





try
{
    foreach(var file in filePath)
    {
        string mdContent = File.ReadAllText(file);
        chunkDictionary.Add(Path.GetFileName(file), mdContent);
        Console.WriteLine($"File found at location: {file}");
    }
}
catch(ArgumentException ex)
{
    Console.WriteLine($"files not found: {filePath}: {ex.Message}");
}

List<PolicyChunk> chunkList = new List<PolicyChunk>();

foreach(var chunk in chunkDictionary)
{
    var pieces = chunk.Value.Split("\n## ");
        

    for(int i=0;i < pieces.Length; i++)
    {
        if(i==0) continue;
        var parts = pieces[i].Split(new[] {"\n"}, 2, StringSplitOptions.None);
        
        if(parts.Length < 2)
        {
            Console.WriteLine("fewer than 2 elements");
            continue;
        }
        chunkList.Add(new PolicyChunk(chunk.Key, parts[0], parts[1].Trim()));
        
    }
    
}

Console.WriteLine($"The chunking: {chunkList.Count}");
foreach( var c in chunkList)
{
    Console.WriteLine($"{c}");
}


//azure search index
var indexClient = new SearchIndexClient(new Uri(searchEndpoint), new AzureKeyCredential(searchAdminKey));

//index search fiels
List<SearchField> searchField = new List<SearchField>
{
    new SimpleField("Id", SearchFieldDataType.String)   { IsKey = true },
    new SimpleField("SourceDocument", SearchFieldDataType.String) { IsFilterable = true },
    new SearchableField("Heading"),
    new SearchableField("Content")
};

//vector config
var vectorSearch = new VectorSearch
{
    Algorithms = {new HnswAlgorithmConfiguration("hnsw-config")},
    Profiles = {new VectorSearchProfile("vector-profile","hnsw-config")}
};

//adding vector to search filed
searchField.Add(new SearchField("ContentVector",
SearchFieldDataType.Collection(SearchFieldDataType.Single))
{
    IsSearchable = true,
    VectorSearchDimensions = 1536,
    VectorSearchProfileName = "vector-profile"
});

//search index  = search field + vector searcgh
var searchIndex = new SearchIndex("policy-docs-index")
{
    Fields = searchField,
    VectorSearch = vectorSearch
};



await indexClient.CreateOrUpdateIndexAsync(searchIndex);
Console.WriteLine("Index created or updated successfully");


//embedding generation using the chunklist

var chunkContent = chunkList.Select(x => x.Content).ToList();


var embeddings = await embeddingGenerator.GenerateAsync(chunkContent);

Console.WriteLine($"count: {embeddings.Count} : Name : {embeddings.GetType().Name}");




//build search document
List<PolicyDocument> policyDoc =  new List<PolicyDocument>();

//made id using sha256
static string GetFileSha256(string f_path)
    {
        using var sha256 = SHA256.Create();
        
        byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(f_path));
        return Convert.ToHexString(hashBytes);
            
        
    }


for(int i=0 ; i < chunkList.Count; i++)
{
        string hash = GetFileSha256(chunkList[i].SourceDocument + chunkList[i].Heading);
        policyDoc.Add(new PolicyDocument(hash, chunkList[i].SourceDocument, chunkList[i].Heading,chunkList[i].Content, embeddings[i].Vector.ToArray()));
}


var searchClient = new SearchClient(new Uri(searchEndpoint), "policy-docs-index", new 
    AzureKeyCredential(searchAdminKey));

var uploadResult = await searchClient.UploadDocumentsAsync(policyDoc);
 foreach (var result in uploadResult.Value.Results)
  {
      if (!result.Succeeded)
      {
          Console.WriteLine($"FAILED: {result.Key} - {result.ErrorMessage}");
      }
  }
var count = await searchClient.GetDocumentCountAsync();
Console.WriteLine($"Index now contains {count.Value} documents.");
Console.WriteLine("Documents uploaded successfully");





//chunking response record
public record PolicyChunk(string SourceDocument, string Heading, string Content);

//document record shape
public record PolicyDocument(string Id, string sourceDocument, string Heading, string Content, float[] ContentVector);


