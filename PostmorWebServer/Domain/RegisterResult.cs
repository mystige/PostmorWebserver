using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Domain
{
    public class RegisterResult
    {
        public bool Succes { get; set; }
        public string PickupTime { get; set; }
        public string DeliveryTime { get; set; }
        public string PubliciKey { get; set; }
        public string PrivateKey { get; set; }
        public IEnumerable<string> Error { get; set; }
    }
}
