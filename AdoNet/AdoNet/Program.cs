using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace AdoNet
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();

                command.CommandText = "insert into gruppa" + "(id,lastname)" + "values" + "(@id,@lastname)";

                SqlParameter sqlParameter = new SqlParameter();

                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = 10,
                    IsNullable = false
                });

                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@lastname",
                    SqlDbType = System.Data.SqlDbType.NVarChar,
                    Value = "Olzhas",
                });
                command.Connection = connection;


                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch(SqlException exception)
                    {
                        Console.WriteLine(exception.Message);
                        transaction.Rollback();
                    }
                }
            }

        }

        public static void ExecuteTransaction(SqlConnection sqlConnection, params SqlCommand[] commands)
        {
            using (var transaction = sqlConnection.BeginTransaction())
            {
                try
                {
                    foreach(var command in commands)
                    {
                        command.Transaction = transaction;
                    }

                    foreach(var command in commands)
                    {
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (SqlException exception)
                {
                    Console.WriteLine(exception.Message);
                    transaction.Rollback();
                }
            }
        }
    }
}
