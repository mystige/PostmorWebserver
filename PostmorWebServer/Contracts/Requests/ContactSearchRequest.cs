using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Contracts.Requests
{
    public class ContactSearchRequest
    {
        public String Token{ get; set; }
        public String Adress { get; set; }
    }
}
