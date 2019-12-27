using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PostmorWebServer
{
    public class User : IdentityUser<int>
    {
        [Required]
        public string Adress { get; set; }
        [Required]
        public string PrivateKey { get; set; }
        [Required]
        public string PublicKey { get; set; }
        [Required]
        public DateTime PickupTime { get; set; }
        [Required]
        public DateTime SendTime { get; set; }
        public bool ActiveUser { get; set; }
        public string ProfilePic { get; set; }
        public List<User> Contacts { get; set; }

        public string Token { get; set; }


    }
}
