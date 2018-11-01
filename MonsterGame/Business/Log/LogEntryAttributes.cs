using System;

namespace Business
{
    public abstract class LogEntryAttributes
    {
        public string ClientUsername { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
