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
            gameLogic = CoreController.GameLogicInstance();
        }

        public bool CreateClient(ClientCredentials clientCredentials)
        {
            Client client = Converter.ClientCredentialsToClient(clientCredentials);

            bool result = gameLogic.CreateClient(client);

            return result;
        }

        public bool DeleteClient(ClientCredentials clientDto)
        {
            Client client = Converter.ClientCredentialsToClient(clientDto);

            bool result = gameLogic.DeleteClient(client);

            return result;
        }

        public List<ClientCredentials> GetClients()
        {
            List<ClientCredentials> clientDtos = new List<ClientCredentials>();

            gameLogic.GetClients().ForEach(c => clientDtos.Add(Converter.ClientToClientCredentials(c)));

            return clientDtos;
        }

        public bool UpdateClient(ClientCredentials existingClientCredentials, ClientCredentials newClientCredentials)
        {
            Client existingClient = Converter.ClientCredentialsToClient(existingClientCredentials);
            Client newClient = Converter.ClientCredentialsToClient(newClientCredentials);

            bool result = gameLogic.UpdateClient(existingClient, newClient);

            return result;
        }
    }
}