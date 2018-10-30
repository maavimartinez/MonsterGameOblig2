﻿using System;
using System.Net;
using System.Net.Sockets;

namespace Protocol
{

    public class ServerProtocol
    {

        public Socket Socket { get; set; }

        public void Start(string ip, int port)
        {
            try
            {
                var serverIpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                Socket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                Socket.Bind(serverIpEndPoint);
                Socket.Listen(100);
            }catch(SocketException e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }

}