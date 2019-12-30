﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Contracts.Requests
{
    public class MessageSendRequest
    {
        public string[] Message { get; set; }
        public string Token { get; set; }
        public string Type { get; set; }
    }
}