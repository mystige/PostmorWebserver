using PostmorWebServer.Domain;
using PostmorWebServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Contracts.Responses
{
    public class UserFetchAllDataResponse
    {
        public List<UserCard> Contacts { get; set; }
        public List<Message> Messages { get; set; }
        public RequesterUserCard Userdata { get; set; }
    }
}
