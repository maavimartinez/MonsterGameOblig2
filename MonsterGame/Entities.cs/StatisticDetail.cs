﻿using System;
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
    }
}
