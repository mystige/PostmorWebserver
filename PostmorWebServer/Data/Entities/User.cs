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

        public string Streetnumber { get; set; }
        [Required]
        public string PrivateKey { get; set; }
        [Required]
        public string PublicKey { get; set; }
        [Required]
        public string PickupTime { get; set; }
        [Required]
        public string SendTime { get; set; }
        [Required]
        public bool ActiveUser { get; set; }
        public string ProfilePic { get; set; }
        public List<UserContact> Contacts { get; set; }
        public List<UserContact> ContactOf { get; set; }
        public List<Letter> Letters { get; set; }

    }
}
