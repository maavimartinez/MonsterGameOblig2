using System;
using System.Linq;
using Business.Exceptions;
using Entities;
using System.Collections.Generic;

namespace Business
{
    public class PlayerLogic
    {

        private IStore Store { get; set; }
        private Game activeGame { get; set; }
        private Board board { get; set; }
        private List<Player> allPlayers { get; set; }

        public PlayerLogic(IStore store)
           {
               Store = store;
           }

           public void SelectRole(Client loggedClient, string role)
           {
               Player player;
               if (role == "Survivor")
               {
                   CheckIfGameHasMonster();
                   player = new Survivor();
               }
               else
               {
                   player = new Monster();
               }
               player.Client = loggedClient;
               allPlayers = Store.GetAllPlayers();
               allPlayers.Add(player);
               Store.SetAllPlayers(allPlayers);
           }

           public void JoinPlayerToGame(Player loggedPlayer)
           {
               activeGame = Store.GetGame();
               if (activeGame.Players.Count < 4 && !TimeHasPassed(activeGame.LimitJoiningTime))
               {
                    activeGame.Players.Add(loggedPlayer);
                Store.SetGame(activeGame);
                loggedPlayer.NumOfActions = GetMaxTurn();
                    LocatePlayersInBoard();
               }
               else if (!TimeHasPassed(activeGame.LimitJoiningTime))
               {
                   var remainingTime = activeGame.StartTime.AddMinutes(3) - (DateTime.Now - activeGame.StartTime);
                   throw new FullGameException("Game is full, try again at " + remainingTime.ToString("HH:mm"));
               }
               else
               {
                   var remainingTime = activeGame.StartTime.AddMinutes(3) - (DateTime.Now - activeGame.StartTime);
                   throw new FullGameException("You can no longer join this game, try again at " + remainingTime.ToString("HH:mm"));
               }
           }

           private void CheckIfGameHasMonster()
           {
               activeGame = Store.GetGame();
               if (activeGame != null && activeGame.Players.Count() == 3)
               {
                   int countMonsters = 0;
                   foreach (Player pl in activeGame.Players)
                   {
                       if (pl is Monster) countMonsters++;
                   }
                   if (countMonsters == 0) throw new NoMonstersInGameException();
               }
           }

           private bool TimeHasPassed(double minutes)
           {
               DateTime startTime = Store.GetGame().StartTime;
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

           private void LocatePlayersInBoard()
           {
               activeGame = Store.GetGame();
               board = Store.GetBoard();
               foreach (Player pl in activeGame.Players)
               {
                   if (pl.Position == null)
                   {
                       int[] pos = GeneratePlayerPosition();
                       pl.Position = board.Cells[pos[0], pos[1]];
                       board.Cells[pos[0], pos[1]].Player = pl;
                   }
               }
               Store.SetGame(activeGame);
               Store.SetBoard(board);
           }

           private int[] GeneratePlayerPosition()
           {
               board = Store.GetBoard();
               int[] pos = new int[2];
               Random ran = new Random();
               bool exit = false;
               while (!exit)
               {
                   int x = ran.Next(0, 8);
                   int y = ran.Next(0, 8);
                   if (board.Cells[x, y].Player == null)
                   {
                       pos[0] = x;
                       pos[1] = y;
                       exit = true;
                   }
               }
               return pos;
           }

           private int GetMaxTurn()
           {
               int max = 0;
               foreach (Player pl in Store.GetGame().Players)
               {
                   if (pl.NumOfActions > max) max = pl.NumOfActions;
               }
               if (max % 2 == 1) max = max - 1;
               return max;
           }

       }
}
