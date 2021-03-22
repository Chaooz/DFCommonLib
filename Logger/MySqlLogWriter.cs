﻿using System;
using DFCommonLib.DataAccess;
using DFCommonLib.Utils;

namespace DFCommonLib.Logger
{
    public class MySqlLogWriter : ILogOutputWriter
    {
        IDbConnectionFactory _connection;

        static int MESSAGE_LENGTH = 1024;

        public MySqlLogWriter(IDbConnectionFactory connection)
        {
            _connection = connection;
        }
        public string GetName()
        {
            return "MySqlLogWriter";
        }

        public void CreateTable(IDBPatcher patcher)
        {
            patcher.Patch("DbPatcher", 1, "CREATE TABLE `logtable` ( `id` int(11) NOT NULL AUTO_INCREMENT, `created` datetime NOT NULL, `loglevel` int(11) NOT NULL, `groupname` varchar(100) NOT NULL DEFAULT '', `message` varchar(1024) NOT NULL DEFAULT '', PRIMARY KEY (`id`))");
        }

        public void LogMessage(DFLogLevel logLevel, string group, string message)
        {
            var sql = @"insert into logtable (id,created, loglevel, groupname, message) values(0,sysdate(), @loglevel,@group,@message)";
            using (var command = _connection.CreateCommand(sql))
            {
                command.AddParameter("@loglevel", (int) logLevel);
                command.AddParameter("@group", group);
                command.AddParameter("@message", DFCommonUtil.CapString(message, MESSAGE_LENGTH) );
                command.ExecuteNonQuery();
            }
        }
    }
}
