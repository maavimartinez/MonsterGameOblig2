using Entities;
using System.Collections.Generic;

namespace Business
{
    public interface IStore
    {
        bool ClientExists(Client client);
        void AddClient(Client client);
        Client GetClient(string clientUsername);
        List<Client> GetClients();
        void ConnectClient(Client client, Session session);
        bool IsClientConnected(Client client);
        void DisconnectClient(string token);
        void UpdateClient(Client existingClient, Client newClient);
        void DeleteClient(Client client);

        void SetGame(Game game);
        Game GetGame();

        void SetBoard(Board board);
        Board GetBoard();

        List<Player> GetAllPlayers();
        void SetAllPlayers(List<Player> players);

        List<string> GetOriginalPlayers();
        void AddOriginalPlayer(string playerUsername);
        void ResetOriginalPlayers();

        List<RankingCredentials> GetRanking();
        void SetRanking(List<RankingCredentials> ranking);

        List<StatisticCredentials> GetStatistics();
        void SetStatistics(List<StatisticCredentials> statistics);

        Player GetLoggedPlayer(string username);

        void AddLogEntry(LogEntry entryAttributes);
    
        List<LogEntry> GetLogEntries();
        LogEntry GetLastLogEntry();

    }
}
