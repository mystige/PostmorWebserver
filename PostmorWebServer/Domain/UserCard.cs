using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Domain
{
    public class UserCard
    {
        public bool Exists { get; set; }
        public string Error{ get; set; }
        public int ContactId { get; set; }
        public string ContactName { get; set; }
        public string Picture { get; set; }
        public bool IsFriend { get; set; }
        public string PublicKey { get; set; }
    }

}
