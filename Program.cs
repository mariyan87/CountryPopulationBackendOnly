using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Backend
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started");
            Console.WriteLine("Getting DB Connection...");
            Task<List<Tuple<string, int>>> sqliteCountryPopulations = null;

            DbConnection conn = null;

            try
            {
                var db = new SqliteDbManager();
                conn = db.getConnection();

                if (conn == null)
                {
                    Console.Error.WriteLine("Failed to get connection");
                    // TODO: considered as primary source, so better to throw exception
                }
                else
                {
                    var sqlLiteStatService = new SqlLiteStatService(db);
                    sqliteCountryPopulations = sqlLiteStatService.GetCountryPopulationsAsync();
                }
            }
            finally
            {
                if (conn != null)
                {
                    Console.WriteLine("Close connection.");
                    conn.Close();
                }
            }

            var concreteStatService = new ConcreteStatService();
            var concreteCountryPopulations = concreteStatService.GetCountryPopulationsAsync();
            List<Tuple<string, int>> results = null;

            if (sqliteCountryPopulations == null)
            {
                results = concreteCountryPopulations.Result.OrderBy(o => o.Item1).ToList();
            }
            else
            {
                Task.WaitAll(sqliteCountryPopulations, concreteCountryPopulations);

                var sqliteCountries = sqliteCountryPopulations.Result.Select(s => s.Item1);
                var filteredConcretePopulations = concreteCountryPopulations.Result.Where(c => !sqliteCountries.Contains(c.Item1));
                results = sqliteCountryPopulations.Result.Union(filteredConcretePopulations).OrderBy(o => o.Item1).ToList();
            }


            foreach (var row in results)
            {
                Console.WriteLine($"Country: {row.Item1.PadRight(35)} population: {row.Item2}");
            }

            Console.ReadLine();
        }
    }
}
