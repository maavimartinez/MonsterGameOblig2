﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [DataContract]
    public class RankingItem 
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public int Score { get; set; }

        [DataMember]
        public string GameDate { get; set; }

        [DataMember]
        public Type Role { get; set; }

        public override string ToString()
        {
            return $"Player: {Username} played as {Role} on .{Environment.NewLine}  - Score: {Score}";
        }
    }
}
