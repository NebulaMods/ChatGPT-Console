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
        using var http = new HttpClient();
        http.Timeout = TimeSpan.FromSeconds(666);
        http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        var requestBody = new
        {
            model,
            messages = new[]
            {
                new 
                {
                    role = "user",
                    content = question
                }
            },
            temperature = 0
        };

        var httpResult = await http.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
        var httpResponse = await httpResult.Content.ReadAsStringAsync();
        if (httpResponse.Contains("context_length_exceeded"))
        {
            requestBody = new
            {
                model = model == GPT3Model ? GPT3ModelLarge : GPT4ModelLarge,
                requestBody.messages,
                requestBody.temperature
            };
            httpResult = await http.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
            httpResponse = await httpResult.Content.ReadAsStringAsync();
        }
        var jsonResponse = JsonConvert.DeserializeObject<Models.ChatGPTDTO>(httpResponse);
        return jsonResponse?.choices[0].message.content;
    }
}
