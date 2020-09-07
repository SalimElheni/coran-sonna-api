using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace ACS.Data.Access.Tables
{

    public class UserAccess
    {

        #region Default Methods
        public static ACS.Data.Entities.Tables.UserEntity Get(int id)
        {
            var dataTable = new DataTable();
            using(var sqlConnection = new MySqlConnection(Config.GetConnectionString()))
            {
                sqlConnection.Open();
                string query = "SELECT * FROM `User` WHERE `Id`=@Id";
                var sqlCommand = new MySqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("Id", id); 

                new MySqlDataAdapter(sqlCommand).Fill(dataTable);

            }

            if (dataTable.Rows.Count > 0)
            {
                return new ACS.Data.Entities.Tables.UserEntity(dataTable.Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public static List<ACS.Data.Entities.Tables.UserEntity> Get()
        {  
            var dataTable = new DataTable();     
            using(var sqlConnection = new MySqlConnection(Config.GetConnectionString()))
            {
                sqlConnection.Open();
                string query = "SELECT * FROM `User`";
                var sqlCommand = new MySqlCommand(query, sqlConnection); 

                new MySqlDataAdapter(sqlCommand).Fill(dataTable);
            }

            if (dataTable.Rows.Count > 0)
            {
                return toList(dataTable);
            }
            else
            {
                return new List<ACS.Data.Entities.Tables.UserEntity>();
            }
        }
        public static List<ACS.Data.Entities.Tables.UserEntity> Get(List<int> ids)
        {
            if(ids != null && ids.Count > 0)
            {
                int maxQueryNumber = Config.MAX_BATCH_SIZE ; 
                List<ACS.Data.Entities.Tables.UserEntity> results = null;
                if(ids.Count <= maxQueryNumber)
                {
                    results = get(ids);
                }else
                {
                    int batchNumber = ids.Count / maxQueryNumber;
                    results = new List<ACS.Data.Entities.Tables.UserEntity>();
                    for(int i=0; i<batchNumber; i++)
                    {
                        results.AddRange(get(ids.GetRange(i * maxQueryNumber, maxQueryNumber)));
                    }
                    results.AddRange(get(ids.GetRange(batchNumber * maxQueryNumber, ids.Count-batchNumber * maxQueryNumber)));
                }
                return results;
            }
            return new List<ACS.Data.Entities.Tables.UserEntity>();
        }
        private static List<ACS.Data.Entities.Tables.UserEntity> get(List<int> ids)
        {
            if(ids != null && ids.Count > 0)
            {
                var dataTable = new DataTable();
                using(var sqlConnection = new MySqlConnection(Config.GetConnectionString()))
                {
                    sqlConnection.Open();
                    var sqlCommand = new MySqlCommand();
                    sqlCommand.Connection = sqlConnection;

                    string queryIds = string.Empty;
                    for(int i=0; i<ids.Count; i++)
                    {
                        queryIds += "@Id" + i + ",";
                        sqlCommand.Parameters.AddWithValue("Id" + i, ids[i]);
                    }
                    queryIds = queryIds.TrimEnd(',');

                    sqlCommand.CommandText = "SELECT * FROM `User` WHERE `Id` IN ("+ queryIds +")";                    
                new MySqlDataAdapter(sqlCommand).Fill(dataTable);
                }

                if (dataTable.Rows.Count > 0)
                {
                    return toList(dataTable);
                }
                else
                {
                    return new List<ACS.Data.Entities.Tables.UserEntity>();
                }
            }
            return new List<ACS.Data.Entities.Tables.UserEntity>();
        }

        public static int Insert(ACS.Data.Entities.Tables.UserEntity item)
        {
            int response = int.MinValue;
            using (var sqlConnection = new MySqlConnection(Config.GetConnectionString()))
            {
                sqlConnection.Open();
                var sqlTransaction = sqlConnection.BeginTransaction();

                string query = "INSERT INTO `User` (`FirstName`,`LastName`,`Username`,`PasswordHash`,`PasswordSalt`)  VALUES (@FirstName,@LastName,@Username,@PasswordHash,@PasswordSalt)";

                using(var sqlCommand = new MySqlCommand(query, sqlConnection, sqlTransaction))
				{

					sqlCommand.Parameters.AddWithValue("FirstName",item.FirstName == null ? (object)DBNull.Value  : item.FirstName);
					sqlCommand.Parameters.AddWithValue("LastName",item.LastName == null ? (object)DBNull.Value  : item.LastName);
					sqlCommand.Parameters.AddWithValue("Username",item.Username);
					sqlCommand.Parameters.AddWithValue("PasswordHash",item.PasswordHash);
					sqlCommand.Parameters.AddWithValue("PasswordSalt",item.PasswordSalt);

                    sqlCommand.ExecuteNonQuery();
                }

                using (var sqlCommand = new MySqlCommand("SELECT `Id` FROM `User` WHERE `Id` = @@IDENTITY", sqlConnection, sqlTransaction))
                {
                    response = sqlCommand.ExecuteScalar() == null? int.MinValue:  int.TryParse(sqlCommand.ExecuteScalar().ToString(), out var insertedId) ? insertedId : int.MinValue;
                }
           
                sqlTransaction.Commit();

                return response;
            }
        }
        public static int Insert(List<ACS.Data.Entities.Tables.UserEntity> items)
        {
            if (items != null && items.Count > 0)
            {
                int maxParamsNumber = Config.MAX_BATCH_SIZE / 6; // Nb params per query
                int results=0;
                if(items.Count <= maxParamsNumber)
                {
                    results = insert(items);
                }else
                {
                    int batchNumber = items.Count / maxParamsNumber;
                    for(int i = 0; i < batchNumber; i++)
                    {
                        results += insert(items.GetRange(i * maxParamsNumber, maxParamsNumber));
                    }
                    results += insert(items.GetRange(batchNumber * maxParamsNumber, items.Count - batchNumber * maxParamsNumber));
                }
                return results;
            }

            return -1;
        }
        private static int insert(List<ACS.Data.Entities.Tables.UserEntity> items)
        {
            if (items != null && items.Count > 0)
            {
                int results = -1;
                using(var sqlConnection = new MySqlConnection(Config.GetConnectionString()))
                {
                    sqlConnection.Open();
                    string query = "";
                    var sqlCommand = new MySqlCommand(query, sqlConnection);

                    int i = 0;
                    foreach (var item in items)
                    {
                        i++;
                        query += " INSERT INTO `User` (`Id`,`FirstName`,`LastName`,`Username`,`PasswordHash`,`PasswordSalt`) VALUES ( "

							+ "@Id"+ i +","
							+ "@FirstName"+ i +","
							+ "@LastName"+ i +","
							+ "@Username"+ i +","
							+ "@PasswordHash"+ i +","
							+ "@PasswordSalt"+ i 
                            + "); ";

                            
							sqlCommand.Parameters.AddWithValue("Id" + i, item.Id);
							sqlCommand.Parameters.AddWithValue("FirstName" + i, item.FirstName == null ? (object)DBNull.Value  : item.FirstName);
							sqlCommand.Parameters.AddWithValue("LastName" + i, item.LastName == null ? (object)DBNull.Value  : item.LastName);
							sqlCommand.Parameters.AddWithValue("Username" + i, item.Username);
							sqlCommand.Parameters.AddWithValue("PasswordHash" + i, item.PasswordHash);
							sqlCommand.Parameters.AddWithValue("PasswordSalt" + i, item.PasswordSalt);
                    }

                    sqlCommand.CommandText = query;

                    results = sqlCommand.ExecuteNonQuery();
                }

                return results;
            }

            return -1;
        }

        public static int Update(ACS.Data.Entities.Tables.UserEntity item)
        {   
            int results = -1;
            using(var sqlConnection = new MySqlConnection(Config.GetConnectionString()))
            {
                sqlConnection.Open();
                string query = "UPDATE `User` SET `FirstName`=@FirstName, `LastName`=@LastName, `Username`=@Username, `PasswordHash`=@PasswordHash, `PasswordSalt`=@PasswordSalt WHERE `Id`=@Id";
                var sqlCommand = new MySqlCommand(query, sqlConnection);
                    
                sqlCommand.Parameters.AddWithValue("Id", item.Id);
				sqlCommand.Parameters.AddWithValue("FirstName",item.FirstName == null ? (object)DBNull.Value  : item.FirstName);
				sqlCommand.Parameters.AddWithValue("LastName",item.LastName == null ? (object)DBNull.Value  : item.LastName);
				sqlCommand.Parameters.AddWithValue("Username",item.Username);
				sqlCommand.Parameters.AddWithValue("PasswordHash",item.PasswordHash);
				sqlCommand.Parameters.AddWithValue("PasswordSalt",item.PasswordSalt);
                        
                results = sqlCommand.ExecuteNonQuery();
            }
                
            return results;
        }
        public static int Update(List<ACS.Data.Entities.Tables.UserEntity> items)
        {
            if (items != null && items.Count > 0)
            {
                int maxParamsNumber = Config.MAX_BATCH_SIZE / 6; // Nb params per query
                int results = 0;
                if(items.Count <= maxParamsNumber)
                {
                    results = update(items);
                }else
                {
                    int batchNumber = items.Count / maxParamsNumber;
                    for(int i = 0; i < batchNumber; i++)
                    {
                        results += update(items.GetRange(i * maxParamsNumber, maxParamsNumber));
                    }
                    results += update(items.GetRange(batchNumber * maxParamsNumber, items.Count - batchNumber * maxParamsNumber));
                }

                return results;
            }

            return -1;
        }
        private static int update(List<ACS.Data.Entities.Tables.UserEntity> items)
        {
            if (items != null && items.Count > 0)
            {
                int results = -1;
                using(var sqlConnection = new MySqlConnection(Config.GetConnectionString()))
                {
                    sqlConnection.Open();
                    string query = "";
                    var sqlCommand = new MySqlCommand(query, sqlConnection);

                    int i = 0;
                    foreach (var item in items)
                    {
                        i++;
                        query += " UPDATE `User` SET "

							+ "`FirstName`=@FirstName"+ i +","
							+ "`LastName`=@LastName"+ i +","
							+ "`Username`=@Username"+ i +","
							+ "`PasswordHash`=@PasswordHash"+ i +","
							+ "`PasswordSalt`=@PasswordSalt"+ i +" WHERE `Id`=@Id" + i 
                            + "; ";

                            sqlCommand.Parameters.AddWithValue("Id" + i, item.Id);
							sqlCommand.Parameters.AddWithValue("FirstName" + i, item.FirstName == null ? (object)DBNull.Value  : item.FirstName);
							sqlCommand.Parameters.AddWithValue("LastName" + i, item.LastName == null ? (object)DBNull.Value  : item.LastName);
							sqlCommand.Parameters.AddWithValue("Username" + i, item.Username);
							sqlCommand.Parameters.AddWithValue("PasswordHash" + i, item.PasswordHash);
							sqlCommand.Parameters.AddWithValue("PasswordSalt" + i, item.PasswordSalt);
                    }

                    sqlCommand.CommandText = query;

                    results = sqlCommand.ExecuteNonQuery();
                }

                return results;
            }

            return -1;
        }

        public static int Delete(int id)
        {
            int results = -1;
            using(var sqlConnection = new MySqlConnection(Config.GetConnectionString()))
            {
                sqlConnection.Open();
                string query = "DELETE FROM `User` WHERE `Id`=@Id";
                var sqlCommand = new MySqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("Id", id);

                results = sqlCommand.ExecuteNonQuery();
            }

            return results;
        }
        public static int Delete(List<int> ids)
        {
            if(ids != null && ids.Count > 0)
            {
                int maxParamsNumber = Config.MAX_BATCH_SIZE; 
                int results=0;
                if(ids.Count <= maxParamsNumber)
                {
                    results = delete(ids);
                } else
                {
                    int batchNumber = ids.Count / maxParamsNumber;
                    for(int i = 0; i < batchNumber; i++)
                    {
                        results += delete(ids.GetRange(i * maxParamsNumber, maxParamsNumber));
                    }
                    results += delete(ids.GetRange(batchNumber * maxParamsNumber, ids.Count - batchNumber * maxParamsNumber));
                }
            }
            return -1;
        }
        private static int delete(List<int> ids)
        {
            if(ids != null && ids.Count > 0)
            {
                int results = -1;
                using(var sqlConnection = new MySqlConnection(Config.GetConnectionString()))
                {
                    sqlConnection.Open();
                    var sqlCommand = new MySqlCommand();
                    sqlCommand.Connection = sqlConnection;

                    string queryIds = string.Empty;
                    for(int i=0; i<ids.Count; i++)
                    {
                        queryIds += "@Id" + i + ",";
                        sqlCommand.Parameters.AddWithValue("Id" + i, ids[i]);
                    }
                    queryIds = queryIds.TrimEnd(',');

                    string query = "DELETE FROM `User` WHERE `Id` IN ("+ queryIds +")";                    
                    sqlCommand.CommandText = query;
                        
                    results = sqlCommand.ExecuteNonQuery();
                }

                return results;
            }
            return -1;
        }
        #endregion

        #region Custom Methods
        public static ACS.Data.Entities.Tables.UserEntity GetByUsernamePassword(string username, byte[] passwordHash, byte[] passwordSalt)
        {
            var dataTable = new DataTable();
            using (var sqlConnection = new MySqlConnection(Config.GetConnectionString()))
            {
                sqlConnection.Open();
                string query = "SELECT * FROM `User` WHERE `Username`=@username AND `PasswordHash`=@passwordHash AND `PasswordSalt`=@passwordSalt";
                var sqlCommand = new MySqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("username", username);
                sqlCommand.Parameters.AddWithValue("passwordHash", passwordHash);
                sqlCommand.Parameters.AddWithValue("passwordSalt", passwordSalt);

                new MySqlDataAdapter(sqlCommand).Fill(dataTable);
            }

            if (dataTable.Rows.Count > 0)
            {
                return new ACS.Data.Entities.Tables.UserEntity(dataTable.Rows[0]);
            }
            else
            {
                return null;
            }
        }
        public static ACS.Data.Entities.Tables.UserEntity GetByUsername(string username)
        {
            var dataTable = new DataTable();
            using (var sqlConnection = new MySqlConnection(Config.GetConnectionString()))
            {
                sqlConnection.Open();
                string query = "SELECT * FROM `User` WHERE `Username`=@username";
                var sqlCommand = new MySqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("username", username);

                new MySqlDataAdapter(sqlCommand).Fill(dataTable);
            }

            if (dataTable.Rows.Count > 0)
            {
                return new ACS.Data.Entities.Tables.UserEntity(dataTable.Rows[0]);
            }
            else
            {
                return null;
            }
        }
        public static ACS.Data.Entities.Tables.UserEntity GetByUsernameUuid(string username, string uuid)
        {
            var dataTable = new DataTable();
            using (var sqlConnection = new MySqlConnection(Config.GetConnectionString()))
            {
                sqlConnection.Open();
                string query = "SELECT * FROM `User` WHERE `Username`=@username AND `uuid`=@uuid";
                var sqlCommand = new MySqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("uuid", uuid);
                sqlCommand.Parameters.AddWithValue("username", username);

                new MySqlDataAdapter(sqlCommand).Fill(dataTable);
            }

            if (dataTable.Rows.Count > 0)
            {
                return new ACS.Data.Entities.Tables.UserEntity(dataTable.Rows[0]);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Helpers

        private static List<ACS.Data.Entities.Tables.UserEntity> toList(DataTable dataTable)
        {
            var list = new List<ACS.Data.Entities.Tables.UserEntity>(dataTable.Rows.Count);
            foreach (DataRow dataRow in dataTable.Rows)
            { list.Add(new ACS.Data.Entities.Tables.UserEntity(dataRow)); }
            return list;
        }
        #endregion
    }
}
