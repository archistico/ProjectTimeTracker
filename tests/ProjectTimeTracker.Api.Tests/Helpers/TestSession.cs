using Microsoft.AspNetCore.Http;

namespace ProjectTimeTracker.Api.Tests.Helpers;

public class TestSession : ISession
{
    private readonly Dictionary<string, byte[]> _sessionStorage = new();

    public IEnumerable<string> Keys => _sessionStorage.Keys;

    public string Id { get; } = Guid.NewGuid().ToString();

    public bool IsAvailable => true;

    public void Clear()
    {
        _sessionStorage.Clear();
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        _sessionStorage.Remove(key);
    }

    public void Set(string key, byte[] value)
    {
        _sessionStorage[key] = value;
    }

    public bool TryGetValue(string key, out byte[] value)
    {
        if (_sessionStorage.TryGetValue(key, out var storedValue))
        {
            value = storedValue;
            return true;
        }

        value = Array.Empty<byte>();
        return false;
    }
}