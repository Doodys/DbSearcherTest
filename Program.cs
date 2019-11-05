using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DbSearcher
{
    class Program
    {
        static void Main(string[] args)
        {
            getData();
            Console.ReadKey();
        }
        static void getData()
        {
            string sTableName = "";
            string sColumnName = "";

            Console.Write("Table: ");
            sTableName = Console.ReadLine();
            Console.Write("Column: ");
            sColumnName = Console.ReadLine();

            Console.Clear();
            string[] tab2 = getColumnsName(sTableName, sColumnName);
            foreach (var column in tab2)
            {
                Console.WriteLine(column);
            }
        }
        static string[] getColumnsName(string tableName, string columnName)
        {
            string sConnString = @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFileName=C:\Users\dudys\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\dbsearch.mdf;Initial Catalog=dbsearch;";
            List<string> listacolumnas = new List<string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(sConnString))
                using (SqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    if (tableName == "*")
                    {
                        string[] arr = GetAllTables(connection);
                        foreach (var tabName in arr)
                        {
                            command.CommandText = "select c.name from sys.columns c inner join sys.tables t on t.object_id = c.object_id and t.name = '" + tabName + "' and t.type = 'U'";
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader.GetValue(0).ToString().Contains(columnName))
                                    {
                                        listacolumnas.Add(tabName + "." + reader.GetValue(0).ToString());
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        command.CommandText = "select c.name from sys.columns c inner join sys.tables t on t.object_id = c.object_id and t.name = '" + tableName + "' and t.type = 'U'";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader.GetValue(0).ToString().Contains(columnName))
                                {
                                    listacolumnas.Add(tableName + "." + reader.GetValue(0).ToString());
                                }
                            }
                        }
                    }
                }

                if (!listacolumnas.Any()) throw new Exception();
                else return listacolumnas.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("No data!");
                Console.ReadKey();
                Console.Clear();
                getData();
            }

            return listacolumnas.ToArray();
        }

        static string[] GetAllTables(SqlConnection connection)
        {
            List<string> result = new List<string>();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT name FROM sys.Tables";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        result.Add(reader["name"].ToString());
                }
            }
            
            return result.ToArray();
        }
    }
}
