using Entities;
using System;
using System.Net.Sockets;


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
            try
            {
                if (!Store.ClientExists(client))
                    Store.AddClient(client);
                else
                    return false;

                return true;
            }
            catch (SocketException)
            {
                throw new SocketException();
            }

        }

        public bool UpdateClient(Client existingClient, Client newClient)
        {
            try
            {
                if (!Store.ClientExists(existingClient))
                    return false;

                return Store.UpdateClient(existingClient, newClient);
            }
            catch (SocketException)
            {
                throw new SocketException();
            }

        }

        public bool DeleteClient(Client client)
        {
            try
            {
                if (!Store.ClientExists(client) || Store.IsClientConnected(client))
                    return false;

                Store.DeleteClient(client);

                return true;
            }
            catch (SocketException)
            {
                throw new SocketException();
            }
        }


    }
}
