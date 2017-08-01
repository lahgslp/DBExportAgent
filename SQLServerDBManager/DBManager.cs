using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLServerDBManager
{
    public class DBManager
    {
        public static int RestoreDB(string ConnectionString, string SourceBAKFile, string NewDBName)
        {
            return 0;
        }

        public static int BackupDB(string ConnectionString, string DBName, string DestinationBAKFile)
        {
            return 0;
        }

        public static int DropDB(string ConnectionString, string DBName)
        {
            //Step 1: Get the names of physical files for DB
            List<string> physicalFiles = new List<string>();
            //Setp 2: Drop the DB from the SQL Server instance
            //Step 3: Remove physical files from computer
            if (RemoveDBFiles(physicalFiles) == 0)
            {
            }
            else
            {
                //Log error
            }
            return 0;
        }

        private static int RemoveDBFiles(List<string> DBFiles)
        {
            //This function should remove the DB files from the server
            return 0;
        }

        public static int ClearDB(string ConnectionString, string DBName
            , bool deleteSPs = true, bool deleteFns = true, bool deleteViews = true, bool deleteTriggers = true, bool deleteFKs = true, bool deleteSynonims = true)
        {
            return 0;
        }

        public static int ExecuteScript(string ConnectionString, string DBName, string sqlScript)
        {
            return 0;
        }
    }
}
