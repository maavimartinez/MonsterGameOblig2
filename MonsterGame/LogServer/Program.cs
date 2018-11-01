﻿using System;
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

            var msmqServer = new MessageQueueServer(gameLogic);
            var msmqServerThread = new Thread(() => msmqServer.Start());
            msmqServerThread.Start();

            Console.Clear();
            Console.WriteLine("Log server running.");
        }
    }
}