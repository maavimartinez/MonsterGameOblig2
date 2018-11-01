using System;
using System.Threading;
using Business;
using Persistence;

namespace LogServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string storeServerIp = Utillities.GetStoreServerIpFromConfigFile();
            int storeServerPort = Utillities.GetStoreServerPortFromConfigFile();
            Store store = null;
            try
            {
                store = (Store)Activator.GetObject(typeof(Store),
                    $"tcp://{storeServerIp}:{storeServerPort}/{StoreUtillities.StoreName}");
                store.GetClients();
            }
            catch (Exception)
            {
                Console.WriteLine("Store isn't available. Closing app...");
                Thread.Sleep(5000);
                Environment.Exit(0);
            }
            CoreController.Build(store);
            BusinessController businessController = CoreController.BusinessControllerInstance();

            var msmqServer = new MessageQueueServer(businessController);
            var msmqServerThread = new Thread(() => msmqServer.Start());
            msmqServerThread.Start();

            Console.Clear();
            Console.WriteLine("Log server running.");
        }
    }
}
