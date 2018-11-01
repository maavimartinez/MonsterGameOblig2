using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Serializable]
    public class RankingItem
    {
        public string Username { get; set; }
        public int Score { get; set; }
        public DateTime GameDate { get; set; }
        public Type Role { get; set; }
    }

}
