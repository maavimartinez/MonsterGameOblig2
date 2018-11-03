using Business;
using Entities;
using System.Collections.Generic;

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

        public static List<RankingItem> SerializeRanking(List<Ranking> ranking)
        {
            List<RankingItem> ret = new List<RankingItem>();
            foreach (Ranking r in ranking)
            {
                RankingItem ri = new RankingItem();
                ri.GameDate = r.GameDate.ToString();
                ri.Role = r.Role;
                ri.Score = r.Score;
                ri.Username = r.Username;
                ret.Add(ri);
            }
            return ret;
        }
    }
}
