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

                WcfCode result = crudServiceClient.DeleteClient(clientToDelete);
                if (result == WcfCode.False)
                {
                    Console.WriteLine("Client is connected. Press any key.");
                    Console.ReadKey();
                }
                else if (result == WcfCode.Null)
                {
                    Console.WriteLine("\nStore server shut down");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Environment.Exit(0);
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

                WcfCode result = crudServiceClient.UpdateClient(existingClient, editedClient);

                if (result == WcfCode.False)
                {
                    Console.WriteLine("The client is already connected, you can't update his info.  Press any key.");
                    Console.ReadKey();
                }
                else if (result == WcfCode.Null)
                {
                    Console.WriteLine("\nStore server shut down");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Environment.Exit(0);
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
            ICollection<ClientDTO> aux = crudServiceClient.GetClients();
            List<ClientDTO> existingClients = new List<ClientDTO>();

            if (aux == null)
            {
                Console.WriteLine("\nStore server shut down");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else if (aux.Count == 0)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                existingClients = aux.ToList();
            }

            var existingClientsUsernames = new List<string>();

            existingClients.ForEach(ec => existingClientsUsernames.Add(ec.Username));

            int option = Menus.MapExistingClients(existingClientsUsernames);
            option--;

            string selectedClientUsername = existingClientsUsernames[option];

            return existingClients.Find(ec => ec.Username.Equals(selectedClientUsername));
        }

        private void CreateClient()
        {
            WcfCode created = WcfCode.False;

            while (created == WcfCode.False)
            {
                ClientDTO clientToCreate = AskNewClientInfo();

                created = crudServiceClient.CreateClient(clientToCreate);

                if (created == WcfCode.False)
                    Console.WriteLine("\nUsername taken");
                if (created == WcfCode.Null)
                {
                    Console.WriteLine("\nStore server shut down");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
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
            ICollection<RankingDTO> aux = crudServiceClient.GetRanking();
            List<RankingDTO> ranking = new List<RankingDTO>();
            if (aux == null)
            {
                Console.WriteLine("\nStore server shut down");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {
                ranking = aux.ToList();
                if (ranking != null && ranking.Count > 0)
                {
                    foreach (RankingDTO ri in ranking)
                    {
                        int index = ranking.IndexOf(ri);
                        index = index + 1;
                        Console.WriteLine(index + ")" + ri.ToString());
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
        }

        private void Statistics()
        {
            ICollection<StatisticDTO> aux = crudServiceClient.GetStatistics();
            List<StatisticDTO> statistics = new List<StatisticDTO>();
            if (aux == null)
            {
                Console.WriteLine("\nStore server shut down");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {
                statistics = aux.ToList();
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
