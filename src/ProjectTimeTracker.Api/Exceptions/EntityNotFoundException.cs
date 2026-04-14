namespace ProjectTimeTracker.Api.Exceptions;

public sealed class EntityNotFoundException : Exception
{
    public string EntityName { get; }
    public object EntityKey { get; }

    public EntityNotFoundException(string entityName, object entityKey)
        : base($"{entityName} con chiave '{entityKey}' non trovato.")
    {
        EntityName = entityName;
        EntityKey = entityKey;
    }
}