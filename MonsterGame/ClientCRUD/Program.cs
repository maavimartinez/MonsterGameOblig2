using System;

namespace CRUDClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var crudClientService = new CRUDClientServiceClient();

            while (true)
            {
                Console.Clear();
                crudClientService.Menu();
            }
        }
    }
}
