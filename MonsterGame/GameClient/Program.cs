using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace GameClient
{
    class Program
    {
        private static ClientController clientController = new ClientController();

        static void Main(string[] args)
        {
            try
            {
                handler = new ConsoleEventDelegate(ConsoleEventCallback);
                SetConsoleCtrlHandler(handler, true);
                clientController.LoopMenu();
            }
            catch (SocketException e)
            {
                Console.WriteLine("There was a problem connecting to the server, the app will exit");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine("There was a problem with the server, the app will exit");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
                Environment.Exit(1);
            }
        }

        static bool ConsoleEventCallback(int eventType)
        {
            if (IsConsoleClosing(eventType))
            {
                Console.WriteLine("Console window closing, disconnecting client");
                clientController.DisconnectFromServer();
            }
            return false;
        }

        private static bool IsConsoleClosing(int eventType)
        {
            return eventType == 2;
        }

        static ConsoleEventDelegate handler;
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

    }
}