using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paragoniarz
{
    internal class FinderManager
    {
        public string CreateSearchQuery(
            int userId,
            string nazwa,
            DateTime? dataOd,
            DateTime? dataDo
        )
        {
            string query =
                $"SELECT id, file_url, original_name, timestamp FROM dbo.files WHERE user_id = {userId}";

            if (!string.IsNullOrEmpty(nazwa))
            {
                query += $" AND original_name LIKE '%{nazwa}%'";
            }

            if (dataOd.HasValue)
            {
                DateTime startOfDay = dataOd.Value.Date;
                query += $" AND timestamp >= '{startOfDay.ToString("yyyy-MM-dd HH:mm:ss")}'";
            }

            if (dataDo.HasValue)
            {
                DateTime endOfDay = dataDo.Value.Date.AddDays(1).AddSeconds(-1);
                query += $" AND timestamp <= '{endOfDay.ToString("yyyy-MM-dd HH:mm:ss")}'";
            }

            return query;
        }
    }
}
