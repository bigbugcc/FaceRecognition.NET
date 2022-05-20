using Microsoft.Data.Sqlite;
using SqlSugar;
using System;
using System.IO;

namespace FaceRecognition.NET.DbContext
{
    public class DbContext
    {
        public SqlSugarClient Client { get; }

        private static string dbName = Path.Combine(Environment.CurrentDirectory, "FaceGo.sqlite");

        private static string connStr = new SqliteConnectionStringBuilder()
        {
            DataSource = dbName,
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString();

        public DbContext()
        {
            Client = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = connStr,
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });
        }
    }
}
