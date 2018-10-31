using System;
using WebServices;
using Business;
using Persistence;
using System.Threading;

namespace CRUDServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string storeServerIp = Settings.GetStoreServerIpFromConfigFile();
            int storeServerPort = Settings.GetStoreServerPortFromConfigFile();

            Store store = null;
            try
            {
                store = (Store)Activator.GetObject(typeof(Store),
                    $"tcp://{storeServerIp}:{storeServerPort}/{StoreSettings.StoreName}");
                store.GetClients();
            }
            catch (Exception)
            {
                Console.WriteLine("Store isn't available. Closing app...");
                Thread.Sleep(5000);
                Environment.Exit(0);
            }

            CoreController.Build(store);

            WCFHost wcfHostService = new WCFHost();
            var wcfHostThread = new Thread((() => wcfHostService.Start()));
            wcfHostThread.Start();

            Console.WriteLine("CRUD Server running. Click any key to stop...");
            Console.ReadKey();

            wcfHostService.Stop();
            wcfHostThread.Abort();
        }
    }
}
