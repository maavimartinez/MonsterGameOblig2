using Business;
using Entities;

namespace WebServices
{
    public class Converter
    {
        public static Client ClientCredentialsToClient(ClientCredentials credentials)
        {
            return new Client(credentials.Username, credentials.Password);
        }

        public static ClientCredentials ClientToCredentials(Client client)
        {
            return new ClientCredentials()
            {
                Username = client.Username,
                Password = client.Password
            };
        }
    }
}
