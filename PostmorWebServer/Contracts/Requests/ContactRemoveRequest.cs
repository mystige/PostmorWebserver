﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Contracts.Requests
{
    public class ContactRemoveRequest
    {
        public string Token { get; set; }
        public int ContactId { get; set; }
    }
}
