using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Serializable]
    public class StatisticDTO
    {
        public string GameDate { get; set; }
        public List<StatisticDetail> gameStatistic { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("- Game:{0}\n ", GameDate);
            foreach (StatisticDetail sd in gameStatistic)
            {
                sb.AppendFormat("\t> Player: {0}\n \t -Role: {1}\n \t -Outcome: {2}\n", sd.Username, sd.Role, sd.Outcome);
            }
            return sb.ToString();
        }
        public override bool Equals(object obj)
        {
            var item = obj as StatisticDTO;

            if (item == null)
            {
                return false;
            }

            bool containsInList = true;
            foreach (StatisticDetail sd in this.gameStatistic)
            {
                if (!item.gameStatistic.Contains(sd))
                    containsInList = false;
            }
            return this.GameDate.Equals(item.GameDate)&&containsInList;
        }
    }
}
