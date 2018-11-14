using System.Configuration;

namespace Business
{
    public class Settings
    {
        public static string GetServerIpFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (string)appSettings.GetValue("ServerIp", typeof(string));
        }

        public static string GetLogServerIpFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (string)appSettings.GetValue("LogServerIp", typeof(string));
        }

        public static string GetStorageServerIpFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (string)appSettings.GetValue("StorageServerIp", typeof(string));
        }

        public static int GetStorageServerPortFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (int)appSettings.GetValue("StorageServerPort", typeof(int));
        }
    }
}
