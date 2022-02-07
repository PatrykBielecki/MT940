using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ExcelTest.Mt940
{
    class MT940
    {
        const string TAG_PATTERN = @"^:(?'tag'[^:]+):(?'value'.*)";

        public List<Transaction> Transactions = new();  //61 + 86

        public MT940(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            int transactionCount = 0;
            int tagCheck = 0;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Match match = Regex.Match(line, TAG_PATTERN);
                string tag = match.Groups["tag"].Value;
                string value = match.Groups["value"].Value;

                if (line.StartsWith(":"))
                {
                    if (tag == "61")
                    {
                        tagCheck = 61;
                        Transactions.Add(new Transaction(value));
                    }
                    else if (tag == "86")
                    {
                        tagCheck = 86;
                        transactionCount++;
                    }
                }
                if (line.StartsWith("~") && tagCheck == 86)
                {
                    Transaction tran = Transactions[transactionCount - 1];
                    Transactions[transactionCount - 1].TransactionDetails += line.Substring(3);
                    if (tran.TransactionDetails.Contains("/IDC/") && tran.TransactionDetails.Contains("/INV/")) tran.TransactionNip = tran.TransactionDetails.Substring(tran.TransactionDetails.IndexOf("/IDC/") + 5, tran.TransactionDetails.LastIndexOf("/INV/") - tran.TransactionDetails.IndexOf("/IDC/") - 5);
                    if (tran.TransactionDetails.Contains("/INV/") && tran.TransactionDetails.Contains("/TXT/")) tran.TransactionInvNumber = tran.TransactionDetails.Substring(tran.TransactionDetails.IndexOf("/INV/") + 5, tran.TransactionDetails.LastIndexOf("/TXT/") - tran.TransactionDetails.IndexOf("/INV/") - 5);
                }
            }
            reader.Close();
        }
    }
}
