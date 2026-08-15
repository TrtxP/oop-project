namespace ClassLibraryATM.Classes
{
    public class AtmSettings
    {
        public string AtmId { get; init; } = "1";
        public string Address { get; init; } = string.Empty;
        public decimal CashAvailable { get; init; }
        public bool IsOnline { get; init; } = true;
        public decimal MaxWithdrawPerOperation { get; init; } = 30000m;
        public decimal FeePercent { get; init; }
    }
}
