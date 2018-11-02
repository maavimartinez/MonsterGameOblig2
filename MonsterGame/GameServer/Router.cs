using System;
using Protocol;
using System.Collections.Generic;
using System.Linq;

namespace GameServer
{
    public class Router
    {

        private readonly ServerController serverController;

        private LogRouter logRouter;


        public Router(ServerController serverController)
        {
            this.serverController = serverController;
            logRouter = new LogRouter();

        }

        public void Handle(Connection conn)
        {
            while (conn.IsConnected())
            {
                try
                {
                    string[] message = conn.ReadMessage();
                    var request = new Request(message);

                    switch (request.Command)
                    {
                        case Command.Login:
                            serverController.ConnectClient(conn, request);
                            break;
                        case Command.DoAction:
                            List<string> gameFinished= serverController.DoAction(conn, request);
                            ShowIfGameFinished(gameFinished);
                            break;
                        case Command.ListPlayersInGame:
                            serverController.ListPlayersInGame(conn, request);
                            break;
                        case Command.ListAllClients:
                            serverController.ListAllClients(conn, request);
                            break;
                        case Command.ListConnectedClients:
                            serverController.ListConnectedClients(conn, request);
                            break;
                        case Command.DisconnectClient:
                            serverController.DisconnectClient(conn, request);
                            break;
                        case Command.JoinGame:
                            serverController.JoinGame(conn, request);
                            break;
                        case Command.SelectRole:
                            serverController.SelectRole(conn, request);
                            break;
                        case Command.TimesOut:
                            serverController.TimesOut(conn, request);
                            break;
                        case Command.RemovePlayerFromGame:
                            serverController.RemovePlayerFromGame(conn, request);
                            break;
                        case Command.CheckIfGameHasFinished:
                            serverController.CheckIfGameHasFinished(conn, request);
                            break;
                        case Command.GetResultByTimesOut:
                            List<string> result = serverController.GetResultByTimesOut(conn, request);
                            ShowIfGameFinished(result);
                            break;
                        case Command.ReadyToSendPicture:
                            serverController.ReadyToSendPicture(conn, request);
                            break;
                        case Command.SendPicturePart:
                            serverController.SendPicturePart(conn, request);
                            break;
                        case Command.SendLastPicturePart:
                            serverController.SendLastPicturePart(conn, request);
                            break;
                        default:
                            serverController.InvalidCommand(conn);
                            break;
                    }
                }
                catch (Exception e)
                {
                    conn.SendMessage(new string[] { ResponseCode.InternalServerError.ToString(), "There was a problem with the server" });
                    break;
                }
            }
        }

        private void ShowIfGameFinished(List<string> responseMessage)
        {
            for (int i = 0; i < responseMessage.Count(); i++)
            {
                if (responseMessage[i] == "FINISHED")
                {
                    if (responseMessage[i + 1].Equals("Game has finished", StringComparison.OrdinalIgnoreCase))
                    {
                        goto End;
                    }
                    logRouter.LogResult(responseMessage);
                }
            }
            End:;
        }

    }

}