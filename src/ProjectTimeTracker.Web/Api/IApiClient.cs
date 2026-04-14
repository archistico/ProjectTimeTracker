namespace ProjectTimeTracker.Web.Api;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default);
    Task<List<T>> GetListAsync<T>(string url, CancellationToken cancellationToken = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken = default);
    Task<bool> PutAsync<TRequest>(string url, TRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string url, CancellationToken cancellationToken = default);
}