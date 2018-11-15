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

        static void Main(string[] args)
        {
            int port = GetServerPortFromConfigFile();
            string ip = GetServerIpFromConfigFile();

            string storeServerIp = GetStoreServerIpFromConfigFile();
            int storeServerPort = GetStoreServerPortFromConfigFile();

            Store store = null;
            try
            {
                handler = new ConsoleEventDelegate(ConsoleEventCallback);
                SetConsoleCtrlHandler(handler, true);
                store = (Store)Activator.GetObject(typeof(Store),
                    $"tcp://{storeServerIp}:{storeServerPort}/RemoteStore");
                store.GetClients();
            }
            catch (Exception)
            {
                Console.WriteLine("Store isn't available. Closing app...");
                Thread.Sleep(5000);
                Environment.Exit(0);
            }

            MainController.CreateInstance(store);
            GameLogic gameLogic = MainController.GameLogicInstance();

            var launcher = new ServerLauncher(ip, port);
            launcher.Launch();
            Thread serverThread = launcher.StartAcceptingConnections(gameLogic);

            var prompt = new ServerPrompt(gameLogic);
            try
            {
                prompt.PromptUserForAction();
            }
            catch (Exception)
            {
                Console.WriteLine("Store isn't available. Closing app...");
                Thread.Sleep(5000);
                Environment.Exit(0);
            }

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
            return (string)appSettings.GetValue("StorageServerIp", typeof(string));
        }

        private static int GetStoreServerPortFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (int)appSettings.GetValue("StorageServerPort", typeof(int));
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

        static bool ConsoleEventCallback(int eventType)
        {
            if (IsConsoleClosing(eventType))
            {
                Console.WriteLine("Console window closing");
                Environment.Exit(0);
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