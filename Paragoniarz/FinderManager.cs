using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paragoniarz
{
    internal class FinderManager
    {
        //Metoda do tworzenia zapytania SQL na podstawie podanych parametrów
        public string CreateSearchQuery(string nazwa,string opis,string osoba,DateTime? dataOd,DateTime? dataDo)
        {
            // Budujemy podstawowe zapytanie SQL
            string query = "SELECT * FROM YourTable WHERE 1=1";

            // Sprawdzamy, czy pola nie są puste i dodajemy warunki do zapytania
            if (!string.IsNullOrEmpty(nazwa))
            {
                query += $" AND Nazwa LIKE '%{nazwa}%'";
            }

            if (!string.IsNullOrEmpty(opis))
            {
                query += $" AND Opis LIKE '%{opis}%'";
            }

            if (!string.IsNullOrEmpty(osoba))
            {
                query += $" AND Osoba LIKE '%{osoba}%'";
            }

            if (dataOd.HasValue)
            {
                query += $" AND Data >= '{dataOd.Value.ToString("yyyy-MM-dd")}'";
            }

            if (dataDo.HasValue)
            {
                query += $" AND Data <= '{dataDo.Value.ToString("yyyy-MM-dd")}'";
            }

            return query;
        }




    }
}
