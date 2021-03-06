﻿using System.Collections.Generic;
using System;
using System.Linq;
using Business.Exceptions;
using Entities;
using System.Net.Sockets;

using System.Text.RegularExpressions;

namespace Business
{
    public class GameLogic
    {
        public IStore Store { get; set; }
        private ActionLogic ActionLogic { get; set; }
        private PlayerLogic PlayerLogic { get; set; }
        private CRUDClientLogic ClientLogic { get; set; }
        private Game activeGame { get; set; }
        private Board board { get; set; }
        private List<Player> allPlayers { get; set; }
        private List<RankingDTO> ranking { get; set; }
        private List<StatisticDTO> statistics { get; set; }

        private readonly object resultByTimeout = new object();


        public GameLogic(IStore store)
        {
            Store = store;
            ActionLogic = new ActionLogic(Store);
            PlayerLogic = new PlayerLogic(Store);
            ClientLogic = new CRUDClientLogic(Store);
        }

        public bool CreateClient(Client client)
        {
            try
            {
                return ClientLogic.CreateClient(client);
            }
            catch (SocketException)
            {
                throw new SocketException();
            }
        }

        public bool UpdateClient(Client oldClient, Client newClient)
        {
            try
            {
                return ClientLogic.UpdateClient(oldClient, newClient);
            }
            catch (SocketException)
            {
                throw new SocketException();
            }
        }

        public bool DeleteClient(Client client)
        {
            try
            {
                return ClientLogic.DeleteClient(client);
            }
            catch (SocketException)
            {
                throw new SocketException();
            }
        }

        public string Login(Client client)
        {
            if (!Store.ClientExists(client))
                Store.AddClient(client);
            Client storedClient = Store.GetClient(client.Username);
            bool isValidPassword = storedClient.ValidatePassword(client.Password);
            bool isClientConnected = Store.IsClientConnected(client);
            if (isValidPassword && isClientConnected)
                throw new ClientAlreadyConnectedException();
            return isValidPassword ? Store.ConnectClient(storedClient) : "";
        }

        public Client GetLoggedClient(string userToken)
        {
            Client loggedUser = Store.GetLoggedClient(userToken);
            if (loggedUser == null)
                throw new ClientNotConnectedException();
            return loggedUser;
        }

        public List<Client> GetLoggedClients()
        {
            return Store.GetLoggedClients();
        }

        public Player GetLoggedPlayer(string username)
        {
            return Store.GetLoggedPlayer(username);
        }

        public List<Player> GetLoggedPlayers()
        {
            List<Client> loggedClients = Store.GetLoggedClients();
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
            try
            {
                return Store.GetClients();
            }
            catch (SocketException)
            {
                throw new SocketException();
            }
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
            catch (SocketException)
            {
                throw new SocketException();
            }
        }

        public void DisconnectClient(string token)
        {
             Store.DisconnectClient(token);
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
            Store.AddOriginalPlayer("Player: " + usernameFrom);
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
                ret.Add(Store.GetGameResult());
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
                    if (ended.Count != 0)
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
            lock (resultByTimeout)
            {
                string activeGameResult = Store.GetGameResult();
                if (activeGameResult == "")
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
                        Store.SetGameResult(aliveSurvivors + " won !");
                        CreateGameStatistic(aliveSurvivors);
                        return EndGame();
                    }
                    else if (aliveSurvivors == "")
                    {
                        Store.SetGameResult("Nobody won :(");
                        CreateGameStatistic("Nobody won :(");
                        return EndGame();
                    }
                }
                List<string> aux = new List<string>();
                aux.Add(activeGameResult);
                return aux;
            }
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
                    Store.SetGameResult("Game has finished");
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
                UpdateRanking();
                RemovePlayersFromAllPlayers();
                ResetScores();
                List<string> ret = new List<string>();
                ret.Add("FINISHED");
                ret.Add(Store.GetGameResult());
                ret = ret.Concat(Store.GetOriginalPlayers()).ToList();
                ret.Add(" Actions during the game:");
                foreach (Player p in activeGame.Players)
                {
                    ret.Add(" -" + p.Client.Username + " made " + p.NumOfMovements + " movements and " + p.NumOfAttacks + " attacks");
                }
                activeGame.Players.Clear();
                if (activeGame.PlayersThatDied.Count > 0)
                    ret.Add(" Players that died:");
                ret = ret.Concat(activeGame.PlayersThatDied).ToList();
                activeGame.PlayersThatDied.Clear();
                Store.SetGame(activeGame);
                return ret;
            }
            return null;
        }

        public void UpdateRanking()
        {
            activeGame = Store.GetGame();
            ranking = Store.GetRanking();
            List<Player> gamePlayers = activeGame.Players.OrderByDescending(x => x.Score).ToList();
            if (ranking.Count == 10)
            {
                foreach (Player pl in gamePlayers)
                {
                    for (int j = 0; j < ranking.Count; j++)
                    {
                        if (pl.Score > Int32.Parse(ranking.ElementAt(j).Score))
                        {
                            ranking.RemoveAt(9);
                            RankingDTO ri = new RankingDTO();
                            ri.GameDate = DateTime.Now.ToString();
                            ri.Role = pl.GetType().ToString();
                            ri.Score = pl.Score.ToString();
                            ri.Username = pl.Client.Username;
                            if (!ranking.Contains(ri))
                                ranking.Add(ri);
                            break;
                        }
                    }
                }
            }
            else
            {
                int i = 0;
                while (ranking.Count < 10 && i < gamePlayers.Count)
                {
                    RankingDTO ri = new RankingDTO();
                    ri.GameDate = DateTime.Now.ToString();
                    ri.Role = gamePlayers[i].GetType().ToString();
                    ri.Score = gamePlayers[i].Score.ToString();
                    ri.Username = gamePlayers[i].Client.Username;
                    if (!ranking.Contains(ri))
                        ranking.Add(ri);
                    i++;
                }
            }
            Store.SetRanking(ranking);
        }

        public void ResetScores()
        {
            activeGame = Store.GetGame();
            List<Player> gamePlayers = activeGame.Players;

            foreach (Player pl in gamePlayers)
            {
                pl.Score = 0;
            }
            Store.SetGame(activeGame);
        }

        public List<RankingDTO> Ranking()
        {
            return Store.GetRanking();
        }

        public List<StatisticDTO> Statistics()
        {
            return Store.GetStatistics();
        }

        public string GetGameResult()
        {
            if (Store.GetGameResult() != "")
            {
                return Store.GetGameResult();
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
            if (activeGame == null)
            {
                activeGame = new Game();
            }
            if (board == null) board = new Board();
            if (activeGame.Players.Count == 0)
            {
                activeGame.isOn = true;
                activeGame.StartTime = DateTime.Now;
                Store.SetGameResult("");
                Store.ResetOriginalPlayers();
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
                Store.SetGameResult(aliveSurvivors + " won !");
                CreateGameStatistic(aliveSurvivors);
                return EndGame();
            }
            else if (alivePlayers == 1 && aliveSurvivors == "" && TimeHasPassed(activeGame.LimitJoiningTime))
            {
                aliveMonsters = aliveMonsters.Trim(',');
                Store.SetGameResult(aliveMonsters + " won !");
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

        public void AddLogEntry(LogEntry entry)
        {
            Store.AddLogEntry(entry);

        }

        public LogEntry GetLastLogEntry()
        {
            return Store.GetLastLogEntry();
        }

        public List<LogEntry> GetLogEntries()
        {
            return Store.GetLogEntries();
        }

        private void CreateGameStatistic(string winners)
        {
            activeGame = Store.GetGame();
            statistics = Store.GetStatistics();
            if (statistics.Count == 10) statistics.RemoveAt(0);
            StatisticDTO gameSt = new StatisticDTO();
            gameSt.GameDate = DateTime.Now.ToString();
            gameSt.gameStatistic = new List<StatisticDetail>();
            if (winners.Equals("Nobody won :("))
            {
                foreach (Player pl in activeGame.Players)
                {
                    StatisticDetail sd = new StatisticDetail();
                    sd.Outcome = "Lost";
                    sd.Role = pl.GetType().Name;
                    sd.Username = pl.Client.Username;
                    if (!gameSt.gameStatistic.Contains(sd))
                        gameSt.gameStatistic.Add(sd);
                }
            }
            else
            {
                foreach (Player pl in activeGame.Players)
                {
                    StatisticDetail sd = new StatisticDetail();
                    sd.Role = pl.GetType().Name;
                    sd.Username = pl.Client.Username;
                    if (PlayerIsInWinnerString(pl, winners))
                    {
                        sd.Outcome = "Won";
                    }
                    else
                    {
                        sd.Outcome = "Lost";
                    }
                    if (!gameSt.gameStatistic.Contains(sd))
                        gameSt.gameStatistic.Add(sd);
                }
            }
            if (!statistics.Contains(gameSt))
                statistics.Add(gameSt);
            Store.SetStatistics(statistics);
        }

        private bool PlayerIsInWinnerString(Player pl, string winnerString)
        {
            string[] winners = winnerString.Split(',');
            foreach (string username in winners)
            {
                if (username.Equals(pl.Client.Username, StringComparison.CurrentCultureIgnoreCase)) return true;
            }
            return false;
        }


    }
}
