﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace UserInfoBot
{
    class DBUtils
    {
        public static MySqlConnection GetDBConnection()
        {
            string host = "localhost";
            int port = 3306;
            string database = "user";
            string username = "root";
            string password = "dwdwfefefe";

            return DBMySQLUtils.GetDBConnection(host, port, database, username, password);
        }

    }
}