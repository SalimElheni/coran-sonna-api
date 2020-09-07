using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACS.Data.Access
{
    public class Config
    {
        public readonly static int MAX_BATCH_SIZE = 1000;
        public static string GetConnectionString()
        {
            return "Server=localhost;Database=acs;Uid=acs_user_db;Pwd=acs_pawd_db";
        }
    }
}
