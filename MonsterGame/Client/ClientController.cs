﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Linq;
using Protocol;
using UI;
using System.Drawing;
using System.IO;


namespace Client
{
    public class ClientController
    {

        private const double WaitTimeAumentation = 1.5;
        private const int InitialWaitTime = 100;
        private readonly ClientProtocol clientProtocol;
        private string clientToken;
        private string clientUsername;
        private Connection SocketConnection { get; set; }
        private Connection TimeControllerConnection { get; set; }
        private bool timesOut = false;
        private bool exitGame = false;
        private bool LastPlayerWantsToLeave = false;
        private Thread timer;

        public ClientController()
        {
            clientToken = "";
            clientUsername = null;
            string serverIp = GetServerIp();
            int serverPort = GetServerPort();
            string clientIp = GetClientIp();
            int clientPort = GetClientPort();
            clientProtocol = new ClientProtocol(serverIp, serverPort, clientIp, clientPort);
        }

        internal void LoopMenu()
        {
            Init();
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine(ClientUI.Title(clientUsername));
                int option = Menus.ClientControllerLoopMenu();
                if (option == 4) exit = true;
                MapOptionToActionOfMainMenu(option);
                if (!exitGame)
                {
                    ClientUI.Clear();
                }
                if (exitGame)
                {
                    ClientUI.ClearBoard();
                }
                exitGame = false;
            }
        }

        public void DisconnectFromServer()
        {
            SocketConnection.SendMessage(BuildRequest(Command.DisconnectClient));
            var response = new Response(SocketConnection.ReadMessage());
            if (response.HadSuccess())
            {
                Console.WriteLine("Disconnected");
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
            }
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }
        }

        private void Init()
        {
            Console.WriteLine(ClientUI.Title());
            ConnectToServer();
            ClientUI.Clear();
        }

        private void MapOptionToActionOfMainMenu(int option)
        {
            switch (option)
            {
                case 1:
                    ListConnectedClients();
                    break;
                case 2:
                    UploadImage(clientUsername);
                    break;
                case 3:
                    Play();
                    break;
                default:
                    DisconnectFromServer();
                    break;
            }

        }

        private void ConnectToServer()
        {
            Console.WriteLine(ClientUI.Connecting());
            bool connected;
            do
            {
                Entities.Client client = AskForCredentials();
                SocketConnection = clientProtocol.ConnectToServer();
                object[] request = BuildRequest(Command.Login, client.Username, client.Password);
                SocketConnection.SendMessage(request);
                var response = new Response(SocketConnection.ReadMessage());
                connected = response.HadSuccess();
                if (connected)
                {
                    clientToken = response.GetClientToken();
                    clientUsername = client.Username;
                    ClientUI.LoginSuccessful();
                }
                else
                {
                    Console.WriteLine(response.ErrorMessage());
                }
            } while (!connected);
        }

        private bool ListConnectedClients()
        {
            bool serverHasClients;
            object[] request = BuildRequest(Command.ListConnectedClients);
            SocketConnection.SendMessage(request);

            var response = new Response(SocketConnection.ReadMessage());
            if (response.HadSuccess())
            {
                ClientUI.TheseAreTheConnectedPlayers();
                List<string> connectedClients = response.UserList();
                PrintPlayers(connectedClients);
                serverHasClients = connectedClients.Count > 0;
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
                serverHasClients = false;
            }
            return serverHasClients;
        }

        private List<string> GetListOfAllClients()
        {
            var clients = new List<string>();
            object[] request = BuildRequest(Command.ListAllClients);
            SocketConnection.SendMessage(request);

            var response = new Response(SocketConnection.ReadMessage());
            if (response.HadSuccess())
            {
                clients = response.UserList();
            }

            return clients;
        }

        private List<string> ListPlayersInGame()
        {
            var friends = new List<string>();

            object[] request = BuildRequest(Command.ListPlayersInGame);
            SocketConnection.SendMessage(request);

            var response = new Response(SocketConnection.ReadMessage());
            if (response.HadSuccess())
            {
                friends = response.UserList();
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
            }

            return friends;
        }

        private void UploadImage(string username)
        {
        AskForPath:
            ClientUI.InsertAvatarPath();
            string path = Input.RequestInput();
            if (path.Equals("EXIT", StringComparison.OrdinalIgnoreCase)) goto End;
            path = Path.Combine(path, "");
            const int CHUNK_SIZE = 9999;
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                string extension = fileInfo.Extension;

                int totalLength = (int)fileInfo.Length;

                Command command = Command.SendPicturePart;

                double splittingTimes = (double)totalLength / CHUNK_SIZE;

                SocketConnection.SendMessage(BuildRequest(Command.ReadyToSendPicture, username, totalLength, extension));
                var response = new Response(SocketConnection.ReadMessage());

                if (response.HadSuccess())
                {
                    using (FileStream fs = fileInfo.OpenRead())
                    {
                        var read = 0;
                        while (read < totalLength)
                        {
                            if (splittingTimes < 1 || splittingTimes == 1)
                            {
                                command = Command.SendLastPicturePart;
                            }
                            byte[] parts = new byte[CHUNK_SIZE];

                            read += fs.Read(parts, 0, CHUNK_SIZE);

                            var stringBasedPart = Convert.ToBase64String(parts);

                            SocketConnection.SendMessage(BuildRequest(command, stringBasedPart));
                            var keepSendingResponse = new Response(SocketConnection.ReadMessage());

                            splittingTimes--;

                            if (!keepSendingResponse.HadSuccess())
                            {
                                Console.WriteLine("\n -> Picture coudn't be uploaded\n");
                                goto End;
                            }
                        }
                    }
                    Console.WriteLine("\n -> Upload was successful\n");
                }
                else
                {
                    Console.WriteLine(response.ErrorMessage());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n -> The path is invalid.\n Please try again or type 'exit' to leave.\n");
                goto AskForPath;
            }
        End:;
        }

        private void PrintPlayers(List<string> players)
        {
            players.ForEach(Console.WriteLine);
        }

        private string GetServerIp()
        {
            var appSettings = new AppSettingsReader();
            return (string)appSettings.GetValue("ServerIp", typeof(string));
        }

        private int GetServerPort()
        {
            var appSettings = new AppSettingsReader();
            return (int)appSettings.GetValue("ServerPort", typeof(int));
        }

        private string GetClientIp()
        {
            var appSettings = new AppSettingsReader();
            return (string)appSettings.GetValue("ClientIp", typeof(string));
        }

        private int GetClientPort()
        {
            var appSettings = new AppSettingsReader();
            return (int)appSettings.GetValue("ClientPort", typeof(int));
        }

        private Entities.Client AskForCredentials()
        {
            ClientUI.LoginTitle();

            ClientUI.InsertUsername();
            string username = Input.RequestUsernameAndPassword("Insert Username: ");

            ClientUI.InsertPassword();
            string password = Input.RequestUsernameAndPassword("Insert Password: ");

            return new Entities.Client(username, password);
        }

        private object[] BuildRequest(Command command, params object[] payload)
        {
            List<object> request = new List<object>(payload);
            request.Insert(0, command.GetHashCode());
            request.Insert(1, clientToken);

            return request.ToArray();
        }

        private void Play()
        {
            exitGame = false;
            timesOut = false;
            LastPlayerWantsToLeave = false;
            int input = Menus.SelectRoleMenu();
            input--;
            string role = "";
            if (input == 0) role = "Monster";
            if (input == 1) role = "Survivor";
            if (input == 2) goto End;

            SocketConnection.SendMessage(BuildRequest(Command.SelectRole, role));

            var response = new Response(SocketConnection.ReadMessage());

            if (response.HadSuccess())
            {
                Console.WriteLine("You are now a " + role);
                ClientUI.Clear();
                JoinGame();
                goto End;
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
            }
        End:;
        }

        private void JoinGame()
        {
            SocketConnection.SendMessage(BuildRequest(Command.JoinGame));

            var response = new Response(SocketConnection.ReadMessage());

            if (response.HadSuccess())
            {

                BoardUI.DrawBoard(clientUsername, response.GetPlayerPosition());
                Console.WriteLine("Action: ");

                if (timer == null)
                {
                    timer = new Thread(() => TimesOut());
                    timer.Start();
                }

                while (!exitGame && !timesOut)
                {

                    string myAction = Input.RequestInput();

                    if (timesOut) goto End;

                    if (myAction.Equals("exit"))
                    {
                        RemovePlayerFromGame();
                        exitGame = true;
                    }
                    else
                    {
                        SocketConnection.SendMessage(BuildRequest(Command.DoAction, myAction));

                        var sendActionResponse = new Response(SocketConnection.ReadMessage());

                        if (sendActionResponse.HadSuccess())
                        {
                            List<string> actionResponse = sendActionResponse.GetDoActionResponse();
                            RefreshBoard(actionResponse);
                            ShowIfGameFinished(actionResponse, false);
                        }
                        else if (sendActionResponse.IsInvalidAction())
                        {
                            Console.WriteLine(sendActionResponse.ErrorMessage());
                            Console.WriteLine("Action: ");
                        }
                        else if (sendActionResponse.PlayerIsDead())
                        {
                            Console.WriteLine(sendActionResponse.ErrorMessage());
                            string st = AskServerIfGameHasFinished();
                            if (st != "GameFinished")
                            {
                                Console.WriteLine("Please wait until game has finished. Type any key to continue...");
                            }
                            else
                            {
                                goto End;
                            }
                        }
                        else
                        {
                            Console.WriteLine(sendActionResponse.ErrorMessage());
                            RemovePlayerFromGame();
                        }
                    }
                }
                ClientUI.Clear();
            }
            else if (response.GameIsFull())
            {
                Console.WriteLine(response.ErrorMessage());
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
                string aux = AskServerIfGameHasFinished();
            }
        End:;
        }

        public void RemovePlayerFromGame()
        {
            SocketConnection.SendMessage(BuildRequest(Command.RemovePlayerFromGame));

            var response = new Response(SocketConnection.ReadMessage());

            if (response.HadSuccess())
            {
                ShowIfGameFinished(response.GetRemovePlayerFromGameResponse(), false);
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
            }
        }

        private string AskServerIfGameHasFinished()
        {
            bool exit = false;
            while (!exit)
            {
                SocketConnection.SendMessage(BuildRequest(Command.CheckIfGameHasFinished));

                var response = new Response(SocketConnection.ReadMessage());

                if (response.HadSuccess())
                {
                    string result = response.GetGameResult();
                    if (result != "GameNotFinished")
                    {
                        Console.WriteLine(result);
                        exit = true;
                        return "GameFinished";
                    }
                    else
                    {
                        return "GameNotFinished";
                    }
                }
                else
                {
                    Console.WriteLine(response.ErrorMessage());
                    exit = true;
                }
            }
            return null;
        }

        private void TimesOut()
        {
            TimeControllerConnection = clientProtocol.ConnectToServer();
            while (!timesOut)
            {
                string aux = "false";
                if (LastPlayerWantsToLeave) aux = "true";
                TimeControllerConnection.SendMessage(BuildRequest(Command.TimesOut, aux));

                var response = new Response(TimeControllerConnection.ReadMessage());

                if (response.GameHasFinished())
                {
                    GetResultByTimesOut();
                }
                else if (response.LastPlayerAbandoned())
                {
                    timesOut = true;
                    timer = null;
                }
            }
        }

        private void GetResultByTimesOut()
        {
            if (SocketConnection.IsConnected())
            {
                SocketConnection.SendMessage(BuildRequest(Command.GetResultByTimesOut));

                var response = new Response(SocketConnection.ReadMessage());

                ShowIfGameFinished(response.GetTimeOutResponse(), true);
                Console.WriteLine("Insert any key to continue...");
            }
        }

        private void ShowIfGameFinished(List<string> responseMessage, bool timesOut2)
        {
            for (int i = 0; i < responseMessage.Count(); i++)
            {
                if (responseMessage[i] == "FINISHED")
                {
                    if (responseMessage[i + 1].Equals("Game has finished", StringComparison.OrdinalIgnoreCase))
                    {
                        LastPlayerWantsToLeave = true;
                        goto End;
                    }
                    if (timesOut2) Console.WriteLine("Active Game's time is over!. You can now join a new game.");
                    if (!timesOut2) Console.WriteLine("Game is over! ");
                    Console.WriteLine(responseMessage[i + 1]);
                    exitGame = true;
                    timesOut = true;
                    timer = null;
                }
            }
        End:;
        }

        private void RefreshBoard(List<string> response)
        {
            BoardUI.DrawBoard(clientUsername, response[0]);
            BoardUI.ShowHP(GetHP(response));
            BoardUI.ShowKills(GetKills(response));
            BoardUI.ShowNearPlayers(GetNearPlayers(response));
            Console.WriteLine("Action: ");
        }

        private string GetHP(List<string> response)
        {
            for (int i = 0; i < response.Count(); i++)
            {
                if (response[i].Equals("HP"))
                    return response[i + 1];
            }
            return "";
        }

        private string GetKills(List<string> response)
        {
            for (int i = 0; i < response.Count(); i++)
            {
                if (response[i].Equals("KILLED"))
                {
                    return response[i + 1];
                }
            }
            return "";
        }

        private List<string> GetNearPlayers(List<string> response)
        {
            List<string> near = new List<string>();
            for (int i = 0; i < response.Count(); i++)
            {
                if (response[i].Equals("NEAR"))
                {
                    for (int j = i + 1; j < response.Count() && !response[j].Equals("HP") && !response[j].Equals("FINISHED"); j++)
                    {
                        near.Add(response[j]);
                    }
                }
            }
            return near;
        }

    }

}
