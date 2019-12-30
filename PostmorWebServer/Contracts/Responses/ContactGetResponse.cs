using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Contracts.Responses
{
    public class ContactGet
    {
        public int ContactId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Picture { get; set; }
        public bool IsFriend { get; set; }
        public string PublicKey { get; set; }
    }
}
