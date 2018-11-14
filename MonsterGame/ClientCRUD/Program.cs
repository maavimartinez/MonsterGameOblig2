using System;
using System.Net.Sockets;


namespace CRUDClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var crudClientService = new CRUDClientServiceClient();

            while (true)
            {
                try
                {
                    Console.Clear();
                    crudClientService.Menu();
                }
            
                  catch (SocketException e)
                {
                    Console.WriteLine("There was a problem connecting to the storage server, the app will exit");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }
        }
    }
}
