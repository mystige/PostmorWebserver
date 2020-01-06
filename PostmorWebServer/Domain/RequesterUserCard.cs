using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Domain
{
    public class RequesterUserCard
    {
        public int RequesterId { get; set; }
        public string RequesterName { get; set; }
        public string RequesterAddress { get; set; }
        public string RequesterPublicKey { get; set; }
        public string RequesterPrivateKey { get; set; }
        public string RequesterPickupTime { get; set; }
        public string RequesterDeliveryTime { get; set; }
    }
}
