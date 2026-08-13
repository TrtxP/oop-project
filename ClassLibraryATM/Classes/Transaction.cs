using ClassLibraryATM.Enums;

namespace ClassLibraryATM.Classes
{
    public class Transaction
    {
        public TransactionType Type { get; private set; }
        public decimal Amount { get; private set; }
        public string FromCard { get; private set; }
        public string ToCard { get; private set; }
        public DateTime Date { get; private set; }

        public decimal Fee { get; private set; }

        public Transaction()
        {
            Type = TransactionType.CheckBalance;
            Amount = 0;
            FromCard = "0000 0000 0000 0000";
            ToCard = "0000 0000 0000 0000";
            Date = new DateTime(2025, 10, 23);
            Fee = 0;
        }

        public Transaction(TransactionType type, decimal amount, string fromCard, string toCard) : this()
        {
            Type = type;
            Amount = amount;
            FromCard = fromCard;
            ToCard = toCard;
        }

        public Transaction(TransactionType type, decimal amount, string fromCard, string toCard, DateTime date, decimal fee) : this()
        {
            Type = type;
            Amount = amount;
            FromCard = fromCard;
            ToCard = toCard;
            Date = date;
            Fee = fee;
        }

        public Transaction(Transaction other)
        {
            Type = other.Type;
            Amount = other.Amount;
            FromCard = other.FromCard;
            ToCard = other.ToCard;
            Date = other.Date;
            Fee = other.Fee;
        }
    }
}
