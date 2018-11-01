using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Business;
using Business.Exceptions;
using Persistence;
using Protocol;
using UI;
using System.Runtime.InteropServices;

namespace GameServer
{

    class Program
    {

        private static List<Thread> threads = new List<Thread>();
        private static List<Connection> connections = new List<Connection>();
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        static void Main(string[] args)
        {
            int port = GetServerPortFromConfigFile();
            string ip = GetServerIpFromConfigFile();

            string storeServerIp = GetStoreServerIpFromConfigFile();
            int storeServerPort = GetStoreServerPortFromConfigFile();

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

            MainController.Build(store);
            GameLogic gameLogic = MainController.GameLogicInstance();

            var launcher = new ServerLauncher(ip, port);
            launcher.Launch();
            Thread serverThread = launcher.StartAcceptingConnections(gameLogic);

            var prompt = new ServerPrompt(gameLogic);
            prompt.PromptUserForAction();

            serverThread.Join();
        }

        private static string GetServerIpFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (string)appSettings.GetValue("ServerIp", typeof(string));
        }

        private static int GetServerPortFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (int)appSettings.GetValue("ServerPort", typeof(int));
        }

        private static string GetStoreServerIpFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (string)appSettings.GetValue("StoreServerIp", typeof(string));
        }

        private static int GetStoreServerPortFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (int)appSettings.GetValue("StoreServerPort", typeof(int));
        }
    

        private static bool GameIsOff(GameLogic controller)
        {
            return controller.Store.GetGame() == null || !controller.Store.GetGame().isOn;
        }

        private static void CloseThreads()
        {
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            Console.WriteLine("Every thread has been closed. Good-bye.");
        }

    }

}