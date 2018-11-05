using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [DataContract]
    public class RankingCredentials 
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Score { get; set; }

        [DataMember]
        public string GameDate { get; set; }

        [DataMember]
        public string Role { get; set; }

        public override string ToString()
        {
            return $"Player: {Username} played as {Role} on {GameDate}.{Environment.NewLine}  - Score: {Score}";
        }
    }
}
