using ExcelTest.API_Classes.Body_Elements;
using System.Collections.Generic;
using System;

namespace ExcelTest.Mt940
{
    class Mt940Sender
    {
        private static IApiConnection connection;

        public Mt940Sender(string elementId, string url)
        {

            try
            {
                connection = new ApiConnection(url, new Account());
            }
            catch (Exception)
            {

                Console.WriteLine("Cannot connect to the server");
                return;
            }

            MoveInvoice(elementId);
        }

        private static void MoveInvoice(string id)
        {
            List<object> requestBodyElemetsList2 = new();
            Attachments attachments = new();

            requestBodyElemetsList2.Add(attachments);

            connection.Request("/api/data/v3.0/db/1/elements/" + id + "?path=" + "39da16b0-e366-414b-b996-1c5d46eeb575", requestBodyElemetsList2, HttpRequestMethod.PATCH);
            Console.WriteLine("Faktura przesunieta do nastepnego kroku");
        }
    }
}
