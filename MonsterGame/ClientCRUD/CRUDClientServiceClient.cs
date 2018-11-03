using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using Business;
using Entities;

namespace CRUDClient
{
    public class CRUDClientServiceClient
    {
        private const string NoClientsMessage = "There are no clients...";

        private WebServices.CRUDClientServiceClient crudServiceClient;

        public CRUDClientServiceClient()
        {
            crudServiceClient = new WebServices.CRUDClientServiceClient();
        }

        public void Menu()
        {

            int option = Menus.CRUDClientMenu();
            MapOptionToAction(option);
        }

        private void MapOptionToAction(int option)
        {
            switch (option)
            {
                case 1:
                    CreateClient();
                    break;
                case 2:
                    UpdateClient();
                    break;
                case 3:
                    DeleteClient();
                    break;
                case 4:
                    PrintLog();
                    break;
                case 5:
                    Ranking();
                    break;
                default:
                    Environment.Exit(0);
                    return;
            }
        }

        private void DeleteClient()
        {
            try
            {
                ClientCredentials clientToDelete = AskExistingClientInfo();

                if (!crudServiceClient.DeleteClient(clientToDelete))
                    Console.WriteLine("Client does not exist or is connected.");
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(NoClientsMessage);
                Console.ReadKey();
            }
        }

        private void UpdateClient()
        {
            try
            {
                ClientCredentials existingClient = AskExistingClientInfo();

                ClientCredentials editedClient = AskNewClientInfo();

                crudServiceClient.UpdateClient(existingClient, editedClient);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(NoClientsMessage);
                Console.ReadKey();
            }
        }

        private ClientCredentials AskExistingClientInfo()
        {
            List<ClientCredentials> existingClients = crudServiceClient.GetClients().ToList();

            if (existingClients.Count == 0)
                throw new IndexOutOfRangeException();

            var existingClientsUsernames = new List<string>();

            existingClients.ForEach(ec => existingClientsUsernames.Add(ec.Username));

            int option = Menus.MapExistingClients(existingClientsUsernames);
            option--;

            string selectedClientUsername = existingClientsUsernames[option];

            return existingClients.Find(ec => ec.Username.Equals(selectedClientUsername));
        }

        private void CreateClient()
        {
            bool created = false;

            while (!created)
            {
                ClientCredentials clientToCreate = AskNewClientInfo();

                created = crudServiceClient.CreateClient(clientToCreate);

                if (!created)
                    Console.WriteLine("\nUsername taken");
            }
        }

        private void PrintLog()
        {
            LogEntry lastLog = crudServiceClient.GetLastLog();
            if (lastLog != null)
            {
                Console.WriteLine(lastLog);
            }
            else
            {
                Console.WriteLine("There hasn't been a game yet.");

            }
            Console.WriteLine("-------------------");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void Ranking()
        {
            List<RankingItem> ranking = crudServiceClient.GetRanking().ToList();
            if (ranking!=null && ranking.Count > 0)
            {
                foreach(RankingItem ri in ranking)
                {
                    Console.WriteLine(ri.ToString());
                }
            }
            else
            {
                Console.WriteLine("No ranking available.");

            }
            Console.WriteLine("-------------------");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private ClientCredentials AskNewClientInfo()
        {
            Console.WriteLine("Insert username:");
            string username = Input.RequestUsernameAndPassword("Insert Username: ");

            Console.WriteLine("Insert Password");
            string password = Input.RequestUsernameAndPassword("Insert Password: ");

            return new ClientCredentials()
            {
                Username = username,
                Password = password
            };
        }
    }
}
