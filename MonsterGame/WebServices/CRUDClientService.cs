using System.Collections.Generic;
using Business;
using Entities;
using System.Net.Sockets;


namespace WebServices
{
    //AGREGAR LOG
    public class CRUDClientService : ICRUDClientService
    {
        private readonly GameLogic gameLogic;

        public CRUDClientService()
        {
            gameLogic = MainController.GameLogicInstance();
        }

        public bool CreateClient(ClientDTO credentials)
        {
                Client client = Converter.ClientDTOToClient(credentials);

                bool result = gameLogic.CreateClient(client);

                return result;
        }

        public bool DeleteClient(ClientDTO credentials)
        {
            Client client = Converter.ClientDTOToClient(credentials);

            bool result = gameLogic.DeleteClient(client);

            return result;
        }

        public List<ClientDTO> GetClients()
        {
            List<ClientDTO> credentials = new List<ClientDTO>();

            gameLogic.GetClients().ForEach(c => credentials.Add(Converter.ClientToCredentials(c)));

            return credentials;
        }

        public LogEntry GetLastLog()
        {
            LogEntry lastGameLog = gameLogic.GetLastLogEntry();

            return lastGameLog;
        }

        public List<RankingDTO> GetRanking()
        {
            List<RankingDTO> ranking = new List<RankingDTO>();

            ranking = gameLogic.Ranking();

            return ranking;
        }

        public List<StatisticDTO> GetStatistics()
        {
            List<StatisticDTO> statistics = new List<StatisticDTO>();

            statistics = gameLogic.Statistics();

            return statistics;
        }

        public bool UpdateClient(ClientDTO oldCredentials, ClientDTO newCredentials)
        {
            Client old = Converter.ClientDTOToClient(oldCredentials);
            Client newC = Converter.ClientDTOToClient(newCredentials);

            bool result = gameLogic.UpdateClient(old, newC);

            return result;
        }
    }
}