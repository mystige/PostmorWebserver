﻿using PostmorWebServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Contracts.Responses
{
    public class MessageFetchNewResponse
    {
        public List<Message> Messages { get; set; }
    }
}
