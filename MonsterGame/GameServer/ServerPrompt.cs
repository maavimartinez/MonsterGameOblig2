using System;
using System.Collections.Generic;
using Business;
using UI;
using Entities;

namespace GameServer
{
    public class ServerPrompt
    {
        private GameLogic gameLogic;

        public ServerPrompt(GameLogic gameLogic)
        {
            this.gameLogic = gameLogic;
        }

        public void PromptUserForAction()
        {
            try
            {
                while (true)
                {
                    int option = Menus.ServerPromptMenu();

                    MapOptionToAction(option);

                    if (option == 3)
                        break;
                }
            }
            catch (System.Net.Sockets.SocketException)
            {
                throw new System.Net.Sockets.SocketException();
            }
        }

        private void MapOptionToAction(int option)
        {
            switch (option)
            {
                case 1:
                    ListAllClients();
                    break;
                case 2:
                    ListConnectedClients();
                    break;
            }
        }

        private void ListAllClients()
        {
            try
            {
                if (gameLogic.GetClients().Count == 0)
                {
                    Console.WriteLine(" -> There are no clients registered");
                }
                else
                {
                    gameLogic.GetClients().ForEach(client =>
                    {
                        Console.WriteLine(
                            $"- {client.Username}");
                    });
                }
            }
            catch (System.Net.Sockets.SocketException)
            {
                throw new System.Net.Sockets.SocketException();
            }
        }

        private void ListConnectedClients()
        {
            try
            {
                if (gameLogic.GetClients().Count == 0)
                {
                    Console.WriteLine(" -> There are no connected clients");
                }
                else
                {
                    List<Client> connectedClients = gameLogic.GetLoggedClients();
                    foreach (Client client in connectedClients)
                    {
                        if (client.ConnectedSince == null) return;
                        string timeConnectedFormatted = ((DateTime)client.ConnectedSince).ToString(@"hh\:mm\:ss");
                        Console.WriteLine(
                            $"- {client.Username} \tConnected since: {timeConnectedFormatted}");
                    }
                }
            }
            catch (System.Net.Sockets.SocketException)
            {
                throw new System.Net.Sockets.SocketException();
            }
        }

    }
}