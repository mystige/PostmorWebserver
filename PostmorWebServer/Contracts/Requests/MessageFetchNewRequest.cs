﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Contracts.Requests
{
    public class MessageFetchNewRequest
    {
        public int LatestMessageId { get; set; }
    }
}
