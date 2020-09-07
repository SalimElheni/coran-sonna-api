using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACS.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        //
        public string Token { get; set; }
        public UserModel()
        {

        }

        public UserModel(ACS.Data.Entities.Tables.UserEntity userEntity)
        {
            if(userEntity != null)
            {
                Id = userEntity.Id;
                FirstName = userEntity.FirstName;
                LastName = userEntity.LastName;
                Username = userEntity.Username;
            }
        }
        public ACS.Data.Entities.Tables.UserEntity ToEntity()
        {
            return new Data.Entities.Tables.UserEntity
            {
                Id = Id,
                FirstName = FirstName,
                LastName = LastName,
                Username = Username
            };
        }
    }
}

