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

        public WcfCode CreateClient(ClientDTO credentials)
        {
            try
            {
                Client client = Converter.ClientDTOToClient(credentials);

                bool result = gameLogic.CreateClient(client);

                return result ? WcfCode.True : WcfCode.False;
            }
            catch (SocketException)
            {
                return WcfCode.Null;
            }
        }

        public WcfCode DeleteClient(ClientDTO credentials)
        {
            try
            {
                Client client = Converter.ClientDTOToClient(credentials);

                bool result = gameLogic.DeleteClient(client);

                return result ? WcfCode.True : WcfCode.False;
            }
            catch (SocketException)
            {
                return WcfCode.Null;
            }

        }

        public List<ClientDTO> GetClients()
        {
            try
            {
                List<ClientDTO> credentials = new List<ClientDTO>();

                gameLogic.GetClients().ForEach(c => credentials.Add(Converter.ClientToCredentials(c)));

                return credentials;
            }
            catch (SocketException) {
                return null;
            } 
        }

        public LogEntry GetLastLog()
        {
            try
            {
                LogEntry lastGameLog = gameLogic.GetLastLogEntry();

                return lastGameLog;
            }
            catch (SocketException)
            {
                return null;
            }
        }

        public List<RankingDTO> GetRanking()
        {
            try
            {
                List<RankingDTO> ranking = new List<RankingDTO>();

                ranking = gameLogic.Ranking();

                return ranking;
            }
            catch (SocketException)
            {
                return null;
            }
        }

        public List<StatisticDTO> GetStatistics()
        {
            try
            {
                List<StatisticDTO> statistics = new List<StatisticDTO>();

                statistics = gameLogic.Statistics();

                return statistics;
            }
            catch (SocketException)
            {
                return null;
            }
        }

        public WcfCode UpdateClient(ClientDTO oldCredentials, ClientDTO newCredentials)
        {
            try
            {
                Client old = Converter.ClientDTOToClient(oldCredentials);
                Client newC = Converter.ClientDTOToClient(newCredentials);

                bool result = gameLogic.UpdateClient(old, newC);

                return result ? WcfCode.True : WcfCode.False;
            }
            catch (SocketException)
            {
                return WcfCode.Null;
            }

        }
    }
}