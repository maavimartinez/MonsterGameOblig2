using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Serializable]
    public class StatisticDetail
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public string Outcome { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as StatisticDetail;

            if (item == null)
            {
                return false;
            }

            return this.Username.Equals(item.Username) && this.Outcome.Equals(item.Outcome) && this.Role.Equals(item.Role);
        }
    }
}
