using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Domain
{
    public class AuthenticationResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Succes { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public int UserID { get; set; }
    }
}
