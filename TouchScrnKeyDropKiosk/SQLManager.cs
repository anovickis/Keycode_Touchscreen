using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Threading;

namespace KeyCabinetKiosk
{
    class SQLManager
    {
        string strConnectionString = @Program.RESERVATION_DATABASE_CONNECTION_STRING;
        public SQLManager()
        {
           
        }

        public SqlParameterCollection SqlStoredProcedure(string ProcedureName, SqlParameterCollection ProcedureParamas)
        {
            SqlCommand comm = new SqlCommand(); //This is needed because there is no public contructor for SqlParameterCollection
            SqlParameterCollection ReturnParams = comm.Parameters;
            SqlParameter ReturnValue = null;
            try
            {
                using (SqlConnection connSQL = new SqlConnection(strConnectionString))
                {
                    // Create the SQL command object and set its properties.
                    SqlCommand cmSQL = new SqlCommand();
                    cmSQL.Connection = connSQL;
                    cmSQL.CommandText = ProcedureName;
                    cmSQL.CommandType = CommandType.StoredProcedure;

                    foreach (SqlParameter p in ProcedureParamas)
                    {
                        if (p.Direction == ParameterDirection.Input)
                            cmSQL.Parameters.Add(CreateParameter(p.ParameterName, p.Direction, p.Value, p.SqlDbType, p.Size));
                        else if (p.Direction == ParameterDirection.Output)
                        {
                            cmSQL.Parameters.Add(CreateParameter(p.ParameterName, p.Direction, p.Value, p.SqlDbType, p.Size));
                            ReturnParams.Add(CreateParameter(p.ParameterName, p.Direction, p.Value, p.SqlDbType, p.Size));
                        }
                        else if (p.Direction == ParameterDirection.ReturnValue)
                        {
                            cmSQL.Parameters.Add(CreateParameter(p.ParameterName, p.Direction, p.Value, p.SqlDbType, p.Size));
                            ReturnValue = CreateParameter(p.ParameterName, p.Direction, p.Value, p.SqlDbType, p.Size);
                        }
                    }
                    
                    // Open the connection and execute the reader.
                    connSQL.Open();

                    cmSQL.ExecuteNonQuery();

                    ReturnValue.Value = (int)cmSQL.Parameters["Return_Val"].Value;
                    foreach (SqlParameter p in ReturnParams)
                    {
                        p.Value = cmSQL.Parameters[p.ParameterName].Value;
                    }
                    ReturnParams.Add(ReturnValue);                    
                }
            }
            catch (SqlException expSql)
            {
                //A SQL Server exception was thrown - process/display exception.
                Program.logEvent("SQL ERROR:" + ProcedureName + ": " + expSql.Message);
                Program.SqlManager.ErrorDatabaseEntry("", "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "SQL ERROR:" + ProcedureName + ": " + expSql.Message, "", 0);
                Program.ShowErrorMessage(LanguageTranslation.DATABASE_ERROR + Program.SERVICE_MANAGER_NUMBER, 6000);
                return null;
            }
            catch (Exception ex)
            {
                //An application exception was thrown - process/display exception.
                Program.logEvent("GENERAL EXCEPTION:" + ProcedureName + ": " + ex.Message);
                Program.SqlManager.ErrorDatabaseEntry("", "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "GENERAL EXCEPTION:" + ProcedureName + ": " + ex.Message, "", 0);
                return null;
            }
            return ReturnParams;            
        }

        public bool ErrorDatabaseEntry(string Reservation, string Operator, string SQLFailTimeStamp, string KioskNum, int FailErrorFlag, string Generic1, string Generic2, int Generic3)
        {
            if (!Program.ENABLE_SQLITE)
                return true;

            string dbFile = @Program.SQLITE_DATABASE_NAME;

            string connString = string.Format(@"Data Source={0}; Pooling=false; FailIfMissing=false;", dbFile);

            using (SQLiteConnection dbConn = new System.Data.SQLite.SQLiteConnection(connString))
            {
                dbConn.Open();
                using (System.Data.SQLite.SQLiteCommand cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText =  @"INSERT INTO RES_KEY_FAIL
                                        (
                                           RESV_RESERV_NO,          
                                           OPERATOR_NO,             
                                           FAIL_DATETIME,           
                                           SQL_FAIL_TIME_STAMP,     
                                           KIOSK_FAIL_NO,           
                                           FAIL_ERROR_FLAG,         
                                           COL_GENERIC_1,           
                                           COL_GENERIC_2,          
                                           COL_GENERIC_3           
                                        )
                                        VALUES
                                        (
                                            @Reservation,
                                            @Operator,
                                            @FailDateTime,
                                            @SQLFailDateTime,
                                            @KioskID,
                                            @FailErrFlag,
                                            @Generic1,
                                            @Generic2,
                                            @Generic3
                                        )";


                    //parameterized update - more flexibility on parameter creation

                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                    {
                        ParameterName = "@Reservation",
                        Value = Reservation
                    });

                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                    {
                        ParameterName = "@Operator",
                        Value = Operator
                    });

                    // SQLite date format is: yyyy-MM-dd HH:mm:ss
                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                    {
                        ParameterName = "@FailDateTime",
                        Value = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now)
                    });

                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                    {
                        ParameterName = "@SQLFailDateTime",
                        Value = SQLFailTimeStamp
                    });

                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                    {
                        ParameterName = "@KioskID",
                        Value = Program.KIOSK_ID
                    });

                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                    {
                        ParameterName = "@FailErrFlag",
                        Value = FailErrorFlag
                    });

                    string gen1;
                    if (Generic1.Length > 254)
                        gen1 = Generic1.Substring(0, 254);
                    else 
                        gen1 = Generic1;
                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                    {
                        ParameterName = "@Generic1",
                        Value = gen1
                    });

                    string gen2;
                    if (Generic2.Length > 254)
                        gen2 = Generic2.Substring(0, 254);
                    else
                        gen2 = Generic2;
                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                    {
                        ParameterName = "@Generic2",
                        Value = gen2
                    });

                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                    {
                        ParameterName = "@Generic3",
                        Value = Generic3
                    });

                    cmd.ExecuteNonQuery();
                }
                Program.logEvent("Local SQLite Database Error Inserted Successfully");

                if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
            }
            return true;
        }

        public void TrimSQLiteDatabase(string filename, string tablename, int MaxRows, string TestColumnName)
        {
            if (!Program.ENABLE_SQLITE)
                return;

            string connString = string.Format(@"Data Source={0}; Pooling=false; FailIfMissing=false;", filename);

            using (SQLiteConnection dbConn = new System.Data.SQLite.SQLiteConnection(connString))
            {
                dbConn.Open();
                int ret = 0; string olddate;
                using (System.Data.SQLite.SQLiteCommand cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText =  @"SELECT COUNT(*) AS NumberOfRows FROM " + tablename;
                                        
                    cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                    {
                        ParameterName = "NumberOfRows"
                    });
                    
                    //int ret = cmd.ExecuteNonQuery();
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    rdr.Read();
                    ret = rdr.GetInt32(0); //get the number from return parameter position 0
                    rdr.Close();
                    if (ret > MaxRows)
                    {
                        //obtain the minimum value for the test column (usually a date field)
                        cmd.CommandText = @"SELECT MIN(" + TestColumnName + ") AS OldestDate FROM " + tablename;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "OldestDate"
                        });

                        SQLiteDataReader rdr2 = cmd.ExecuteReader();
                        rdr2.Read();
                        olddate = rdr2.GetString(0);                     
                        rdr2.Close();

                        //delete all rows containing that minimum test value
                        cmd.CommandText = @"DELETE FROM " + tablename + " WHERE " + TestColumnName + " = '" + olddate + "'";
                        cmd.Parameters.Clear();
                        cmd.ExecuteNonQuery();
                    }
                }
                                

                Program.logEvent("Local SQLite Database Table " + tablename + " Trimmed Successfully");

                if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
            }
        }

        public SqlParameter CreateParameter(string Name, ParameterDirection dir, object value, SqlDbType type)
        {
            SqlParameter p = new SqlParameter();
            p.ParameterName = Name;
            p.Direction = dir;
            p.Value = value;
            p.SqlDbType = type;
            return p;
        }

        public SqlParameter CreateParameter(string Name, ParameterDirection dir, object value, SqlDbType type, int size)
        {
            SqlParameter p = new SqlParameter();
            p.ParameterName = Name;
            p.Direction = dir;
            p.Value = value;
            p.SqlDbType = type;
            p.Size = size;
            return p;
        }
    }
}
