using System.Text;
using Microsoft.AspNetCore.Http;

namespace ProjectTimeTracker.Api.Tests.Helpers;

internal sealed class TestSession : ISession
{
    private readonly Dictionary<string, byte[]> _store = new();

    public IEnumerable<string> Keys => _store.Keys;
    public string Id => Guid.NewGuid().ToString();
    public bool IsAvailable => true;

    public void Clear() => _store.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => _store.Remove(key);

    public void Set(string key, byte[] value) => _store[key] = value;

    public bool TryGetValue(string key, out byte[]? value) => _store.TryGetValue(key, out value);

    public string? GetString(string key)
    {
        return _store.TryGetValue(key, out var value)
            ? Encoding.UTF8.GetString(value)
            : null;
    }
}
