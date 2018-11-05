using Business;
using Entities;
using System.Collections.Generic;
using System.Linq;

namespace WebServices
{
    public class Converter
    {
        public static Client ClientDTOToClient(ClientDTO credentials)
        {
            return new Client(credentials.Username, credentials.Password);
        }

        public static ClientDTO ClientToCredentials(Client client)
        {
            return new ClientDTO()
            {
                Username = client.Username,
                Password = client.Password
            };
        }
    }
}
