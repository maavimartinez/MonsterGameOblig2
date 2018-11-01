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

        List<RankingItem> GetRanking();
        void SetRanking(List<RankingItem> ranking);

        List<StatisticItem> GetStatistics();
        void SetStatistics(List<StatisticItem> statistics);

        Player GetLoggedPlayer(string username);

      //  void AddLogEntry(LogEntry entryAttributes);

    }
}
