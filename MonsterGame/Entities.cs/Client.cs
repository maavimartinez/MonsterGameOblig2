﻿using System;
using System.Collections.Generic;

namespace Entities
{
    public class Client
    {

        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime? ConnectedSince => Sessions.Find(session => session.Active)?.ConnectedSince;
        public int ConnectionsCount => Sessions.Count;
        private List<Session> Sessions { get; }

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