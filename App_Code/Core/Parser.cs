using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Specialized;
using System.Text;
using Titan;
using System.Web.Security;

namespace Prem.PTC
{
    /// <summary>
    /// Parser allows all basic operations between script and database
    /// Compatibile with types: int, string, bool, Money, DateTime
    /// </summary>
    public class Parser : IDisposable
    {
        private string _connString;
        public SqlConnection Conn;
        public Database ConnectedTo { get; private set; }

        /// <summary>
        /// Creates parser that connects to the database
        /// </summary>
        public Parser(Database database)
        {
            ConnectedTo = database;
            _connString = ConnectionString.GetConnectionString(database);
            Conn = new SqlConnection(_connString);
        }

        public void Open()
        {
            try
            {
                Conn.Open();
                DatabaseConnectionPersister.SetLastSuccessfulDatabaseConnection(DateTime.Now);
            }
            catch (Exception ex)
            {
                DatabaseConnectionPersister.TryRestartApplication();
                throw new DbException(ex.Message);
            }
        }

        public void Close()
        {
            try
            {
                Conn.Close();
            }
            catch (Exception ex)
            {
                throw new DbException(ex.Message);
            }
        }

        /// <summary>
        /// Update records to specified table using WHERE directive. 
        /// Connection must be open. Use 'null' for no arguments.
        /// </summary>
        public void Update(string dbTable, Dictionary<string, object> What, Dictionary<string, object> Where)
        {
            //string readyCommand = "Not loaded yet";

            if (!AppSettings.IsDemo)
            {
                try
                {
                    StringBuilder Command = new StringBuilder();
                    Command.Append("UPDATE ")
                           .Append(dbTable)
                           .Append(" SET ");

                    SqlCommand fCommand = new SqlCommand();

                    Command.Append(GetCommandFromSet(What, ",  ", "A", ref fCommand));
                    if (Where != null)
                    {
                        Command.Append(" WHERE ");
                        Command.Append(GetCommandFromSet(Where, " AND ", "B", ref fCommand));
                    }
                    //readyCommand = Command;
                    fCommand.CommandText = Command.ToString();
                    fCommand.Connection = Conn;
                    fCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new DbException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Adds new row to specified table.
        /// Connection must be open. Use 'null' for no arguments.
        /// </summary>
        public void Insert(string dbTable, Dictionary<string, object> NameValue)
        {
            //string readyCommand = "Not loaded yet";

            if (!AppSettings.IsDemo)
            {
                try
                {
                    StringBuilder Command = new StringBuilder("INSERT INTO ");
                    Command.Append(dbTable)
                           .Append(" ");
                    SqlCommand fCommand = new SqlCommand();
                    Command.Append(GetInsertCommandFromSet(NameValue, ref fCommand));

                    //readyCommand = Command;
                    fCommand.CommandText = Command.ToString();
                    fCommand.Connection = Conn;
                    fCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new DbException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Select records from specified table using WHERE directive. 
        /// Connection must be open. Use 'null' for no arguments.
        /// </summary>
        public object Select(string dbTable, string What, Dictionary<string, object> Where, string sign = "=")
        {
            object Obj;
            try
            {
                StringBuilder Command = new StringBuilder("SELECT ");
                Command.Append(What)
                       .Append(" FROM ")
                       .Append(dbTable);
                SqlCommand fCommand = new SqlCommand();

                if (Where != null)
                {
                    Command.Append(" WHERE ");
                    Command.Append(GetCommandFromSet(Where, " AND ", "A", ref fCommand, sign));
                }
                fCommand.CommandText = Command.ToString();
                fCommand.Connection = Conn;

                Obj = fCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                throw new DbException(ex.Message);
            }
            return Obj;
        }

        public List<string> SelectList(string query)
        {
            List<String> columnData = new List<String>();
            using (SqlCommand command = new SqlCommand(query, Conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columnData.Add(reader.GetString(0));
                    }
                }
            }
            return columnData;
        }

        /// <summary>
        /// Select records from specified table using WHERE directive and store it in Datatable. 
        /// Connection must be open. Use 'null' for no arguments.
        /// </summary>
        public DataTable Select(string dbTable, StringCollection What, Dictionary<string, object> Where, string sign = "=")
        {
            try
            {
                StringBuilder collection = new StringBuilder(What[0]);
                for (int i = 1; i < What.Count; ++i) collection.Append(",").Append(What[i]);
                return select(dbTable, collection.ToString(), Where, sign);
            }
            catch (Exception ex)
            {
                throw new DbException(ex.Message);
            }
        }

        /// <summary>
        /// Select all records from specified table using WHERE directive and store it in Datatable. 
        /// Connection must be open. Use 'null' for no arguments.
        /// </summary>
        public DataTable Select(string dbTable, Dictionary<string, object> Where, string sign = "=")
        {
            return select(dbTable, "*", Where, sign);
        }

        private DataTable select(string dbTable, string What, Dictionary<string, object> Where, string sign = "=")
        {
            DataTable dTable = new DataTable(dbTable);

            StringBuilder Command = new StringBuilder("SELECT ");
            Command.Append(What)
                    .Append(" FROM ")
                    .Append(dbTable);
            SqlCommand fCommand = new SqlCommand();
            if (Where != null)
            {
                Command.Append(" WHERE ");
                Command.Append(GetCommandFromSet(Where, " AND ", "A", ref fCommand, sign));
            }
            fCommand.CommandText = Command.ToString();
            fCommand.Connection = Conn;
            SqlDataAdapter dAdapter = new SqlDataAdapter(fCommand);
            dAdapter.Fill(dTable);

            return dTable;
        }


        /// <summary>
        /// Delete records from specified table using WHERE directive.
        /// Connection must be open. Use 'null' for no arguments.
        /// </summary>
        public void Delete(string dbTable, Dictionary<string, object> Where, string sign = "=")
        {
            if (!AppSettings.IsDemo)
            {
                try
                {
                    StringBuilder Command = new StringBuilder("DELETE FROM ");
                    Command.Append(dbTable);
                    SqlCommand fCommand = new SqlCommand();

                    if (Where != null)
                    {
                        Command.Append(" WHERE ");
                        Command.Append(GetCommandFromSet(Where, " AND ", "A", ref fCommand, sign));
                    }
                    fCommand.CommandText = Command.ToString();
                    fCommand.Connection = Conn;

                    fCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new DbException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Counts records from specified table using WHERE directive. 
        /// Connection must be open. Use 'null' for no arguments. -1 = ERROR.
        /// </summary>
        public int Count(string dbTable, Dictionary<string, object> Where, string sign = "=")
        {
            int Counter = -1;
            try
            {
                StringBuilder Command = new StringBuilder("SELECT COUNT(*) FROM ");
                Command.Append(dbTable);
                SqlCommand fCommand = new SqlCommand();
                fCommand.CommandTimeout = 180;

                if (Where != null)
                {
                    Command.Append(" WHERE ");
                    Command.Append(GetCommandFromSet(Where, " AND ", "A", ref fCommand, sign));
                }
                fCommand.CommandText = Command.ToString();
                fCommand.Connection = Conn;

                Counter = (int)fCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new DbException(ex.Message);
            }
            return Counter;
        }

        private string Secure(string Str)
        {
            return Str.Replace("'", "''");
        }

        /// <summary>
        /// Convenient way to create string collection.
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static StringCollection Columns(params string[] columns)
        {
            StringCollection columnsCollection = new StringCollection();
            if (columns.Length > 0) columnsCollection.AddRange(columns);
            return columnsCollection;
        }

        private SqlDbType GetSqlDbType(object Item)
        {
            if (Item is CryptocurrencyMoney)
                return SqlDbType.Decimal;
            if (Item is Decimal)
                return SqlDbType.Decimal;
            if (Item is Money)
                return SqlDbType.Decimal;
            if (Item is Points)
                return SqlDbType.SmallMoney;
            if (Item is Int32)
                return SqlDbType.Int;
            if (Item is DateTime)
                return SqlDbType.DateTime;
            if (Item is string)
                return SqlDbType.NVarChar;
            if (Item is bool)
                return SqlDbType.Bit;
            if (Item is Double)
                return SqlDbType.Decimal;
            if (Item == null)
                return SqlDbType.VarChar;

            throw new ArgumentException("Type not supported by database. Only int, string, bool, Money, DateTime are allowed");
        }

        private string GetCommandFromSet(Dictionary<string, object> ValSet, string KeyWord, string ArgKey, ref SqlCommand fCommand, string sign = "=")
        {
            StringBuilder Command = new StringBuilder();
            int i = 0;
            foreach (KeyValuePair<string, object> Pair in ValSet)
            {
                StringBuilder Set = new StringBuilder();
                if (i > 0)
                {
                    Set.Append(KeyWord);
                }
                Set.Append(Secure(Pair.Key)).Append(" ").Append(sign).Append(" @Val").Append(ArgKey).Append(i);
                StringBuilder Temp = new StringBuilder();
                Temp.Append("@Val").Append(ArgKey).Append(i);

                fCommand.Parameters.Add(Temp.ToString(), GetSqlDbType(Pair.Value));

                if (Pair.Value is Money)
                {
                    fCommand.Parameters[Temp.ToString()].Value = ((Money)Pair.Value).ToDecimal();
                }
                else if (Pair.Value is CryptocurrencyMoney)
                {
                    fCommand.Parameters[Temp.ToString()].Value = ((CryptocurrencyMoney)Pair.Value).ToDecimal();
                }
                else if (Pair.Value is Points)
                {
                    fCommand.Parameters[Temp.ToString()].Value = ((Points)Pair.Value).ToDecimal();
                }
                else if (Pair.Value == null)
                {
                    fCommand.Parameters[Temp.ToString()].Value = DBNull.Value;
                }


                else
                {
                    fCommand.Parameters[Temp.ToString()].Value = Pair.Value;
                }
                Command.Append(Set);
                i++;
            }
            return Command.ToString();
        }

        private string GetInsertCommandFromSet(Dictionary<string, object> ValSet, ref SqlCommand fCommand)
        {
            StringBuilder Command = new StringBuilder();
            StringBuilder Command1 = new StringBuilder("(");
            StringBuilder Command2 = new StringBuilder(" VALUES (");
            int i = 0;

            foreach (KeyValuePair<string, object> Pair in ValSet)
            {
                if (i > 0) { Command1.Append(","); Command2.Append(","); }

                Command1.Append(Secure(Pair.Key));
                Command2.Append("@ValA").Append(i);

                StringBuilder Temp = new StringBuilder();
                Temp.Append("@ValA").Append(i);

                fCommand.Parameters.Add(Temp.ToString(), GetSqlDbType(Pair.Value));
                if (Pair.Value == null)
                {
                    fCommand.Parameters[Temp.ToString()].Value = DBNull.Value;
                }
                else if (Pair.Value.GetType() == typeof(CryptocurrencyMoney))
                {
                    fCommand.Parameters[Temp.ToString()].Value = ((CryptocurrencyMoney)Pair.Value).ToDecimal();
                }
                else if (Pair.Value.GetType() == typeof(Money))
                {
                    fCommand.Parameters[Temp.ToString()].Value = ((Money)Pair.Value).ToDecimal();
                }
                else if (Pair.Value.GetType() == typeof(Points))
                {
                    fCommand.Parameters[Temp.ToString()].Value = ((Points)Pair.Value).ToDecimal();
                }
                else
                {
                    fCommand.Parameters[Temp.ToString()].Value = Pair.Value;
                }
                i++;
            }
            Command1.Append(")"); Command2.Append(")");
            Command.Append(Command1).Append(Command2);
            return Command.ToString();
        }

        /// <summary>
        /// Executes the statement using ExecuteScalar method (can return single object)
        /// For: simple SELECT, SELECT COUNT
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public object ExecuteRawCommandScalar(string command)
        {
            object counter;
            try
            {
                SqlCommand fCommand = new SqlCommand();
                fCommand.CommandText = command;
                fCommand.Connection = Conn;
                counter = fCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new DbException(ex.Message);
            }
            return counter;
        }

        public object ExecuteRawCommandScalar(SqlCommand command)
        {
            object counter;
            try
            {
                command.Connection = Conn;
                counter = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new DbException(ex.Message);
            }
            return counter;
        }

        /// <summary>
        /// Executes the statement using ExecuteNonQuery method (no returns).
        /// For: UPDATE, INSERT, REMOVE
        /// </summary>
        public void ExecuteRawCommandNonQuery(string command)
        {
            if (!AppSettings.IsDemo)
            {
                try
                {
                    SqlCommand fCommand = new SqlCommand();
                    fCommand.CommandText = command;
                    fCommand.Connection = Conn;
                    fCommand.CommandTimeout = 360;
                    fCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new DbException(ex.Message);
                }
            }
        }

        public void ExecuteRawCommandNonQuery(SqlCommand command)
        {
            if (!AppSettings.IsDemo)
            {
                try
                {
                    command.Connection = Conn;
                    command.CommandTimeout = 360;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new DbException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Executes command SELECT query and save the results to the datatable
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public DataTable ExecuteRawCommandToDataTable(string command)
        {
            SqlCommand fCommand = new SqlCommand();
            fCommand.CommandText = command;
            fCommand.Connection = Conn;
            fCommand.CommandTimeout = 180;
            return ExecuteRawCommandToDataTable(fCommand);
        }

        public DataTable ExecuteRawCommandToDataTable(SqlCommand command)
        {
            DataTable dTable = new DataTable("none");
            command.Connection = Conn;
            command.CommandTimeout = 180;
            SqlDataAdapter dAdapter = new SqlDataAdapter(command);
            dAdapter.Fill(dTable);
            return dTable;
        }



        public void Dispose()
        {
            this.Close();
        }


    }

}