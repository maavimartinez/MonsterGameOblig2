using System.Messaging;

namespace Business
{
    public class Logger
    {
        private string serverIp;

        public Logger(string serverIp)
        {
            this.serverIp = serverIp;
        }

        public void LogAction(LogEntry entry)
        {
            string queuePath = QueueSettings.Path(serverIp);

            using (var messageQueue = new MessageQueue(queuePath))
            {
                var message = new System.Messaging.Message(entry);

                messageQueue.Send(message);
            }
        }
    }
}
