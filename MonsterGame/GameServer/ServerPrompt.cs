using System;
using System.Collections.Generic;
using Business;
using UI;

//FALTA PONER CONNECTED PLAYERS O ALGO ASI EN EL MENU, LO DEL OBLIG ANTERIOR
namespace GameServer
{
    public class ServerPrompt
    {
        private GameLogic gameLogic;
        //private LogRouter logRouter;

        public ServerPrompt(GameLogic gameLogic)
        {
            this.gameLogic = gameLogic;
            //logRouter = new LogRouter();
        }

        public void PromptUserForAction()
        {
            while (true)
            {
                int option = Menus.ServerPromptMenu();

                MapOptionToAction(option);

                if (option == 3)
                    break;
            }
        }

        private void MapOptionToAction(int option)
        {
            switch (option)
            {
                case 1:
                    ListAllClients();
                   // logRouter.LogListOfAllClients();
                    break;
                case 2:
                    ListConnectedClients();
                    //logRouter.LogListOfConnectedUsers();
                    break;
            }
        }

        private void ListAllClients()
        {
            if (gameLogic.GetClients().Count == 0)
            {
                Console.WriteLine(" -> There are no clients registered");
            }else
            {
                gameLogic.GetClients().ForEach(client =>
                {
                    Console.WriteLine(
                        $"- {client.Username}");
                });
            }
        }

        private void ListConnectedClients()
        {
            if (gameLogic.GetClients().Count == 0)
            {
                Console.WriteLine(" -> There are no connected clients");
            }
            else
            {
                gameLogic.GetLoggedClients().ForEach(client =>
                {
                if (client.ConnectedSince == null) return;
                TimeSpan timeConnected = DateTime.Now.Subtract((DateTime)client.ConnectedSince);
                string timeConnectedFormatted = timeConnected.ToString(@"hh\:mm\:ss");
                Console.WriteLine(
                    $"- {client.Username} \tConnected: {client.ConnectionsCount} times \tConnected for: {timeConnectedFormatted}");
                });
            }
        }

    }
}