using System;
using System.Collections.Generic;
using Entities;
using Business;

namespace Persistence
{
    public class Store : MarshalByRefObject, IStore
    {
        private List<Client> Clients { get; set; }
        public List<Player> AllPlayers { get; set; }
        public Game ActiveGame { get; set; }
        public Board Board { get; set; }

        public Store()
        {
            Clients = new List<Client>();
            AllPlayers = new List<Player>();
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
            return AllPlayers.Find(p => p.Client.Username.Equals(clientUsername));
        }

        public Game GetGame()
        {
            return ActiveGame;
        }

        public void SetGame(Game game)
        {
            this.ActiveGame = ActiveGame;
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

        public void SetAllPlayers(List<Player> allPlayers)
        {
            this.AllPlayers = allPlayers;
        }

    }
}