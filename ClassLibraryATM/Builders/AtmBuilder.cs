using ClassLibraryATM.Classes;

namespace ClassLibraryATM.Builders
{
    public class AtmBuilder
    {
        private string? _atmId;
        private string? _address;
        private decimal _cashAvailable = 0;
        private bool _isOnline = true;
        private decimal _maxWithdrawPerOperation = 30000m;
        private decimal _feePercent = 0;

        public AtmBuilder WithAtmId(string atmId)
        {
            _atmId = atmId;
            return this;
        }

        public AtmBuilder WithAddress(string address)
        {
            _address = address;
            return this;
        }

        public AtmBuilder WithCashAvailable(decimal cash)
        {
            _cashAvailable = cash;
            return this;
        }

        public AtmBuilder WithOnlineStatus(bool isOnline)
        {
            _isOnline = isOnline;
            return this;
        }

        public AtmBuilder WithMaxWithdrawPerOperation(decimal max)
        {
            _maxWithdrawPerOperation = max;
            return this;
        }

        public AtmBuilder WithFeePercent(decimal fee)
        {
            _feePercent = fee;
            return this;
        }

        public AtmSettings Build()
        {
            if (string.IsNullOrWhiteSpace(_atmId))
            {
                throw new InvalidOperationException("Ідентифікатор банкомату є обов'язковим.");
            }

            if (string.IsNullOrWhiteSpace(_address))
            {
                throw new InvalidOperationException("Адреса банкомату є обов'язковою.");
            }

            return new AtmSettings
            {
                AtmId = _atmId,
                Address = _address,
                CashAvailable = _cashAvailable,
                IsOnline = _isOnline,
                MaxWithdrawPerOperation = _maxWithdrawPerOperation,
                FeePercent = _feePercent
            };
        }
    }
}
