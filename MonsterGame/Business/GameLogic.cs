using System.Collections.Generic;
using System;
using System.Linq;
using Business.Exceptions;
using Entities;
using System.Text.RegularExpressions;

namespace Business
{
    public class GameLogic
    {
        public IStore Store { get; set; }
        private Server Server { get; set; }
        private ActionLogic ActionLogic { get; set; }
        private PlayerLogic PlayerLogic { get; set; }
        private CRUDClientLogic ClientLogic { get; set; }
        private string ActiveGameResult { get; set; }
        private Game activeGame { get; set; }
        private Board board { get; set; }
        private List<Player> allPlayers { get; set; }
        private List<RankingItem> ranking { get; set; }
        private List<StatisticItem> statistics { get; set; }

        public GameLogic(IStore store)
        {
            Store = store;
            Server = new Server();
            ActionLogic = new ActionLogic(Store);
            PlayerLogic = new PlayerLogic(Store);
            ClientLogic = new CRUDClientLogic(Store);
        }

        public bool CreateClient(Client client)
        {
            return ClientLogic.CreateClient(client);
        }

        public bool UpdateClient(Client oldClient, Client newClient)
        {
            return ClientLogic.UpdateClient(oldClient, newClient);
        }

        public bool DeleteClient(Client client)
        {
            return ClientLogic.DeleteClient(client);
        }

        public string Login(Client client)
        {
            if (!Store.ClientExists(client))
                Store.AddClient(client);
            Client storedClient = Store.GetClient(client.Username);
            bool isValidPassword = storedClient.ValidatePassword(client.Password);
            bool isClientConnected = Server.IsClientConnected(client);
            if (isValidPassword && isClientConnected)
                throw new ClientAlreadyConnectedException();
            return isValidPassword ? Server.ConnectClient(storedClient) : "";
        }

        public Client GetLoggedClient(string userToken)
        {
            Client loggedUser = Server.GetLoggedClient(userToken);
            if (loggedUser == null)
                throw new ClientNotConnectedException();
            return loggedUser;
        }

        public List<Client> GetLoggedClients()
        {
            return Server.GetLoggedClients();
        }

        public Player GetLoggedPlayer(string username)
        {
            return Store.GetLoggedPlayer(username);
        }

        public List<Player> GetLoggedPlayers()
        {
            List<Client> loggedClients = Server.GetLoggedClients();
            List<Player> ret = new List<Player>();
            allPlayers = Store.GetAllPlayers();
            foreach (Client cl in loggedClients)
            {
                foreach (Player pl in allPlayers)
                {
                    if (cl.Username.Equals(pl.Client.Username))
                        ret.Add(pl);
                }
            }
            return ret;
        }

        public List<Client> GetClients()
        {
            return Store.GetClients();
        }

        public List<Player> GetCurrentPlayers()
        {
            try
            {
                return Store.GetGame().Players;
            }
            catch (NullReferenceException)
            {
                return new List<Player>();
            }
        }

        public void DisconnectClient(string token)
        {
            Server.DisconnectClient(token);
        }

        public void SelectRole(Client loggedClient, string role)
        {
            if (loggedClient == null)
                throw new ClientNotConnectedException();

            PlayerLogic.SelectRole(loggedClient, role);
        }

        public void JoinGame(string usernameFrom)
        {
            Player logged = Store.GetLoggedPlayer(usernameFrom);
            if (logged == null) throw new RoleNotChosenException();
            InitializeGame();
            PlayerLogic.JoinPlayerToGame(logged);
        }

        public List<string> DoAction(string usernameFrom, string action)
        {
            List<string> ret = new List<string>();
            activeGame = Store.GetGame();
            Player player = GetLoggedPlayer(usernameFrom);
            if (!player.IsAlive) throw new LoggedPlayerIsDeadException();
            if (!activeGame.isOn)
            {
                ret.Add("FINISHED");
                ret.Add(ActiveGameResult.ToUpper());
                return ret;
            }
            else
            {
                try
                {
                    ret = ret.Concat(ActionLogic.DoAction(player, action)).ToList();
                    List<string> ended = CheckIfGameHasEnded();
                    if (ended != null) ret = ret.Concat(ended).ToList();
                }
                catch (WaitForTurnException)
                {
                    List<string> ended = CheckIfGameHasEnded();
                    if (ended != null)
                    {
                        ret = ret.Concat(ended).ToList();
                    }
                    else
                    {
                        throw new WaitForTurnException();
                    }
                }
                return ret;
            }
        }

        public void TimesOut(string lastPlayerWantsToLeave)
        {
            bool aux = false;
            if (lastPlayerWantsToLeave.Equals("true", StringComparison.OrdinalIgnoreCase)) aux = true;
            if (aux) throw new LastPlayerAbandonedGameException();
            if ((TimeHasPassed(3)))
            {
                throw new TimesOutException("");
            }
        }

        public List<string> GetGameResultByTimeOut()
        {
            string aliveMonsters = "";
            string aliveSurvivors = "";
            int alivePlayers = 0;
            activeGame = Store.GetGame();
            foreach (Player pl in activeGame.Players)
            {
                if (pl.IsAlive)
                {
                    alivePlayers++;
                    if (pl is Monster) aliveMonsters = aliveMonsters + pl.Client.Username + ",";
                    if (pl is Survivor) aliveSurvivors = aliveSurvivors + pl.Client.Username + ",";
                }
            }
            if (aliveSurvivors != "")
            {
                aliveSurvivors.Trim(',');
                ActiveGameResult = aliveSurvivors + " ,won !";
                CreateGameStatistic(aliveSurvivors);
                return EndGame();
            }
            else if (aliveSurvivors == "")
            {
                ActiveGameResult = "Nobody won :(";
                CreateGameStatistic("Nobody won :(");
                return EndGame();
            }
            return new List<string>();
        }

        public List<string> RemovePlayerFromGame(string username)
        {
            List<string> ret = new List<string>();
            Player player = GetLoggedPlayer(username);
            board = Store.GetBoard();
            activeGame = Store.GetGame();
            allPlayers = Store.GetAllPlayers();
            if (player != null)
            {
                board.Cells[player.Position.X, player.Position.Y].Player = null;
                activeGame.Players.Remove(player);
                allPlayers.Remove(player);
                if (activeGame.Players.Count > 0)
                {
                    Store.SetBoard(board);
                    Store.SetGame(activeGame);
                    Store.SetAllPlayers(allPlayers);
                    return CheckIfGameHasEnded();
                }
                else if (activeGame.Players.Count == 0)
                {
                    ActiveGameResult = "Game has finished";
                    Store.SetBoard(board);
                    Store.SetGame(activeGame);
                    Store.SetAllPlayers(allPlayers);
                    return EndGame();
                }
            }
            else
            {
                ret.Add("Player was not in the game");
            }
            Store.SetBoard(board);
            Store.SetGame(activeGame);
            Store.SetAllPlayers(allPlayers);
            return ret;
        }

        public List<string> EndGame()
        {
            activeGame = Store.GetGame();
            if (activeGame != null)
            {
                activeGame.isOn = false;
                activeGame.Result = "";
                RemovePlayersFromAllPlayers();
                activeGame.Players.Clear();
                List<string> ret = new List<string>();
                ret.Add("FINISHED");
                ret.Add(ActiveGameResult.ToUpper());
                Store.SetGame(activeGame);
                UpdateRanking();
                return ret;
            }
            return null;
        }

        public void UpdateRanking()
        {
            activeGame = Store.GetGame();
            ranking = Store.GetRanking();
            List<Player> gamePlayers = activeGame.Players.OrderByDescending(x => x.Score).ToList();
            if(ranking.Count == 10)
            {
                foreach(Player pl in gamePlayers)
                {
                    for(int j=0; j<ranking.Count; j++)
                    {
                        if(pl.Score > ranking.ElementAt(j).Score)
                        {
                            ranking.RemoveAt(9);
                            RankingItem ri = new RankingItem();
                            ri.GameDate = DateTime.Today;
                            ri.Role = pl.GetType();
                            ri.Score = pl.Score;
                            ranking.Add(ri);
                            break;
                        }
                    }      
                }
            }else
            {
                int i = 0;
                while (ranking.Count < 10 && i < gamePlayers.Count)
                {
                    RankingItem ri = new RankingItem();
                    ri.GameDate = DateTime.Today;
                    ri.Role = gamePlayers[i].GetType();
                    ri.Score = gamePlayers[i].Score;
                    ranking.Add(ri);
                    i++;
                }
            }
            Store.SetRanking(ranking);
        }

        public string GetGameResult()
        {
            if (ActiveGameResult != "")
            {
                return ActiveGameResult.ToUpper();
            }
            else
            {
                return "GameNotFinished";
            }
        }

        private void InitializeGame()
        {
            activeGame = Store.GetGame();
            board = Store.GetBoard();
            if (activeGame == null) activeGame = new Game();
            if (board == null) board = new Board();
            if (activeGame.Players.Count == 0)
            {
                activeGame.isOn = true;
                activeGame.StartTime = DateTime.Now;
                ActiveGameResult = "";
                board.InitializeBoard();
            }
            Store.SetGame(activeGame);
            Store.SetBoard(board);
        }

        private List<string> CheckIfGameHasEnded()
        {
            string aliveMonsters = "";
            string aliveSurvivors = "";
            int alivePlayers = 0;
            activeGame = Store.GetGame();
            foreach (Player pl in activeGame.Players)
            {
                if (pl.IsAlive)
                {
                    alivePlayers++;
                    if (pl is Monster) aliveMonsters = aliveMonsters + pl.Client.Username + ",";
                    if (pl is Survivor) aliveSurvivors = aliveSurvivors + pl.Client.Username + ",";
                }
            }
            if (aliveMonsters == "" && TimeHasPassed(activeGame.LimitJoiningTime))
            {
                aliveSurvivors = aliveSurvivors.Trim(',');
                ActiveGameResult = aliveSurvivors + " ,won !";
                CreateGameStatistic(aliveSurvivors);
                return EndGame();
            }
            else if (alivePlayers == 1 && aliveSurvivors == "" && TimeHasPassed(activeGame.LimitJoiningTime))
            {
                aliveMonsters = aliveMonsters.Trim(',');
                ActiveGameResult = aliveMonsters + " ,won !";
                CreateGameStatistic(aliveMonsters);
                return EndGame();
            }
            return new List<string>();
        }

        private bool TimeHasPassed(double minutes)
        {
            activeGame = Store.GetGame();
            DateTime startTime = activeGame.StartTime;
            DateTime endTime = startTime.AddMinutes(minutes);
            DateTime now = DateTime.Now;
            if (now < endTime)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void RemovePlayersFromAllPlayers()
        {
            allPlayers = Store.GetAllPlayers();
            foreach (Player pl in Store.GetGame().Players)
            {
                allPlayers.Remove(pl);
            }
            Store.SetAllPlayers(allPlayers);
        }

        private void CreateGameStatistic(string winners)
        {
            activeGame = Store.GetGame();
            statistics = Store.GetStatistics();
            statistics.RemoveAt(0);
            StatisticItem gameSt = new StatisticItem();
            if (winners.Equals("Nobody won :("))
            {
                foreach(Player pl in activeGame.Players)
                {
                    StatisticDetail sd = new StatisticDetail();
                    sd.Outcome = "Lost";
                    sd.Role = pl.GetType();
                    sd.Username = pl.Client.Username;
                    gameSt.gameStatistic.Add(sd);
                }
            }else
            {
                foreach (Player pl in activeGame.Players)
                {
                    StatisticDetail sd = new StatisticDetail();
                    sd.Role = pl.GetType();
                    sd.Username = pl.Client.Username;     
                    if (PlayerIsInWinnerString(pl, winners))
                    {
                        sd.Outcome = "Won";
                    }else
                    {
                        sd.Outcome = "Lost";
                    }
                    gameSt.gameStatistic.Add(sd);
                }
            }
            statistics.Add(gameSt);
            Store.SetStatistics(statistics);
        }

        private bool PlayerIsInWinnerString(Player pl, string winnerString)
        {
            string[] winners = winnerString.Split(',');
            foreach(string username in winners)
            {
                if (username.Equals(pl.Client.Username, StringComparison.CurrentCultureIgnoreCase)) return true;
            }
            return false;
        }
           
            
        }

    }
}