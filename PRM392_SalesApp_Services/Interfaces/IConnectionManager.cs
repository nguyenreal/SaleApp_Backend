namespace PRM392.SalesApp.Services.Interfaces
{
    public interface IConnectionManager
    {
        void AddConnection(int userId, string connectionId);
        void RemoveConnection(string connectionId);
        HashSet<string> GetConnections(int userId);
    }
}