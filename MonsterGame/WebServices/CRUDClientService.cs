using System.Collections.Generic;
using Business;
using Entities;

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

        public List<RankingCredentials> GetRanking()
        {
            List<RankingCredentials> ranking = new List<RankingCredentials>();

            ranking = gameLogic.Ranking();

            return ranking;
        }

        public List<StatisticCredentials> GetStatistics()
        {
            List<StatisticCredentials> statistics = new List<StatisticCredentials>();

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