using System;
using System.ServiceModel;

namespace WebServices
{
    public class WCFHost
    {
        private bool hostServiceIsRunning = false;

        public void Start()
        {
            try
            {
                using (var serviceHost = new ServiceHost(typeof(CRUDClientService)))
                {
                    serviceHost.Open();
                    hostServiceIsRunning = true;
                    while (hostServiceIsRunning) { }
                    serviceHost.Close();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("There was a problem opening the service host");
            }
        }

        public void Stop()
        {
            hostServiceIsRunning = false;
        }
    }
}