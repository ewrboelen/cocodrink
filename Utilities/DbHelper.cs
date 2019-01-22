using System;
using Cocodrinks.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data.SqlClient;
using System.Data.Common;


namespace Cocodrinks.Utilities
{
    public class DbHelper
    {
        internal static Boolean finduser(CocodrinksContext context, string name)
        {
           using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT Id FROM user WHERE name='"+name+"'";
                context.Database.OpenConnection();
                DbDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read()) {
                        Console.WriteLine("found userid {0}\t.", reader.GetInt32(0));
                    }
                    reader.Close();
                    return true;
                }
                else{
                    Console.WriteLine("No users found.");
                }
                reader.Close();

                return false;

            }
        }
    }
}