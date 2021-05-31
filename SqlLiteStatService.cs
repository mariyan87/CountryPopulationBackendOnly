using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Backend
{
    public class SqlLiteStatService : IStatService
    {
        private readonly SqliteDbManager db;

        public SqlLiteStatService(SqliteDbManager db)
        {
            this.db = db;
        }

        public List<Tuple<string, int>> GetCountryPopulations()
		{
            var query = @"
SELECT country.CountryName,
       sum(city.population)
FROM country
LEFT JOIN state ON country.CountryId = state.CountryId
LEFT JOIN city ON state.StateId = city.StateId
GROUP BY country.CountryName";

			var dt = db.GetDataByQuery(query);

			var result = new List<Tuple<string, int>>();

			foreach (var row in dt.Rows)
            {
                var pair = ((DataRow)row).ItemArray;

                string country = pair[0].ToString();

                if (!int.TryParse(pair[1].ToString(), out var population))
                {
                    throw new FormatException("Invalid population data");
                }

                result.Add(new Tuple<string, int>(country, population));
            }

            return result;
		}


        public async Task<List<Tuple<string, int>>> GetCountryPopulationsAsync()
        {
            return await Task.FromResult<List<Tuple<string, int>>>(GetCountryPopulations());
        }
    }
}
