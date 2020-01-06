using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Domain
{
    public class AddOrRemoveContactResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
