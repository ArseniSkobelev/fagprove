namespace IdeaManagement.API.Domain.SignalR;

/// <summary>
/// In-memory storage for mapping T to a connection id/string
/// </summary>
/// <typeparam name="T"></typeparam>
public class HubConnectionMapping<T> where T : notnull
{
    private readonly Dictionary<T, HashSet<string>> _connections = new();

    public void Add(T key, string connectionId)
    {
        lock (_connections)
        {
            if (!_connections.TryGetValue(key, out var connections))
            {
                connections = new HashSet<string>();
                _connections.Add(key, connections);
            }

            lock (connections)
                connections.Add(connectionId);
        }
    }

    public IEnumerable<string> GetConnections(T key)
    {
        if (_connections.TryGetValue(key, out var connections))
            return connections;

        return Enumerable.Empty<string>();
    }
}