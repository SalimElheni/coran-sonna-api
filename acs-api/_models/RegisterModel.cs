using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ACS.Models
{
    public class RegisterModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public RegisterModel()
        {

        }
        public RegisterModel(ACS.Data.Entities.Tables.UserEntity userEntity)
        {
            if (userEntity != null)
            {
                FirstName = userEntity.FirstName;
                LastName = userEntity.LastName;
                Username = userEntity.Username;
            }
        }
        public ACS.Data.Entities.Tables.UserEntity ToEntity()
        {
            return new Data.Entities.Tables.UserEntity
            {
                FirstName = FirstName,
                LastName = LastName,
                Username = Username
            };
        }

    }
}

