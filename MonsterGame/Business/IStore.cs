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
        string ConnectClient(Client client);
        Client GetLoggedClient(string userToken);
        List<Client> GetLoggedClients();
        bool IsClientConnected(Client client);
        void DisconnectClient(string token);
        bool UpdateClient(Client existingClient, Client newClient);
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

        List<RankingDTO> GetRanking();
        void SetRanking(List<RankingDTO> ranking);

        List<StatisticDTO> GetStatistics();
        void SetStatistics(List<StatisticDTO> statistics);

        Player GetLoggedPlayer(string username);

        void AddLogEntry(LogEntry entryAttributes);
    
        List<LogEntry> GetLogEntries();
        LogEntry GetLastLogEntry();

    }
}
