using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paragoniarz
{
    internal class FinderManager
    {
        // Metoda do tworzenia zapytania SQL na podstawie podanych parametrów i idUser
        public string CreateSearchQuery(int userId,string nazwa,DateTime? dataOd,DateTime? dataDo)
        {
            // Budujemy podstawowe zapytanie SQL - szukamy plików powiązanych z danym userId
            string query = $"SELECT file_url, original_name, timestamp FROM dbo.files WHERE user_id = {userId}";

            // Dodajemy filtrację po nazwie pliku (original_name)
            if (!string.IsNullOrEmpty(nazwa))
            {
                query += $" AND original_name LIKE '%{nazwa}%'";
            }

            // Dodajemy warunki dla daty
            if (dataOd.HasValue)
            {
                query += $" AND timestamp >= '{dataOd.Value.ToString("yyyy-MM-dd")}'";
            }

            if (dataDo.HasValue)
            {
                query += $" AND timestamp <= '{dataDo.Value.ToString("yyyy-MM-dd")}'";
            }

            return query;
        }

    }
}
