using System;
using System.Collections.Generic;

namespace Entities
{
    [Serializable]
    public class Client
    {

        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime ConnectedSince {get; set;}// ConnectedSince => Sessions.Find(session => session.Active)?.ConnectedSince;
        public int ConnectionsCount {
            get
            {
                return Sessions.Count;
            }
            set { }
        }
        public List<Session> Sessions { get; set; }

        public Client(string username, string password)
        {
            Username = username;
            Password = password;
            Sessions = new List<Session>();
        }

        public override bool Equals(object obj)
        {
            var toCompare = (Client) obj;
            return toCompare != null && Username.Equals(toCompare.Username);
        }

        public void AddSession(Session session)
        {
            Sessions.Add(session);
        }

        public bool ValidatePassword(string clientPassword)
        {
            return Password.Equals(clientPassword);
        }

    }
}