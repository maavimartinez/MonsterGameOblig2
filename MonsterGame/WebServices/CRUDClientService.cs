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

        public bool CreateClient(ClientCredentials credentials)
        {
            Client client = Converter.ClientCredentialsToClient(credentials);

            bool result = gameLogic.CreateClient(client);

            return result;
        }

        public bool DeleteClient(ClientCredentials credentials)
        {
            Client client = Converter.ClientCredentialsToClient(credentials);

            bool result = gameLogic.DeleteClient(client);

            return result;
        }

        public List<ClientCredentials> GetClients()
        {
            List<ClientCredentials> credentials = new List<ClientCredentials>();

            gameLogic.GetClients().ForEach(c => credentials.Add(Converter.ClientToCredentials(c)));

            return credentials;
        }

        public LogEntry GetLastLog()
        {
            LogEntry lastGameLog = gameLogic.GetLastLogEntry();

            return lastGameLog;
        }

        public bool UpdateClient(ClientCredentials oldCredentials, ClientCredentials newCredentials)
        {
            Client old = Converter.ClientCredentialsToClient(oldCredentials);
            Client newC = Converter.ClientCredentialsToClient(newCredentials);

            bool result = gameLogic.UpdateClient(old, newC);

            return result;
        }
    }
}