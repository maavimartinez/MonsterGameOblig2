using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Serializable]
    public class RankingCredentials 
    {
        public string Username { get; set; }

        public string Score { get; set; }

        public string GameDate { get; set; }

        public string Role { get; set; }

        public override string ToString()
        {
            return $"Player: {Username} played as {Role.ToString().Split('.').Last()} on {GameDate}.{Environment.NewLine}  - Score: {Score}";
        }
    }
}
