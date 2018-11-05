using System;
using System.Collections.Generic;
using Entities;
using Business;
using System.Linq;

namespace Persistence
{
    public class Store : MarshalByRefObject, IStore
    {
        private List<Client> Clients { get; set; }
        public List<Player> AllPlayers { get; set; }
        private List<LogEntry> LogEntries { get; set; }
        public List<Ranking> Ranking { get; set; }
        public List<StatisticItem> Statistics { get; set; }
        public Game ActiveGame { get; set; }
        private List<string> OriginalPlayers { get; set; }

        public Board Board { get; set; }

        private readonly object messagesLocker = new object();
        private readonly object loginLocker = new object();
        private readonly object friendshipLocker = new object();
        private readonly object logLocker = new object();

        //PONER TODOS LOS LOCKERS

        public Store()
        {
            Clients = new List<Client>();
            AllPlayers = new List<Player>();
            Ranking = new List<Ranking>();
            Statistics = new List<StatisticItem>();
            LogEntries = new List<LogEntry>();
            OriginalPlayers = new List<string>();
        }

        public bool ClientExists(Client client)
        {
            return Clients.Contains(client);
        }

        public void AddClient(Client client)
        {
            Clients.Add(client);
        }

        public Client GetClient(string clientUsername)
        {
            return Clients.Find(client => client.Username.Equals(clientUsername));
        }

        public List<Client> GetClients()
        {
            return Clients;
        }

        public Player GetLoggedPlayer(string clientUsername)
        {
            return AllPlayers.FindLast(p => p.Client.Username.Equals(clientUsername));
        }

        public Game GetGame()
        {
            return ActiveGame;
        }

        public void SetGame(Game game)
        {
            this.ActiveGame = game;
            UpdateGamePlayersOnAllPlayers(game);
        }

        public void UpdateGamePlayersOnAllPlayers(Game activeGame)
        {
            foreach(Player pl in activeGame.Players)
            {
                for(int i=0; i < AllPlayers.Count; i++)
                {
                    if(AllPlayers[i].Client.Username == pl.Client.Username)
                    {
                        AllPlayers[i] = pl;
                    }
                }
            }
        }

        public Board GetBoard()
        {
            return Board;
        }

        public void SetBoard(Board board)
        {
            this.Board = board;
        }

        public List<Player> GetAllPlayers()
        {
            return this.AllPlayers;
        }

        public List<string> GetOriginalPlayers()
        {
            return this.OriginalPlayers;
        }

        public void AddOriginalPlayer(string playerUsername)
        {
            OriginalPlayers.Add(playerUsername);
        }

        public void ResetOriginalPlayers()
        {
            OriginalPlayers.Clear();
        }

        public void SetAllPlayers(List<Player> allPlayers)
        {
            this.AllPlayers = allPlayers;
        }

        public void DeleteClient(Client client)
        {
            lock (loginLocker)
            {
                Clients.Remove(client);
            }
        }

        public void ConnectClient(Client client, Session session)
        {
            lock (loginLocker)
            {
                Client storedClient = GetClient(client.Username);
                storedClient.ConnectionsCount++;
                storedClient.AddSession(session);
            }
        }

        public bool IsClientConnected(Client client)
        {
            Client storedClient = Clients.Find(c => c.Equals(client));
            return storedClient != null && storedClient.Sessions.Exists(session => session.Active);
        }

        public void DisconnectClient(string token)
        {
            lock (loginLocker)
            {
                Client storedClient = Clients.Find(c => c.Sessions.Exists(s => s.Id.Equals(token)));
                storedClient.Sessions = new List<Session>();
            }
        }

        public void UpdateClient(Client existingClient, Client newClient)
        {
            lock (loginLocker)
            {
                Client storedClient = GetClient(existingClient.Username);

                storedClient.Username = newClient.Username;
                storedClient.Password = newClient.Password;
            }
        }

        public List<Ranking> GetRanking()
        {
            return this.Ranking.OrderByDescending(x=>x.Score).Take(10).ToList();
        }

        public void SetRanking(List<Ranking> newRanking)
        {
            this.Ranking = newRanking;
        }

        public List<StatisticItem> GetStatistics()
        {
            return this.Statistics;
        }

        public void SetStatistics(List<StatisticItem> statistics)
        {
            this.Statistics = statistics;
        }

        public void AddLogEntry(LogEntry entry)
        {
            lock (logLocker)
            {
                LogEntries.Add(entry);
            }
        }

        public List<LogEntry> GetLogEntries()
        {
            lock (logLocker)
            {
                return LogEntries;
            }
        }

        public LogEntry GetLastLogEntry()
        {
            lock (logLocker)
            {
                if(LogEntries.Count>0)
                return LogEntries.Last();
                return null;
            }
        }
    }
}