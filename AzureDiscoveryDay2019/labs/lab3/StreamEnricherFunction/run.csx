using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using pelazem.azure.cognitive.textanalytics;

public static void Run(string myEventHubMessage, ILogger log, out string outputEventHubMessage)
{
    // log.LogInformation($"Azure Function StreamEnricher triggered by new Event Hub message:");
    // log.LogInformation(myEventHubMessage);

    // Parse the entire message which should be valid JSON, otherwise this will fail
	// TODO add try/catch and appropriate notification
    JObject jMessage = JObject.Parse(myEventHubMessage);

    // Get the customer's comments from the message JSON
    string customer_comments = jMessage["customer_comments"].ToString();
    // log.LogInformation($"{nameof(customer_comments)} = {customer_comments}");

    // Get our API root URL and API key from application settings - you should have created these settings
    string textAnalyticsUrlRoot = Environment.GetEnvironmentVariable("TextAnalyticsApiEndpoint");
    string textAnalyticsApiKey = Environment.GetEnvironmentVariable("TextAnalyticsApiKey");
    // log.LogInformation($"{nameof(textAnalyticsUrlRoot)} = {textAnalyticsUrlRoot}");
    // log.LogInformation($"{nameof(textAnalyticsApiKey)} = {textAnalyticsApiKey}");

    // Instantiate text analytics cognitive service client
    TextAnalyticsServiceClient svc = new TextAnalyticsServiceClient(textAnalyticsUrlRoot, textAnalyticsApiKey);

    // Get text analytics results from evaluating customer comments
    // As we use an out variable to forward to the next Event Hub, we cannot use async/await, hence using GetAwaiter().GetResult()
    TextAnalyticsServiceResult taResult = svc.ProcessAsync(customer_comments, "en", true, false, false, false).GetAwaiter().GetResult();

    // If we got a response from text analytics, add important information into the JSON structure we got from the inbound message
    if (taResult.Responses.Count > 0)
    {
        TextAnalyticsResponse response = taResult.Responses[0];

        // Sentiment score between 0 (negative) and 1 (positive)
        jMessage["textanalytics_customer_sentiment_score"] = response.SentimentScore;

        // Key phrases in the customer's comments
        // jMessage["textanalytics_customer_key_phrases"] = new JArray(response.KeyPhrases);

        // What language(s) was/were detected in the customer's comments
        // jMessage["textanalytics_customer_detected_languages"] = (JArray)JToken.FromObject(response.DetectedLanguages);

        // What entities were detected in the customer's comments
        // jMessage["textanalytics_customer_entities"] = (JArray)JToken.FromObject(response.Entities);

        // Any errors that text analytics may have encountered analyzing the customer's comments
        // jMessage["textanalytics_errors"] = (JArray)JToken.FromObject(taResult.Errors);
    }

    // We now write the augmented message to the outbound Event Hub so it can be forwarded downstream
    outputEventHubMessage = jMessage.ToString();

    // Echo what was sent onward to the console
    // log.LogInformation(outputEventHubMessage);
}