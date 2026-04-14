using System.Net.Http.Json;

namespace ProjectTimeTracker.Web.Api;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
    }

    public async Task<List<T>> GetListAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return new List<T>();
        }

        var result = await response.Content.ReadFromJsonAsync<List<T>>(cancellationToken: cancellationToken);
        return result ?? new List<T>();
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
    }

    public async Task<bool> PutAsync<TRequest>(string url, TRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(url, request, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(url, cancellationToken);
        return response.IsSuccessStatusCode;
    }
}