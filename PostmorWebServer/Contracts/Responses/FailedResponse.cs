using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Contracts.Responses
{
    public class FailedResponse
    {
        public IEnumerable<String> Errors { get; set; }
    }
}
