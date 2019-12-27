using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Data.Entities
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }
        public string JwtID { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime Expirydate { get; set; }
        public bool Used { get; set; }
        public bool Invaildated { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
