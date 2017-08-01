using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLServerDBManager;

namespace DBExportAgent
{
    class Program
    {
        /*
         * Steps to complete:
         * 1: Create a NEW database from a BAK file
         * 2: Delete unneeded artifacts: SPs, Fns, triggers, views, synonyms, indexes, foreign keys, constraints
         * 3: Run customized SP (to delete unneeded rows and tables)
         * 4: Backup NEW database to a BAK file
         * 5: Drop NEW database
         * 6: Delete exiting physical files for NEW database
         */
        static void Main(string[] args)
        {
            string SAConnectionString = ""; //This it to use the System Administrator credentials
            string DBOwnerConnectionString = ""; //This it to use the owner of the new DB credentials. QUESTION: Can they be the same? Should they?
            string ProductionBackupFile = "";
            string ResultingBackupFile = "";
            string NewDBName = "";
            string Script = "";

            int result = DBManager.RestoreDB(SAConnectionString, ProductionBackupFile, NewDBName);

            if (result == 0)
            {
                result = DBManager.ClearDB(DBOwnerConnectionString, NewDBName);

                if (result == 0)
                {
                    result = DBManager.ExecuteScript(DBOwnerConnectionString, NewDBName, Script);
                    if (result == 0)
                    {
                        result = DBManager.BackupDB(SAConnectionString, NewDBName, ResultingBackupFile);
                        if (result == 0)
                        {
                            result = DBManager.DropDB(SAConnectionString, NewDBName);
                            if (result == 0)
                            {
                                //SUCCESS!!!
                            }
                            else
                            {
                                //Log error
                            }
                        }
                        else
                        {
                            //Log error
                        }
                    }
                    else
                    {
                        //Log error
                    }
                }
                else
                {
                    //Log error
                }
            }
            else
            {
                //Log error
            }
        }
    }
}
