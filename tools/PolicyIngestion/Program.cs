using Microsoft.Extensions.Configuration;
using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using System.Numerics;
using Microsoft.Win32.SafeHandles;


var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

//get azure credentials
string searchEndpoint = config["AzureSearch:Endpoint"]!;
string searchAdminKey = config["AzureSearch:AdminKey"]!;

string currentDir = Directory.GetCurrentDirectory();
string[] filePath = Directory.GetFiles(Path.Combine(currentDir, "src/AI/PolicyDocs"), "*.md");

//chunking dictionary
var chunkDictionary = new Dictionary<string,string>();




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







//chunking response record
public record PolicyChunk(string SourceDocument, string Heading, string Content);