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
            //en FetchAllData
        }
        public static class Contacts
        {
            public const string Add = "/contact/add";
            public const string Remove = "/contact/remove";
            public const string Search = "/contact/search";
            public const string Get = "/contact/get";

        }
        public static class Messages
        {
            public const string Send = "/message/send";
            public const string FetchNew = "/message/fetch/new";
        }
    }
}
