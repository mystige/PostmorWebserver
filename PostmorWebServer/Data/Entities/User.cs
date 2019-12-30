using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PostmorWebServer.Data.Entities
{
    public class User : IdentityUser<int>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string PrivateKey { get; set; }
        [Required]
        public string PublicKey { get; set; }
        [Required]
        public string PickupTime { get; set; }
        [Required]
        public string SendTime { get; set; }
        public bool ActiveUser { get; set; }
        public string ProfilePic { get; set; }
        //public List<User> Contacts { get; set; }
       
    }
}
