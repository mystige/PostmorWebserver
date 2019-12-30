using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Contracts
{
    public class ApiRoutes
    {
        public static class Identity
        {
            public const string Login = "/identity/login";
            public const string Register = "/identity/register";
            public const string Refresh = "/identity/refresh";
            public const string GenerateAdresses = "/identity/generateaddresses";
        }
    }
}
