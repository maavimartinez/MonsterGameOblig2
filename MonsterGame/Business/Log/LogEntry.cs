using System;

namespace Business
{
    [Serializable]
    public class LogEntry
    {
        public string Text { get; set; }

        public LogEntry() { }
        public LogEntry(LogEntryAttributes logEntryAttributes)
        {
            Text = logEntryAttributes.ToString();
        }

        public override bool Equals(object obj)
        {
            LogEntry toCompare = obj as LogEntry;
            if (toCompare == null)
                return false;
            return toCompare.Text.Equals(Text);
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
