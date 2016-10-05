using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace ADO.Query.Core.Test.Data
{
    public static class LocalDb
    {
        public const string DbDirectory = "Data";

        public static void CreateLocalDb(string databaseName, string scriptName, bool deleteIfExists = false)
        {
            var appDirectory = Directory.GetCurrentDirectory();
            string path = Uri.UnescapeDataString(appDirectory);

            string outputFolder = Path.Combine(appDirectory, "Database");
            string mdfFilename = databaseName + ".mdf";
            string databaseFileName = Path.Combine(outputFolder, mdfFilename);

            // Create Data Directory If It Doesn't Already Exist.
            if (!Directory.Exists(outputFolder))
            {
                throw new Exception("Database path not found");
            }

            if (CheckDatabaseExists(databaseName) && deleteIfExists)
            {
                DropDatabaseObjects(databaseName);
            }
            else if (!CheckDatabaseExists(databaseName))
            {
                // If the database does not already exist, create it.
                CreateDatabase(databaseName, databaseFileName);
            }
            if (deleteIfExists)
            {
                ExecuteScript(databaseName, Path.Combine(outputFolder, scriptName));
            }
        }

        private static void ExecuteScript(string databaseName, string scriptFullPath)
        {
            string connectionString = string.Format(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog={0};Integrated Security=True", databaseName);

            var file = new FileInfo(scriptFullPath);
            string script = file.OpenText().ReadToEnd();

            string[] commands = script.Split(new[] { "GO\r\n", "GO ", "GO\t" }, StringSplitOptions.RemoveEmptyEntries);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (string c in commands)
                {
                    var command = new SqlCommand(c, connection);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static bool CheckDatabaseExists(string databaseName)
        {
            string connectionString = string.Format(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connection Timeout=300");
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();

                cmd.CommandText = string.Format("SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = '{0}' OR name = '{1}')", databaseName, databaseName);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static void DropDatabaseObjects(string databaseName)
        {
            try
            {
                string connectionString = $@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog={databaseName};Integrated Security=True;Connection Timeout=300";

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var commandSetSingle = new SqlCommand(@"declare @ord int, @cmd varchar(8000)
 
                    declare objs cursor for
                    select 0, 'drop trigger [' + name + '] on database' from sys.triggers
                    where parent_class = 0 and is_ms_shipped = 0
                    union
                    select 1, 'drop synonym [' + schema_name(schema_id) + '].[' + name + ']' from sys.objects o
                    where o.type = 'SN'
                    union
                    select 2, 'drop procedure [' + schema_name(schema_id) + '].[' + name + ']' from sys.objects o
                    where o.type = 'P'
                    union
                    select 3, 'drop view [' + schema_name(schema_id) + '].[' + name + ']' from sys.objects o
                    where o.type = 'V'
                    union
                    select 4, 'drop function [' + schema_name(schema_id) + '].[' + name + ']' from sys.objects o
                    where o.type IN ('FN','IF', 'TF')
                    union
                    select 5, 'alter table [' + schema_name(schema_id) + '].[' + object_name(parent_object_id) + '] drop constraint [' + name + ']'
                    from sys.objects
                    where type = 'F'
                    union
                    select 6, 'drop table [' + schema_name(schema_id) + '].[' + name + ']' from sys.objects o
                    where o.type = 'U'
                    union
                    select 7, 'drop type [' + schema_name(schema_id) + '].[' + name + ']' from sys.types
                    where is_user_defined = 1
                    union
                    select 8, 'drop default [' + schema_name(schema_id) + '].[' + name + ']' from sys.objects o
                    where o.type = 'D'
                    order by 1
 
                    open objs
                    fetch next from objs into @ord, @cmd
 
                    while @@FETCH_STATUS = 0
                    begin
                      print @cmd
                      execute (@cmd)
                      fetch next from objs into @ord, @cmd
                    end
 
                    close objs
                    deallocate objs", connection);

                    commandSetSingle.ExecuteNonQuery();
                }
            }
            catch
            {
            }
        }

        private static void CreateDatabase(string databaseName, string databaseFileName)
        {
            string connectionString = string.Format(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True");
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();

                // TryDetachDatabase(databaseName);
                cmd.CommandText = string.Format("CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}')", databaseName, databaseFileName);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
