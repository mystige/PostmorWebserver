using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Contracts.Responses
{
    public class RegisterSuccessRespones
    {
        public int Id { get; set; }
        public string PickupTime { get; set; }
        public string DeliveryTime { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }

    }
}
