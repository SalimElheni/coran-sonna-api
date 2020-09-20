using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ACS.Models
{
    public class AuthenticateModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class AuthModel
    {
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
