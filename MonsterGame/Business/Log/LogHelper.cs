using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Log
{
   public static class LogHelper
    {
      public static string ExtractResult(List<string> result)
        {
            string ret = "";
            foreach (string s in result)
            {
                if (!s.Contains("Player:"))
                    ret += s + Environment.NewLine;
            }
            return ret;
        }

       public static string ExtractPlayers(List<string> result)
        {
            string players = "";
            foreach (string s in result)
            {
                if (s.Contains("Player:"))
                {
                    if (!players.Equals(""))
                        players += ", ";
                    players += (s.Split(':').Last());
                }
            }
            return players;
        }
    }
}
