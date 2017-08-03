using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Data.SqlClient;
using System.IO;

namespace SQLServerDBManager
{
    public class DBManager
    {
        public string sServidor = "";
        public string sUsuario = "";
        public string sContrasenia = "";

        private static ServerConnection Connection(string servidor, string usuario, string contrasenia)
        {
            ServerConnection conexion = new ServerConnection(servidor);

            conexion.LoginSecure = false;
            conexion.Login = usuario;
            conexion.Password = contrasenia;

            return conexion;
        }

        private static ServerConnection Connection(string connectionString)
        {
            //String connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True;MultipleActiveResultSets=True";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            ServerConnection oServerConnection = new ServerConnection(sqlConnection);

            return oServerConnection;
        }

        public static int RestoreDB(string ConnectionString, string SourceBAKfile, string NewDBname, Registrador.IRegistroEjecucion Logger)
        {
            try{
                ServerConnection oConnection = Connection(ConnectionString); ;
                Server oServer = new Server(oConnection);
                Database oDataBase = oServer.Databases[NewDBname];

                if (oDataBase != null){                    
                    Logger.RegistrarError("ya existe una BD con el nombre '" + NewDBname + "', por lo cual no es posible restaurar la BD y así evitar perdida de información.");
                    return 2;
                }
                else{
                    BackupDeviceItem oDevice = new BackupDeviceItem(SourceBAKfile, DeviceType.File);
                    Restore oRestore = new Restore();
                    RelocateFile DataFile = new RelocateFile();
                    RelocateFile LogFile = new RelocateFile();
                    string Path = oServer.MasterDBPath;

                    oRestore.Devices.Add(oDevice);

                    string LogicFileName_MDF = oRestore.ReadFileList(oServer).Rows[0][0].ToString();
                    DataFile.LogicalFileName = LogicFileName_MDF;
                    DataFile.PhysicalFileName = Path + "\\" + NewDBname + ".mdf";

                    string LogicFileName_LDF = oRestore.ReadFileList(oServer).Rows[1][0].ToString();
                    LogFile.LogicalFileName = LogicFileName_LDF;
                    LogFile.PhysicalFileName = Path + "\\" + NewDBname + ".ldf";

                    oRestore.NoRecovery = false;

                    oRestore.RelocateFiles.Add(DataFile);
                    oRestore.RelocateFiles.Add(LogFile);

                    oRestore.Database = NewDBname;
                    oRestore.SqlRestore(oServer);

                    oRestore.Devices.Remove(oDevice);
                    oConnection.Disconnect();
                }
            }catch (Exception ex){
                Logger.RegistrarError("No fue posible la restaurar la BD. Detalles: " + Environment.NewLine + ex.Message);
                return 1;
            }

            return 0;
        }

        public static int BackupDB(string ConnectionString, string DBname, string DestinationBAKFile, Registrador.IRegistroEjecucion Logger)
        {
            try{
                ServerConnection oConnection = Connection(ConnectionString);
                Server oServer = new Server(oConnection);
                Database oDatabase = oServer.Databases[DBname];
                Backup oBackup = new Backup();
                BackupDeviceItem oDevice = new BackupDeviceItem(DestinationBAKFile + ".bak", DeviceType.File);

                oBackup.Action = BackupActionType.Database;

                oBackup.BackupSetDescription = "Respaldo de '" + DestinationBAKFile + "'.";
                oBackup.BackupSetName = DestinationBAKFile + ".bak";
                oBackup.Database = DBname;
                oBackup.Devices.Add(oDevice);
                oBackup.Incremental = false;

                oBackup.SqlBackup(oServer);
                oBackup.Devices.Remove(oDevice);

                oServer.KillAllProcesses(DBname);
                oDatabase.Drop();
                oConnection.Disconnect();
            }catch (Exception ex){
                Logger.RegistrarError("No fue posible crear el repaldo de la BD. Detalles: " + Environment.NewLine + ex.Message);
                return 1;
            }

            return 0;
        }

        public static int DropDB(string ConnectionString, string DBName)
        {            
            return 0;
        }

        //public static int DropDB(string ConnectionString, string DBName)
        //{
        //    //Step 1: Get the names of physical files for DB
        //    List<string> physicalFiles = new List<string>();
        //    //Setp 2: Drop the DB from the SQL Server instance
        //    //Step 3: Remove physical files from computer
        //    if (RemoveDBFiles(physicalFiles) == 0)
        //    {
        //    }
        //    else
        //    {
        //        //Log error
        //    }
        //    return 0;
        //}

        //private static int RemoveDBFiles(List<string> DBFiles)
        //{
        //    //This function should remove the DB files from the server
        //    return 0;
        //}

        public static int ClearDB(string ConnectionString, string DBname, Registrador.IRegistroEjecucion Logger, bool deleteSPs = true, bool deleteFns = true, 
            bool deleteViews = true, bool deleteTriggers = true, bool deleteFKs = true, bool deleteSynonims = true)
        {

            try
            {
                ServerConnection oConnection =  Connection(ConnectionString); 
                Server oServer = new Server(oConnection);
                Database oDataBase = oServer.Databases[DBname];

                ////Delete views
                if (deleteViews){ 
                    IEnumerable<View> enumViews = oDataBase.Views.Cast<View>().Where(x => !x.IsSystemObject);
                    while (enumViews != null && enumViews.Count() > 0)
                    {
                        enumViews.ElementAt(0).Drop();
                    }
                }

                ////Delete Stored Procedures
                if (deleteSPs){
                    IEnumerable<StoredProcedure> enumStoredProcedures = oDataBase.StoredProcedures.Cast<StoredProcedure>().Where(x => !x.IsSystemObject);
                    while (enumStoredProcedures != null && enumStoredProcedures.Count() > 0)
                    {
                        enumStoredProcedures.ElementAt(0).Drop();
                    }
                }

                ////Delete Functions
                if (deleteFns){
                    IEnumerable<UserDefinedFunction> enumFunctions = oDataBase.UserDefinedFunctions.Cast<UserDefinedFunction>().Where(x => !x.IsSystemObject);
                    while (enumFunctions != null && enumFunctions.Count() > 0)
                    {
                        enumFunctions.ElementAt(0).Drop();
                    }
                }

                ////Delete Triggers
                if (deleteTriggers){
                    IEnumerable<Trigger> enumTriggers = oDataBase.Triggers.Cast<Trigger>().Where(x => !x.IsSystemObject);
                    while (enumTriggers != null && enumTriggers.Count() > 0)
                    {
                        enumTriggers.ElementAt(0).Drop();
                    }
                }

                //Delete Synonyms
                if (deleteSynonims) {
                    IEnumerable<Synonym> aSynonyms = oDataBase.Synonyms.Cast<Synonym>().Where(x => !x.IsSchemaOwned);
                    while (aSynonyms != null && aSynonyms.Count() > 0)
                    {
                        aSynonyms.ElementAt(0).Drop();
                    }
                }

                //Delete Foreign Keys for each table
                if (deleteFKs){
                    foreach (Table oTable in oDataBase.Tables)
                    {
                        if (!oTable.IsSystemObject)
                        {
                            IEnumerable<ForeignKey> enumForeignKeys = oTable.ForeignKeys.Cast<ForeignKey>().Where(x => !x.IsSystemNamed);
                            while (enumForeignKeys != null && enumForeignKeys.Count() > 0)
                            {
                                enumForeignKeys.ElementAt(0).Drop();
                            }
                        }
                    }
                }

                oConnection.Disconnect();
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("No fue posible limpiar la BD. Detalles: " + Environment.NewLine + ex.Message);
                return 1;
            }

            return 0;
        }

        public static int ExecuteScript(string ConnectionString, string DBname, string sqlScript, Registrador.IRegistroEjecucion Logger)
        {
            try
            {
                ServerConnection oConnection = Connection(ConnectionString); 
                Server oServer = new Server(oConnection);
                Database oDataBase = oServer.Databases[DBname];

                FileInfo InfoArchivo = new FileInfo(sqlScript);
                string script = InfoArchivo.OpenText().ReadToEnd();

                oDataBase.ExecuteNonQuery(script);

                oConnection.Disconnect();
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("No fue posible ejecutar el script. Detalles: " + Environment.NewLine + ex.Message);
                return 1;
            }

            return 0;
        }
    }
}
