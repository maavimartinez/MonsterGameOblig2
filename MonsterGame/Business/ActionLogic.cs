﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Business.Exceptions;
using Entities;

namespace Business
{

    public class ActionLogic
    {

        const int RADIUS = 1;
        const int WIDTH = 8;
        const int HEIGHT = 8;

        private IStore Store { get; set; }
        private Game activeGame { get; set; }
        private Board board { get; set; }
        private List<Player> allPlayers { get; set; }

        public ActionLogic(IStore store)
        {
            Store = store;
        }

        public List<string> DoAction(Player player, string action)
        {
            List<string> ret = new List<string>();
            bool right = CheckRightTurn(player);
            ret = TranslateAndDoAction(player, action);
            int x = player.Position.X;
            int y = player.Position.Y;
            ret = ret.Concat(GetNearPlayers(x, y)).ToList();
            ret = ret.Concat(GetPlayerHP(player)).ToList();
            return ret;
        }

      
        public List<string> GetNearPlayers(int x, int y)
        {
            board = Store.GetBoard();
            List<string> nearPlayers = new List<string>();
            nearPlayers.Add("NEAR");

            var lowerY = Math.Max(0, y - RADIUS);
            var upperY = Math.Min(WIDTH - 1, y + RADIUS);
            var lowerX = Math.Max(0, x - RADIUS);
            var upperX = Math.Min(HEIGHT - 1, x + RADIUS);
            for (int i = lowerX; i <= upperX; ++i)
            {
                for (int j = lowerY; j <= upperY; ++j)
                {
                    if (board.Cells[i, j].Player != null && (i != x || j != y))
                    {
                        string nearUsername = board.Cells[i, j].Player.Client.Username;
                        string role = board.Cells[i, j].Player.ToString();

                        nearPlayers.Add(nearUsername + "(" + role + ")");
                    }
                }
            }
            if (nearPlayers.Count == 1)
            {
                return new List<string>();
            }
            else
            {
                return nearPlayers;
            }
        }

        public List<string> GetPlayerHP(Player player)
        {
            List<string> ret = new List<string>();
            ret.Add("HP");
            ret.Add(player.HP + "");
            return ret;
        }

        public Player GetDefender(string username)
        {
            foreach (Player pl in Store.GetGame().Players)
            {
                if (pl.Client.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    return pl;
                }
            }
            throw new ClientNotConnectedException("Defender does not exist");
        }

        private bool CheckRightTurn(Player player)
        {
            int minTurn = GetMinTurn();
            bool youAreMinTurn = player.NumOfActions == minTurn;
            int rest = minTurn % 2;
            int difference = player.NumOfActions - minTurn;
            if ((rest == 0 && difference < 2) || youAreMinTurn)
            {
                return true;
            }
            else
            {
                throw new WaitForTurnException();
            }
        }

        private int GetMinTurn()
        {
            int min = Int32.MaxValue;
            foreach (Player pl in Store.GetGame().Players)
            {
                if (pl.NumOfActions < min) min = pl.NumOfActions;
            }
            return min;
        }

        private List<string> TranslateAndDoAction(Player player, string cmd)
        {
            List<string> ret = new List<string>();
            string aux = cmd.Replace(" ", String.Empty).ToUpper();
            if (aux.Length < 5) throw new ActionException("Invalid action format");
            string action = aux.Substring(0, 3);
            string sndParameter = aux.Substring(3);
            if (action.Equals("MOV"))
            {
                CheckCorrectMoveFormat(sndParameter);
                Move(player, sndParameter);
                player.NumOfActions++;
                ret = new List<string>();
            }
            else if (action.Equals("ATT"))
            {
                Player defender = GetDefender(sndParameter);
                ret = Attack(player, defender);
                player.NumOfActions++;
            }
            else
            {
                throw new ActionException("Invalid Action");
            }
            return ret;
        }

        private void CheckCorrectMoveFormat(string move)
        {
            char row = move[0];
            int column = (int)Char.GetNumericValue(move[1]) - 1;
            bool validRow = Regex.IsMatch(row.ToString(), "[a-h]", RegexOptions.IgnoreCase);
            bool validColumn = (column >= 0 && column <= 7);
            if (move.Length > 2 || !validRow || !validColumn) throw new Exception("Invalid format");
        }

        private void Move(Player player, string pos)
        {
            int y = pos[0] - 65;
            int x = (int)Char.GetNumericValue(pos[1]) - 1;
            CheckValidMovement(player, x, y);
            CheckFreePosition(x, y);
            RemovePlayerFromCell(player);
            LocatePlayerOnCell(player, x, y);
        }

        private void CheckValidMovement(Player player, int x, int y)
        {
            var lowerY = Math.Max(0, player.Position.Y - RADIUS);
            var upperY = Math.Min(WIDTH - 1, player.Position.Y + RADIUS);
            var lowerX = Math.Max(0, player.Position.X - RADIUS);
            var upperX = Math.Min(HEIGHT - 1, player.Position.X + RADIUS);
            if (x < lowerX || x > upperX || y < lowerY || y > upperY)
                throw new MovementOutOfBoundsException();
            if (player.Position.Y == y && player.Position.X == x)
                throw new SamePlaceMovementException();
        }

        private void CheckFreePosition(int x, int y)
        {
            if (Store.GetBoard().Cells[x, y].Player != null) throw new CellAlreadyContainsAPlayerException();
        }

        private void LocatePlayerOnCell(Player player, int x, int y)
        {
            board = Store.GetBoard();
            player.Position = board.Cells[x, y];
            board.Cells[x, y].Player = player;
            Store.SetBoard(board);
        }

        private void RemovePlayerFromCell(Player player)
        {
            board = Store.GetBoard();
            int x = player.Position.X;
            int y = player.Position.Y;
            board.Cells[x, y].Player = null;
            Store.SetBoard(board);
        }

        private List<string> Attack(Player attacker, Player defender)
        {
            if (attacker.GetType() == typeof(Survivor) && defender.GetType() == typeof(Survivor))
                throw new ActionException("Survivor can't attack survivors");
            defender.HP = defender.HP - attacker.AP;
            if (defender.HP == 0) defender.IsAlive = false;
            List<string> ret = new List<string>();
            if (!defender.IsAlive)
            {
                ret.Add("KILLED");
                ret.Add(defender.Client.Username);
            }
            return ret;
        }

    }

}
