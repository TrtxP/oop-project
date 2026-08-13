using ClassLibraryATM.Enums;
using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Builders
{
    public class AtmBuilder
    {
        private string? _atmId;
        private string? _address;
        private decimal _cashAvailable = 0;
        private bool _isOnline = true;
        private IBank? _ownerBank;
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

        public AtmBuilder WithOwnerBank(IBank bank)
        {
            _ownerBank = bank;
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

        public AutomatedTellerMachine Build()
        {
            if (_ownerBank == null)
                throw new InvalidOperationException("Банк власник обов'язковий.");

            return new AutomatedTellerMachine(
                _atmId,
                _address,
                _cashAvailable,
                _isOnline,
                _ownerBank,
                DateTime.Now,
                _maxWithdrawPerOperation,
                _feePercent
            );
        }
    }
}
