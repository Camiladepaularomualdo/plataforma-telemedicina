using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ClinifyMobile.Helpers;

namespace ClinifyMobile.Services;

/// <summary>
/// Wrapper sobre HttpClient com tratamento global de erros e desserialização.
/// Todos os Services dependem desta classe.
/// </summary>
public class ApiService
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(AppSettings.ApiBaseUrl + "/");
        _httpClient.Timeout = TimeSpan.FromSeconds(AppSettings.HttpTimeoutSeconds);
    }

    // ─── GET ─────────────────────────────────────────────────────────────────

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            await EnsureSuccessAsync(response);
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw new ApiException($"Erro ao buscar dados: {ex.Message}", ex);
        }
    }

    // ─── POST ────────────────────────────────────────────────────────────────

    public async Task<T?> PostAsync<T>(string endpoint, object payload)
    {
        try
        {
            var json    = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            await EnsureSuccessAsync(response);
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw new ApiException($"Erro ao enviar dados: {ex.Message}", ex);
        }
    }

    public async Task PostAsync(string endpoint, object? payload = null)
    {
        try
        {
            HttpContent? content = null;
            if (payload is not null)
            {
                var json = JsonSerializer.Serialize(payload, _jsonOptions);
                content  = new StringContent(json, Encoding.UTF8, "application/json");
            }
            var response = await _httpClient.PostAsync(endpoint, content);
            await EnsureSuccessAsync(response);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw new ApiException($"Erro ao enviar dados: {ex.Message}", ex);
        }
    }

    // ─── PATCH ───────────────────────────────────────────────────────────────

    public async Task PatchAsync(string endpoint)
    {
        try
        {
            var request  = new HttpRequestMessage(HttpMethod.Patch, endpoint);
            var response = await _httpClient.SendAsync(request);
            await EnsureSuccessAsync(response);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw new ApiException($"Erro ao atualizar dados: {ex.Message}", ex);
        }
    }

    // ─── Error Handling ──────────────────────────────────────────────────────

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        var body = await response.Content.ReadAsStringAsync();
        var message = response.StatusCode switch
        {
            System.Net.HttpStatusCode.Unauthorized => "Sessão expirada. Faça login novamente.",
            System.Net.HttpStatusCode.NotFound     => "Recurso não encontrado.",
            System.Net.HttpStatusCode.BadRequest   => string.IsNullOrEmpty(body) ? "Dados inválidos." : body.Trim('"'),
            _ => $"Erro do servidor ({(int)response.StatusCode}): {body}"
        };

        throw new ApiException(message);
    }
}

/// <summary>Exception tipada para erros de API.</summary>
public class ApiException : Exception
{
    public ApiException(string message, Exception? inner = null)
        : base(message, inner) { }
}
