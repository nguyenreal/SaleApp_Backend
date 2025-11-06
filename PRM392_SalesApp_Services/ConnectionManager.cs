using PRM392.SalesApp.Services.Interfaces;
using System.Collections.Concurrent;

namespace PRM392.SalesApp.Services
{
    // Dùng ConcurrentDictionary để đảm bảo an toàn khi truy cập từ nhiều thread
    public class ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<int, HashSet<string>> _connections =
            new ConcurrentDictionary<int, HashSet<string>>();

        public void AddConnection(int userId, string connectionId)
        {
            _connections.AddOrUpdate(userId,
                // Add new
                (key) => new HashSet<string> { connectionId },
                // Update existing
                (key, existingSet) =>
                {
                    lock (existingSet)
                    {
                        existingSet.Add(connectionId);
                    }
                    return existingSet;
                });
        }

        public HashSet<string> GetConnections(int userId)
        {
            _connections.TryGetValue(userId, out var connections);
            return connections ?? new HashSet<string>();
        }

        public void RemoveConnection(string connectionId)
        {
            foreach (var (userId, connections) in _connections)
            {
                lock (connections)
                {
                    if (connections.Contains(connectionId))
                    {
                        connections.Remove(connectionId);
                        if (connections.Count == 0)
                        {
                            _connections.TryRemove(userId, out _);
                        }
                        break;
                    }
                }
            }
        }
    }
}