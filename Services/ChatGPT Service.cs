using System.Net.Http.Json;

using Newtonsoft.Json;

namespace ChatGPT.Services;
internal static class ChatGPTApi
{
    internal const string GPT3Model = "gpt-3.5-turbo";
    internal const string GPT3ModelLarge = "gpt-3.5-turbo-16k";
    internal const string GPT4Model = "gpt-4";
    internal const string GPT4ModelLarge = "gpt-4-32k";

    internal static async Task<string?> AskQuestionAsync(string question, string apiKey, string model = GPT3Model)
    {
        using var http = CreateHttpClient(apiKey);
        var requestBody = CreateRequestBody(question, model);

        var httpResponse = await PostQuestionAsync(http, requestBody);
        if (httpResponse.Contains("context_length_exceeded"))
        {
            requestBody = UpdateRequestBodyWithLargeModel(requestBody, model);
            httpResponse = await PostQuestionAsync(http, requestBody);
        }

        return ExtractContentFromResponse(httpResponse);
    }

    private static HttpClient CreateHttpClient(string apiKey)
    {
        var http = new HttpClient { Timeout = TimeSpan.FromSeconds(666) };
        http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        return http;
    }

    private static object CreateRequestBody(string question, string model)
    {
        return new
        {
            model,
            messages = new[] { new { role = "user", content = question } },
            temperature = 0
        };
    }

    private static async Task<string> PostQuestionAsync(HttpClient http, object requestBody)
    {
        var httpResult = await http.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
        return await httpResult.Content.ReadAsStringAsync();
    }

    private static object UpdateRequestBodyWithLargeModel(object requestBody, string model)
    {
        return new
        {
            model = model == GPT3Model ? GPT3ModelLarge : GPT4ModelLarge,
            messages = requestBody.GetType().GetProperty("messages")?.GetValue(requestBody),
            temperature = requestBody.GetType().GetProperty("temperature")?.GetValue(requestBody)
        };
    }

    private static string? ExtractContentFromResponse(string httpResponse)
    {
        var jsonResponse = JsonConvert.DeserializeObject<Models.ChatGPTDTO>(httpResponse);
        return jsonResponse?.choices[0].message.content;
    }
}

