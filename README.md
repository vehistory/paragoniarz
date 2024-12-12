# paragoniarz
paragony paragony azure, cs i inne parabole
### Do prawidłowego połączenia sie z bazą danych należy zainstalować pakiet MySql.Data przez NuGet

### Aby z App.config pobierał sie conection string nalezy w właściwościach pliku zmienic opcje "Copy to Output Directory" na "Copy Always"
    <configuration>
	<connectionStrings>
		<add name="ParagoniarzConnectionString"
			 connectionString="Connection String" />
	</connectionStrings>
  </configuration>

Connection string nalezy odszukac w Azure/database/setting/connection string/ klucz dla ADO.NET (SQL authentication)
