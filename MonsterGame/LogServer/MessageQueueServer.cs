using System;
using System.Messaging;
using Business;

namespace LogServer
{
    internal class MessageQueueServer
    {
        private bool isServerRunning;
        private GameLogic gameLogic;

        public delegate void NewEntryEventHandler();

        public event NewEntryEventHandler NewEntry;

        public MessageQueueServer(GameLogic gameLogic)
        {
            isServerRunning = true;
            QueuePath = QueueSettings.QueueCreationPath();
            this.gameLogic = gameLogic;
        }

        protected virtual void OnNewEntry(EventArgs e)
        {
            NewEntry?.Invoke();
        }

        public string QueuePath { get; }

        public void Start()
        {
            if (!MessageQueue.Exists(QueuePath)) MessageQueue.Create(QueuePath);

            var messageQueue = new MessageQueue(QueuePath)
            {
                Formatter = new XmlMessageFormatter(new[] { typeof(LogEntry) })
            };
            while (isServerRunning)
            {
                LogEntry entry = null;

                try
                {
                    System.Messaging.Message message = messageQueue.Receive();
                    entry = message?.Body as LogEntry;
                }
                catch (MessageQueueException) { }

                if (IsValidEntry(entry))
                {
                    gameLogic.AddLogEntry(entry);
                    OnNewEntry(new EventArgs());
                }
            }
        }

        public void Stop()
        {
            isServerRunning = false;
        }

        bool IsValidEntry(LogEntry entry)
        {
            return entry?.Text != null;
        }
    }
}
