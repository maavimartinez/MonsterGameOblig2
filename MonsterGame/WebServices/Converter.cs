using Business;
using Entities;

namespace WebServices
{
    public class Converter
    {
        public static Client ClientCredentialsToClient(ClientCredentials clientDto)
        {
            return new Client(clientDto.Username, clientDto.Password);
        }

        public static ClientCredentials ClientToClientCredentials(Client client)
        {
            return new ClientCredentials()
            {
                Username = client.Username,
                Password = client.Password
            };
        }
    }
}
