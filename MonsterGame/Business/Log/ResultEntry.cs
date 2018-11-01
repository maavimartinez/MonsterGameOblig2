using System;

namespace Business.Log
{
    public class ResultEntry : LogEntryAttributes
    {

        public override string ToString()
        {
            return $"{Timestamp}: Last Game's result was {Result}.";
        }
    }
}
