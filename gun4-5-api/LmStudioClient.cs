using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class LmStudioClient
{
    private readonly HttpClient _http;
    private readonly LmStudioOptions _opt;

    public LmStudioClient(HttpClient http, IOptions<LmStudioOptions> opt)
    {
        _http = http;
        _opt = opt.Value;
    }

    public async Task<ChatResult> SorAsync(
        string system,
        string soru,
        CancellationToken ct = default)
    {
        var istek = new ChatRequest(
            _opt.Model,
            [
                new ChatMessage("system", system),
                new ChatMessage("user", soru)
            ],
            Temperature: 0);

        using var cevap = await _http.PostAsJsonAsync(
            "chat/completions",
            istek,
            ct);

        cevap.EnsureSuccessStatusCode();

        var govde = await cevap.Content.ReadFromJsonAsync<ChatResponse>(ct)
            ?? throw new InvalidOperationException("Modelden boş cevap geldi.");

        return new ChatResult(
            govde.Choices[0].Message.Content,
            govde.Usage.TotalTokens);
    }
}

public record ChatMessage(string Role, string Content);

public record ChatRequest(
    string Model,
    List<ChatMessage> Messages,
    double Temperature);

public record ChatChoice(ChatMessage Message);

public record ChatUsage(
    [property: JsonPropertyName("prompt_tokens")] int PromptTokens,
    [property: JsonPropertyName("completion_tokens")] int CompletionTokens,
    [property: JsonPropertyName("total_tokens")] int TotalTokens);

public record ChatResponse(
    List<ChatChoice> Choices,
    ChatUsage Usage);

public record ChatResult(
    string Content,
    int TotalTokens);