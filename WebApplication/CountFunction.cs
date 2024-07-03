using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public static class VisitCountFunction
{
    private static readonly string EndpointUri = Environment.GetEnvironmentVariable("CosmosDBEndpoint");
    private static readonly string PrimaryKey = Environment.GetEnvironmentVariable("CosmosDBKey");
    private static readonly DocumentClient client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);
    private static readonly string DatabaseId = Environment.GetEnvironmentVariable("CosmosDBDatabaseId");
    private static readonly string CollectionId = Environment.GetEnvironmentVariable("CosmosDBCollectionId");

    [FunctionName("IncrementVisitCount")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        var query = client.CreateDocumentQuery<VisitCount>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                          .AsDocumentQuery();
        var results = await query.ExecuteNextAsync<VisitCount>();
        var visitCount = results.FirstOrDefault() ?? new VisitCount { Id = "1", Count = 0 };

        visitCount.Count += 1;
        await client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), visitCount);

        return new OkObjectResult($"{visitCount.Count}");
    }

    public class VisitCount
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
