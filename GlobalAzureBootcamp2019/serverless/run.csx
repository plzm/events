using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using pelazem.azure.cognitive.textanalytics;

public static void Run(string myQueueItem, ILogger log, out string outputBlob)
{
    log.LogInformation("Message: " + myQueueItem);

    outputBlob = string.Empty;

    string apiUrl = "https://eastus.api.cognitive.microsoft.com/text/analytics/v2.1/";
    string apiKey = "YOUR_KEY_HERE";

    TextAnalyticsServiceClient svc = new TextAnalyticsServiceClient(apiUrl, apiKey);

    TextAnalyticsServiceResult svcResult = svc.ProcessAsync(myQueueItem).GetAwaiter().GetResult();

    if (svcResult.Responses.Count > 0)
    {
        TextAnalyticsResponse response = svcResult.Responses.First();

        outputBlob = ToJson(response);
    }
}

public static string ToJson(object obj)
{
    JsonSerializerSettings settings = new JsonSerializerSettings();
    settings.Formatting = Formatting.Indented;
    settings.NullValueHandling = NullValueHandling.Include;

    return JsonConvert.SerializeObject(obj, settings);
}
