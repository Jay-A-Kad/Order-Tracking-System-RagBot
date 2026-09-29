using System.ComponentModel;
using Azure.Search.Documents;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Azure.Search.Documents.Models;
namespace OrderTracking.AI;

public class KnowledgeBasePlugin
{

    private readonly SearchClient _searchClient;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embed;

    public KnowledgeBasePlugin(SearchClient searchClient, IEmbeddingGenerator<string, Embedding<float>> embed)
    {
        _embed = embed;
        _searchClient = searchClient;
    }

    [KernelFunction("search_policies")]
    [Description("Searches shipping, returns, refund, and warranty policy documents to answer a customer's policy question")]
    public async Task<string> SearchPoliciesAsync([Description("The customer's question or topic to search policy documents for")] string query)
    {
        //embed query 
        var queryEmbedResult = await _embed.GenerateAsync(new[] {query});
        var queryVector = queryEmbedResult[0].Vector.ToArray();

        //vector search request
        var searchOptions = new SearchOptions();
        searchOptions.VectorSearch = new VectorSearchOptions
            {
                Queries = {new VectorizedQuery(queryVector){KNearestNeighborsCount = 3, Fields = {"ContentVector"}}}
            };



        //run and build result string
        var response = await _searchClient.SearchAsync<PolicySearchResult>(null, searchOptions);
        var summary = string.Empty;
        await foreach(var result in response.Value.GetResultsAsync())
        {
             summary += $"[{result.Document.SourceDocument} - {result.Document.Heading}]:  {result.Document.Content}\n\n";
        }

        return summary.Length == 0 ? "No relevant policy information found." : summary;
    }



    
}


//reading response
record PolicySearchResult(string SourceDocument, string Heading, string Content);
