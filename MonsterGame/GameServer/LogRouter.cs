﻿using System;
using Business;
using Business.Log;
using System.Collections.Generic;

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
                    Result = PrepareResult(result),
                    Timestamp = DateTime.Now
                }
            );

            logger.LogAction(resultEntry);
        }

        private string PrepareResult(List<string> result)
        {
            string ret = "";
            foreach (string s in result)
            {
                ret += s + Environment.NewLine;
            }
            return ret;
        }

    }
}
