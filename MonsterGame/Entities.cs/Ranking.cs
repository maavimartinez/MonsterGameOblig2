using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Serializable]
  public  class Ranking
    {
        public string Username { get; set; }
        public int Score { get; set; }
        public string GameDate { get; set; }
        public Type Role { get; set; }

        public override string ToString()
        {
            return $"Player: {Username} played as {Role} on {GameDate}.{Environment.NewLine}  - Score: {Score}";
        }
    }
}
