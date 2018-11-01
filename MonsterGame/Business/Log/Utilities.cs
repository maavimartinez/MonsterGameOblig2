using System.Configuration;

namespace Business
{
    public class Utillities
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

        public static string GetStoreServerIpFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (string)appSettings.GetValue("StoreServerIp", typeof(string));
        }

        public static int GetStoreServerPortFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (int)appSettings.GetValue("StoreServerPort", typeof(int));
        }
    }
}
