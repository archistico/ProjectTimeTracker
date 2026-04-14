using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProjectTimeTracker.Web.Api;

public class ApiClient : IApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(() => _httpClient.GetAsync(url, cancellationToken), cancellationToken);
        return await ReadContentAsync<T>(response, cancellationToken);
    }

    public async Task<List<T>> GetListAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(() => _httpClient.GetAsync(url, cancellationToken), cancellationToken);
        var result = await ReadContentAsync<List<T>>(response, cancellationToken);
        return result ?? new List<T>();
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(
            () => _httpClient.PostAsJsonAsync(url, request, cancellationToken),
            cancellationToken);

        return await ReadContentAsync<TResponse>(response, cancellationToken);
    }

    public async Task<bool> PutAsync<TRequest>(string url, TRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(
            () => _httpClient.PutAsJsonAsync(url, request, cancellationToken),
            cancellationToken);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(
            () => _httpClient.DeleteAsync(url, cancellationToken),
            cancellationToken);

        return response.IsSuccessStatusCode;
    }

    private async Task<HttpResponseMessage> SendAsync(
        Func<Task<HttpResponseMessage>> httpCall,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpCall();

            if (!response.IsSuccessStatusCode)
            {
                throw await CreateApiExceptionAsync(response, cancellationToken);
            }

            return response;
        }
        catch (ApiClientException)
        {
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new ApiClientException("L'API non ha risposto entro il tempo massimo previsto.", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new ApiClientException("Impossibile contattare l'API. Verifica che sia avviata e raggiungibile.", ex);
        }
    }

    private static async Task<T?> ReadContentAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.Content == null)
        {
            return default;
        }

        if (response.Content.Headers.ContentLength == 0)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }

    private static async Task<ApiClientException> CreateApiExceptionAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var statusCode = response.StatusCode;
        var rawContent = response.Content == null
            ? null
            : await response.Content.ReadAsStringAsync(cancellationToken);

        ApiErrorResponse? apiError = null;

        if (!string.IsNullOrWhiteSpace(rawContent))
        {
            try
            {
                apiError = JsonSerializer.Deserialize<ApiErrorResponse>(rawContent, JsonOptions);
            }
            catch
            {
                // Ignora contenuti non compatibili con il formato atteso.
            }
        }

        var message = apiError?.Message;

        if (string.IsNullOrWhiteSpace(message))
        {
            message = statusCode switch
            {
                HttpStatusCode.BadRequest => "La richiesta inviata al server non è valida.",
                HttpStatusCode.NotFound => "La risorsa richiesta non è stata trovata.",
                HttpStatusCode.Conflict => "L'operazione non può essere completata per un conflitto sui dati.",
                HttpStatusCode.InternalServerError => "Il server ha restituito un errore interno.",
                _ => $"L'API ha restituito un errore HTTP {(int)statusCode} ({statusCode})."
            };
        }

        return new ApiClientException(statusCode, message, apiError?.Error);
    }
}