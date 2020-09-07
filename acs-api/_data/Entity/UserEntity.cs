using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ACS.Data.Entities.Tables
{
    public class UserEntity
    {
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Username { get; set; }
		public byte[] PasswordHash { get; set; }
		public byte[] PasswordSalt { get; set; }
		public string UUID { get; set; }
		public int Validated { get; set; }
		public int Blocked { get; set; }

		public UserEntity() { }

        public UserEntity(DataRow dataRow)
        {
			Id = Convert.ToInt32(dataRow["Id"]);
			FirstName = (dataRow["FirstName"] == System.DBNull.Value) ? "" : Convert.ToString(dataRow["FirstName"]);
			LastName = (dataRow["LastName"] == System.DBNull.Value) ? "" : Convert.ToString(dataRow["LastName"]);
			Username = Convert.ToString(dataRow["Username"]);
			PasswordHash = (byte[])dataRow["PasswordHash"];
			PasswordSalt = (byte[])dataRow["PasswordSalt"];
			UUID = (dataRow["UUID"] == System.DBNull.Value) ? "" : Convert.ToString(dataRow["UUID"]);
			Validated = (dataRow["Validated"] == System.DBNull.Value) ? 0 : Convert.ToInt32(dataRow["Validated"]);
			Blocked = (dataRow["Blocked"] == System.DBNull.Value) ? 0 : Convert.ToInt32(dataRow["Blocked"]);
		}
    }
}

