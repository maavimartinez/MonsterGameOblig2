using System;
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
            activeGame = Store.GetGame();
            player = activeGame.Players.Find(h => h.Client.Username == player.Client.Username);
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
                Game auxGame = Store.GetGame();
                auxGame.Players.Find(h => h.Client.Username == player.Client.Username).NumOfActions++;
                 auxGame.Players.Find(h => h.Client.Username == player.Client.Username).NumOfMovements++;
                ret = new List<string>();
                Store.SetGame(auxGame);

            }
            else if (action.Equals("ATT"))
            {
                Player defender = GetDefender(sndParameter);
                ret = Attack(player, defender);
                player.NumOfActions++;
                Game auxGame = Store.GetGame();
                auxGame.Players.Find(h => h.Client.Username == defender.Client.Username).HP = defender.HP;
                 auxGame.Players.Find(h => h.Client.Username == player.Client.Username).NumOfActions++;
                 auxGame.Players.Find(h => h.Client.Username == player.Client.Username).NumOfAttacks++;
                auxGame.Players.Find(h => h.Client.Username == defender.Client.Username).IsAlive = defender.IsAlive;
                Store.SetGame(auxGame);

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
            UpdatePlayerScore(player, "move");
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
            activeGame = Store.GetGame();
            activeGame.Players.Find(h => h.Client.Username == player.Client.Username).Position = board.Cells[x, y];
            Store.SetGame(activeGame);
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
            UpdatePlayerScore(attacker, "attack");
            if (!defender.IsAlive)
            {
                Game aux = Store.GetGame();
                ret.Add("KILLED");
                ret.Add(defender.Client.Username);
                aux.PlayersThatDied.Add("-"+defender.Client.Username);
                Store.SetGame(aux);
                UpdatePlayerScore(attacker, "killed");
                UpdatePlayerScore(defender, "dead");
            }
            return ret;
        }

        private void UpdatePlayerScore(Player player, string action)
        {
            activeGame = Store.GetGame();
            if(action == "move")
            {
                activeGame.Players.Find(x => x.Client.Username ==  player.Client.Username).Score += 3;
            }else if(action == "attack")
            {
                activeGame.Players.Find(x => x.Client.Username == player.Client.Username).Score += 10;
            }
            else if(action == "killed")
            {
                activeGame.Players.Find(x => x.Client.Username == player.Client.Username).Score += 20;
            }
            else if(action == "dead")
            {
                int score = activeGame.Players.Find(x => x.Client.Username == player.Client.Username).Score;
                if(score<=10) activeGame.Players.Find(x => x.Client.Username == player.Client.Username).Score = 0;
                else activeGame.Players.Find(x => x.Client.Username == player.Client.Username).Score -= 10;
            }
            Store.SetGame(activeGame);
        }

    }

}
