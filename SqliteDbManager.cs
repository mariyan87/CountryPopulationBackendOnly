using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Backend
{
    public class SqliteDbManager : IDbManager
    {
        private static SQLiteConnection connection = null;

        private static DbConnection GetConnectionInstance()
        {
            if (connection != null)
            {
                return connection;
            }

            try
            {
                connection = new SQLiteConnection("Data Source=citystatecountry.db; Version=3; FailIfMissing=True");
                return connection.OpenAndReturn();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public DbConnection getConnection()
        {
            return GetConnectionInstance();
        }

        /// <summary>
        /// Reads data from given query.
        /// </summary>
        /// <param name="query">SQL query.</param>
        /// <returns>Data preserved in a Datatable.</returns>
        public DataTable GetDataByQuery(string query)
        {
            using (var dataAdapter = new SQLiteDataAdapter(query, connection))
            {
                DataTable dt = new DataTable();

                dataAdapter.Fill(dt);

                return dt;
            }
        }
    }
}
