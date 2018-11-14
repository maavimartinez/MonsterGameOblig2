using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using Business;
using Entities;
using System.Net.Sockets;


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
                case 6:
                    Statistics();
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
                ClientDTO clientToDelete = AskExistingClientInfo();

                if (!crudServiceClient.DeleteClient(clientToDelete))
                {
                    Console.WriteLine("Client is connected. Press any key.");
                    Console.ReadKey();
                }    
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
                ClientDTO existingClient = AskExistingClientInfo();

                ClientDTO editedClient = AskNewClientInfo();

                bool result = crudServiceClient.UpdateClient(existingClient, editedClient);

                if (!result)
                {
                    Console.WriteLine("The client is already connected, you can't update his info.  Press any key.");
                    Console.ReadKey();
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(NoClientsMessage);
                Console.ReadKey();
            }
        }

        private ClientDTO AskExistingClientInfo()
        {
            List<ClientDTO> existingClients = crudServiceClient.GetClients().ToList();

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
                    ClientDTO clientToCreate = AskNewClientInfo();

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
            List<RankingDTO> ranking = crudServiceClient.GetRanking().ToList();
            if (ranking!=null && ranking.Count > 0)
            {
                foreach(RankingDTO ri in ranking)
                {
                    int index = ranking.IndexOf(ri);
                    index = index + 1;
                    Console.WriteLine(index+")"+ ri.ToString());
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

        private void Statistics()
        {
            List<StatisticDTO> statistics = crudServiceClient.GetStatistics().ToList();
            if (statistics != null && statistics.Count > 0)
            {
                foreach (StatisticDTO st in statistics)
                {
                    Console.WriteLine(st.ToString());
                }
            }
            else
            {
                Console.WriteLine("No statistics available.");

            }
            Console.WriteLine("-------------------");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private ClientDTO AskNewClientInfo()
        {
            Console.WriteLine("Insert username:");
            string username = Input.RequestUsernameAndPassword("Insert Username: ");

            Console.WriteLine("Insert Password");
            string password = Input.RequestUsernameAndPassword("Insert Password: ");

            return new ClientDTO()
            {
                Username = username,
                Password = password
            };
        }
    }
}
