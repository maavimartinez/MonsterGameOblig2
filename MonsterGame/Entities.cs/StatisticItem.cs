using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Serializable]
    public class StatisticItem
    {
        public List<StatisticDetail> gameStatistic { get; set; }
    }
}
