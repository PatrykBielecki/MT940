namespace ExcelTest.Mt940
{
    class Transaction
    {
        public string TransactionDetails = "";
        public string TransactionValue;
        public string TransactionNip;
        public string TransactionInvNumber;
        public Transaction(string line)
        {
            line = line.Substring(0, line.IndexOf("N"));
            if (line.Contains("C")) TransactionValue = line.Substring(line.IndexOf("C") + 1);
            else TransactionValue = line.Substring(line.IndexOf("D") + 1);
            TransactionValue = TransactionValue.Replace(",", ".");
        }
    }
}
