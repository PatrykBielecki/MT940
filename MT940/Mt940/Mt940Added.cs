using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

namespace ExcelTest.Mt940
{
    class Mt940Added
    {
        private static readonly string serverUrl = ConfigurationManager.AppSettings.Get("serverUrl");
        private static readonly string dbDataSource = ConfigurationManager.AppSettings.Get("dbDataSource");
        private static readonly string dbInitialCatalog = ConfigurationManager.AppSettings.Get("dbInitialCatalog");
        private static readonly string dbUserId = ConfigurationManager.AppSettings.Get("dbUserId");
        private static readonly string dbPassword = ConfigurationManager.AppSettings.Get("dbPassword");
        public Mt940Added(FileSystemEventArgs e, string filePath)
        {
            ShowWindow(GetConsoleWindow(), 5);
            Console.WriteLine($"Dodano plik Mt940: {e.FullPath} \n");

            Console.WriteLine("Wczytywanie danych z pliku Mt940... \n");
            MT940 mt940 = new(filePath);
            Console.WriteLine("Dane  wczytane \n");

            string connString = $"Data Source={dbDataSource}; Initial Catalog={dbInitialCatalog}; User Id={dbUserId};Password={dbPassword};";
            List<string> fieldsNames = new List<string>();
            List<Invoice> invoicesInfo = new List<Invoice>();

            Console.WriteLine("Pobieranie faktur z bazy... \n");
            try
            {
                SqlConnection conn = new(connString);

                string query = @"";

                SqlCommand cmd = new(query, conn);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fieldsNames.Add(reader[0].ToString());
                    }
                }

                query = $"";

                cmd = new(query, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Invoice tmp = new Invoice();
                        tmp.ElementId = reader[0].ToString();
                        tmp.Nip = reader[1].ToString();
                        tmp.InvNumber = reader[2].ToString();
                        tmp.TotalValue = reader[3].ToString();
                        invoicesInfo.Add(tmp);
                    }
                }

                conn.Close();
                Console.WriteLine("Pobrano faktury z bazy \n");
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            foreach (var tran in mt940.Transactions)
            {
                foreach (var invoice in invoicesInfo)
                {
                    if (tran.TransactionNip != null && tran.TransactionInvNumber != null)
                    {
                        if (tran.TransactionNip == invoice.Nip && float.Parse(tran.TransactionValue) == float.Parse(invoice.TotalValue) && tran.TransactionInvNumber == invoice.InvNumber)
                        {
                            Console.WriteLine($"Znaleziono fakture {invoice.InvNumber} numer nip {invoice.Nip} na kwote {tran.TransactionValue}");
                            _ = new Mt940Sender(invoice.ElementId, serverUrl);
                        }
                    }
                    else break;
                }
            }

            Console.WriteLine("Zakonczono przesuwanie faktur");
            ShowWindow(GetConsoleWindow(), 0);

        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
