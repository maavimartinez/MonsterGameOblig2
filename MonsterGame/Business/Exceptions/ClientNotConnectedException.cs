﻿using System;

namespace Business.Exceptions
{
    public class ClientNotConnectedException : BusinessException
    {
        public ClientNotConnectedException() : base("Client not connected")
        {
        }

        public ClientNotConnectedException(string msg) : base(msg)
        {

        }
    }
}