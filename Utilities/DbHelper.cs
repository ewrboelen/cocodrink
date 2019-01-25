using System;
using Cocodrinks.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data.SqlClient;
using System.Data.Common;
using Microsoft.Extensions.Logging;

namespace Cocodrinks.Utilities
{
    public class DbHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DbHelper));
        internal static Boolean finduser(CocodrinksContext context, string name)
        {
           using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT Id FROM users WHERE name='"+name+"'";
                context.Database.OpenConnection();
                DbDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read()) {
                        log.Info("found userid "+ reader.GetInt32(0));
                    }
                    reader.Close();
                    return true;
                }
                else{
                    log.Info("no users found");
                }
                reader.Close();

                return false;

            }
        }

         internal static Int32 findUserId(CocodrinksContext context, string name)
        {
           Int32 userid = -1;
           using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT Id FROM users WHERE name='"+name+"'";
                context.Database.OpenConnection();
                DbDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read()) {
                        userid = reader.GetInt32(0);
                        log.Info("found userid "+ userid.ToString());
                    }
                    reader.Close();
                    
                }
                else{
                    log.Info("no users found");
                }
                reader.Close();

                return userid;

            }
        }
    }
}