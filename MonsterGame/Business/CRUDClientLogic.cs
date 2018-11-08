using Entities;

namespace Business
{
    public class CRUDClientLogic
    {
        private IStore Store { get; set; }

        public CRUDClientLogic(IStore store)
        {
            this.Store = store;
        }

        public bool CreateClient(Client client)
        {
            if (!Store.ClientExists(client))
                Store.AddClient(client);
            else
                return false;

            return true;
        }

        public bool UpdateClient(Client existingClient, Client newClient)
        {
            if (!Store.ClientExists(existingClient))
                return false;

            return Store.UpdateClient(existingClient, newClient);

        }

        public bool DeleteClient(Client client)
        {
            if (!Store.ClientExists(client) || Store.IsClientConnected(client))
                return false;

            Store.DeleteClient(client);

            return true;
        }


    }
}
