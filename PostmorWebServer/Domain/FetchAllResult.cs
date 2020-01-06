using PostmorWebServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Domain
{
    public class FetchAllResult
    {
        public List<UserCard> Contacts { get; set; }
        public List<Message> Messages { get; set; }
        public RequesterUserCard RequesterUserCard { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
