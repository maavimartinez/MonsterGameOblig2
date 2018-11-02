using System;
using Business;
using Business.Log;
using System.Collections.Generic;
using System.Linq;

namespace GameServer
{
    public class LogRouter
    {
        private Logger logger;
        public LogRouter()
        {
            string logServerIp = Utillities.GetLogServerIpFromConfigFile();
            logger = new Logger(logServerIp);
        }

        internal void LogResult(List<string> result)
        {
            var resultEntry = new LogEntry
            (
                new ResultEntry()
                {
                    Result = LogHelper.ExtractResult(result),
                    Players = LogHelper.ExtractPlayers(result),
                    Timestamp = DateTime.Now
                }
            );

            logger.LogAction(resultEntry);
        }

    }
}
