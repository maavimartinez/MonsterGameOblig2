using System;

namespace Business.Log
{
    public class ResultEntry : LogEntryAttributes
    {
        public string Result { get; set; }

        public string Players { get; set; }

        public override string ToString()
        {
            return $"Last game ended at time : {Timestamp}.The result was: {Result}. These were the original players: {Players}";
        }
    }
}
