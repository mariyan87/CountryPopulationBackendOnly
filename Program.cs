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

            var db = new SqliteDbManager();
            DbConnection conn = db.getConnection();

            try
            {
                if (conn == null)
                {
                    Console.Error.WriteLine("Failed to get connection");
                }

                var sqlLiteStatService = new SqlLiteStatService(db);
                sqliteCountryPopulations = sqlLiteStatService.GetCountryPopulationsAsync();
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

            Task.WaitAll(sqliteCountryPopulations, concreteCountryPopulations);

            var sqliteCountries = sqliteCountryPopulations.Result.Select(s => s.Item1);
            var filteredConcretePopulations = concreteCountryPopulations.Result.Where(c => !sqliteCountries.Contains(c.Item1));
            var results = sqliteCountryPopulations.Result.Union(filteredConcretePopulations).OrderBy(o => o.Item1);

            foreach (var row in results)
            {
                Console.WriteLine($"Country: {row.Item1.PadRight(35)} population: {row.Item2}");
            }

            Console.WriteLine("all done");
            Console.ReadLine();
        }
    }
}
