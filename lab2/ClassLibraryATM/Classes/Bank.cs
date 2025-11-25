namespace ClassLibraryATM.Classes
{
    public class Bank
    {
        public string? Name { get; private set; }
        public List<AutomatedTellerMachine>? Atms { get; private set; }
        public Dictionary<string, Account> Accounts { get; private set; }
        private decimal TransferFeePercent { get; set; }
        private decimal MinBalanceRequired { get; set; }
        public List<Transaction> BackLedger { get; private set; }

        public Bank()
        {
            Name = "ATM №12";
            Atms = new List<AutomatedTellerMachine>();
            Accounts = new Dictionary<string, Account>();
            TransferFeePercent = 0;
            MinBalanceRequired = 0;
            BackLedger = new List<Transaction>();
        }

        public Bank(string name, List<AutomatedTellerMachine> atms, Dictionary<string, Account> accounts) : this()
        {
            Name = name;
            Atms = atms;
            Accounts = accounts;
        }

        public Bank(string name, List<AutomatedTellerMachine> atms, Dictionary<string, Account> accounts, decimal transferFeePercent, decimal minBalanceRequired, List<Transaction> backLedger) : this()
        {
            Name = name;
            Atms = atms;
            Accounts = accounts;
            TransferFeePercent = transferFeePercent;
            MinBalanceRequired = minBalanceRequired;
            BackLedger = backLedger;
        }

        public Bank(Bank other)
        {
            Name = other.Name;
            Atms = other.Atms != null ? new List<AutomatedTellerMachine>(other.Atms) : new List<AutomatedTellerMachine>();
            Accounts = other.Accounts != null ? new Dictionary<string, Account>(other.Accounts) : new Dictionary<string, Account>();
            TransferFeePercent = other.TransferFeePercent;
            MinBalanceRequired = other.MinBalanceRequired;
            BackLedger = other.BackLedger != null ? new List<Transaction>(other.BackLedger) : new List<Transaction>();
        }

        public void RegisterAccount(Account acc)
        {
            if (acc.CardNumber != null)
            {
                Accounts[acc.CardNumber] = acc;
            }
        }

        public Account? FindAccount(string cardNumber)
        {
            Accounts.TryGetValue(cardNumber, out var acc);
            return acc;
        }
    }
}
