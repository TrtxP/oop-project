using ClassLibraryATM.Enums;
using ClassLibraryATM.Events;
using ClassLibraryATM.Interfaces;
using ClassLibraryATM.Services;

namespace ClassLibraryATM.Classes
{
    public class AutomatedTellerMachine : IAtm
    {
        public string? AtmId { get; private set; }
        public string? Address { get; private set; }
        public decimal CashAvailable { get; set; }
        private bool _isOnline;
        public IBank OwnerBank { get; private set; }
        public DateTime LastServiceDate { get; private set; }
        private decimal _maxWithdrawPerOperation;
        private decimal _feePercent;
        public List<Transaction> AtmJournal { get; private set; }

        public IAccount? CurrentAccount { get; private set; }

        public AtmState State { get; private set; }

        private readonly IAtmEventPublisher _atmEventsPublichser;

        private readonly IAuthenticationService _authService;
        private readonly IWithdrawService _withdrawService;
        private readonly IDepositService _depositService;
        private readonly ITransferService _transferService;
        private readonly ITransactionService _transactionService;

        public AutomatedTellerMachine()
        {
            AtmId = "1";
            Address = "Вулиця Героїв Чорнобиля";
            CashAvailable = 0;
            _isOnline = true;
            OwnerBank = new Bank();
            LastServiceDate = new DateTime(2025, 09, 18, 22, 32, 24);
            _maxWithdrawPerOperation = 30000m;
            _feePercent = 0;
            AtmJournal = new List<Transaction>();
            CurrentAccount = null;
            State = AtmState.Idle;
            _authService = new AuthenticationService(new Validators.PinValidator());
            _withdrawService = new WithdrawService(new Validators.AmountValidator());
            _depositService = new DepositService(new Validators.AmountValidator());
            _transferService = new TransferService(new Validators.AmountValidator());
            _transactionService = new TransactionService();
            _atmEventsPublichser = new AtmEventPublisher();
        }

        public AutomatedTellerMachine(string atmId, string address, decimal cashAvailable, bool isOnline, IBank ownerBank) : this()
        {
            AtmId = atmId;
            Address = address;
            CashAvailable = cashAvailable;
            _isOnline = isOnline;
            OwnerBank = ownerBank ?? new Bank();
            State = _isOnline ? AtmState.Idle : AtmState.OutOfService;
        }

        public AutomatedTellerMachine(string atmId, string address, decimal cashAvailable, bool isOnline, IBank ownerBank, DateTime lastServiceDate, decimal maxWithdrawPerOperation, decimal feePercent) : this(atmId, address, cashAvailable, isOnline, ownerBank)
        {
            LastServiceDate = lastServiceDate;
            _maxWithdrawPerOperation = maxWithdrawPerOperation;
            _feePercent = feePercent;
        }

        public AutomatedTellerMachine(AtmSettings settings, IBank ownerBank, 
            IAuthenticationService authService,
            IWithdrawService withdrawService,
            IDepositService depositService,
            ITransferService transferService,
            ITransactionService transactionService,
            IAtmEventPublisher atmEventPublisher)
        {
            OwnerBank = ownerBank ?? throw new ArgumentNullException(nameof(ownerBank));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _withdrawService = withdrawService ?? throw new ArgumentNullException(nameof(withdrawService));
            _depositService = depositService ?? throw new ArgumentNullException(nameof(depositService));
            _transferService = transferService ?? throw new ArgumentNullException(nameof(transferService));
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _atmEventsPublichser = atmEventPublisher ?? throw new ArgumentNullException(nameof(atmEventPublisher));

            AtmId = settings.AtmId;
            Address = settings.Address;
            CashAvailable = settings.CashAvailable;
            _isOnline = settings.IsOnline;
            _maxWithdrawPerOperation = settings.MaxWithdrawPerOperation;
            _feePercent = settings.FeePercent;
            AtmJournal = new List<Transaction>();
            CurrentAccount = null;
            State = _isOnline ? AtmState.Idle : AtmState.OutOfService;
            LastServiceDate = DateTime.Now;
        }

        public void Logout()
        {
            CurrentAccount = null;
            State = _isOnline ? AtmState.Idle : AtmState.OutOfService;
        }

        public bool Authenticate(string cardNumber, string pin)
        {
            if (!_isOnline)
            {
                State = AtmState.OutOfService;
                _atmEventsPublichser.PublishAuthenticated(this, new AuthenticatedEventArgs
                {
                    CardNumber = cardNumber ?? "NULL",
                    Success = false,
                    Message = "Банкомат не активний."
                });
                return false;
            }

            if (string.IsNullOrWhiteSpace(cardNumber) || string.IsNullOrWhiteSpace(pin))
            {
                State = AtmState.Idle;
                _atmEventsPublichser.PublishAuthenticated(this, new AuthenticatedEventArgs
                {
                    CardNumber = cardNumber ?? "NULL",
                    Success = false,
                    Message = "Некоректні дані для авторизації."
                });
                return false;
            }

            var account = OwnerBank.FindAccount(cardNumber);

            if (account == null)
            {
                State = AtmState.Idle;
                _atmEventsPublichser.PublishAuthenticated(this, new AuthenticatedEventArgs
                {
                    CardNumber = cardNumber,
                    Success = false,
                    Message = "Картку не знайдено."
                });
                return false;
            }

            if (account.Status == AccountStatus.Blocked)
            {
                State = AtmState.Idle;
                _atmEventsPublichser.PublishAuthenticated(this, new AuthenticatedEventArgs
                {
                    CardNumber = cardNumber,
                    Success = false,
                    Message = "Картка заблокована. Зверніться до відділення банку."
                });
                return false;
            }

            if (account.Status == AccountStatus.Expired)
            {
                State = AtmState.Idle;
                _atmEventsPublichser.PublishAuthenticated(this, new AuthenticatedEventArgs
                {
                    CardNumber = cardNumber,
                    Success = false,
                    Message = "Термін дії картки закінчився."
                });
                return false;
            }

            bool ok = _authService.Authenticate(account, pin);

            if (ok)
            {
                CurrentAccount = account;
                State = AtmState.Authenticated;
                _atmEventsPublichser.PublishAuthenticated(this, new AuthenticatedEventArgs
                {
                    CardNumber = cardNumber,
                    Success = true,
                    Message = "PIN код успішно підтверджено."
                });
                return true;
            }
            else
            {
                State = AtmState.Idle;
                string message = account.Status == AccountStatus.Blocked
                    ? "Картку заблоковано через 3 невірні спроби введення PIN-коду."
                    : "Некоректний PIN код.";

                _atmEventsPublichser.PublishAuthenticated(this, new AuthenticatedEventArgs
                {
                    CardNumber = cardNumber,
                    Success = false,
                    Message = message
                });
                return false;
            }
        }

        public void CheckBalance()
        {
            if (CurrentAccount == null || State != AtmState.Authenticated)
            {
                _atmEventsPublichser.PublishBalanceChecked(this, new BalanceCheckedEventArgs
                {
                    Account = CurrentAccount,
                    Balance = 0,
                    Message = "Користувач не авторизований."
                });
                return;
            }

            var transaction = new Transaction(
                TransactionType.CheckBalance,
                0,
                CurrentAccount.CardNumber ?? "0000000000000000",
                CurrentAccount.CardNumber ?? "0000000000000000",
                DateTime.Now,
                0
            );
            _transactionService.RecordTransaction(CurrentAccount, transaction);
            AtmJournal.Add(transaction);
            OwnerBank.BackLedger?.Add(transaction);

            _atmEventsPublichser.PublishBalanceChecked(this, new BalanceCheckedEventArgs
            {
                Account = CurrentAccount,
                Balance = CurrentAccount.Balance,
                Message = "Баланс успішно отримано."
            });
        }

        public void Withdraw(decimal amount)
        {
            if (CurrentAccount == null || State != AtmState.Authenticated)
            {
                _atmEventsPublichser.PublishWithdrawCompleted(this, new WithdrawCompletedEventArgs
                {
                    Account = CurrentAccount,
                    Amount = amount,
                    Success = false,
                    Message = "Користувач не авторизований."
                });
                return;
            }

            if (!_isOnline)
            {
                State = AtmState.OutOfService;
                _atmEventsPublichser.PublishWithdrawCompleted(this, new WithdrawCompletedEventArgs
                {
                    Account = CurrentAccount,
                    Amount = amount,
                    Success = false,
                    Message = "Банкомат не активний."
                });
                return;
            }

            if (CurrentAccount.Status != AccountStatus.Active)
            {
                _atmEventsPublichser.PublishWithdrawCompleted(this, new WithdrawCompletedEventArgs
                {
                    Account = CurrentAccount,
                    Amount = amount,
                    Success = false,
                    Message = "Картка не активна."
                });
                return;
            }

            if (amount <= 0)
            {
                _atmEventsPublichser.PublishWithdrawCompleted(this, new WithdrawCompletedEventArgs
                {
                    Account = CurrentAccount,
                    Amount = amount,
                    Success = false,
                    Message = "Сума повинна бути більше нуля."
                });
                return;
            }

            if (amount > _maxWithdrawPerOperation)
            {
                _atmEventsPublichser.PublishWithdrawCompleted(this, new WithdrawCompletedEventArgs
                {
                    Account = CurrentAccount,
                    Amount = amount,
                    Success = false,
                    Message = $"Сума перевищує ліміт операції ({_maxWithdrawPerOperation})."
                });
                return;
            }

            if (!_withdrawService.CanWithdraw(CurrentAccount, amount, CashAvailable))
            {
                string message = "Неможливо зняти цю суму. Перевірте баланс та ліміти.";
                if (amount > CashAvailable)
                    message = "У банкоматі недостатньо готівки.";
                else if (amount > CurrentAccount.Balance)
                    message = "Недостатньо коштів на рахунку.";
                else if (CurrentAccount.WithdrawnToday + amount > CurrentAccount.DailyWithdrawLimit)
                    message = "Перевищено денний ліміт зняття коштів.";

                _atmEventsPublichser.PublishWithdrawCompleted(this, new WithdrawCompletedEventArgs
                {
                    Account = CurrentAccount,
                    Amount = amount,
                    Success = false,
                    Message = message
                });
                return;
            }

            try
            {
                decimal fee = _feePercent > 0 ? Math.Round(amount * (_feePercent / 100m), 2) : 0m;
                _withdrawService.ProcessWithdraw(CurrentAccount, amount, fee);
                CashAvailable -= amount;

                var transaction = new Transaction(
                    TransactionType.Withdraw,
                    amount,
                    CurrentAccount.CardNumber ?? "0000000000000000",
                    CurrentAccount.CardNumber ?? "0000000000000000",
                    DateTime.Now,
                    fee
                );
                _transactionService.RecordTransaction(CurrentAccount, transaction);
                AtmJournal.Add(transaction);
                OwnerBank.BackLedger?.Add(transaction);

                _atmEventsPublichser.PublishWithdrawCompleted(this, new WithdrawCompletedEventArgs
                {
                    Account = CurrentAccount,
                    Amount = amount,
                    Success = true,
                    Message = "Грошi успішно знято."
                });
            }
            catch (Exception ex)
            {
                _atmEventsPublichser.PublishWithdrawCompleted(this, new WithdrawCompletedEventArgs
                {
                    Account = CurrentAccount,
                    Amount = amount,
                    Success = false,
                    Message = $"Помилка при зняттi: {ex.Message}"
                });
            }
        }

        public void Deposit(decimal amount)
        {
            if (CurrentAccount == null || State != AtmState.Authenticated)
            {
                _atmEventsPublichser.PublishDepositCompleted(this, new DepositCompletedEventArgs
                {
                    Account = CurrentAccount,
                    Amount = amount,
                    Success = false,
                    Message = "Користувач не авторизований."
                });
                return;
            }

            if (!_isOnline)
            {
                State = AtmState.OutOfService;
                _atmEventsPublichser.PublishDepositCompleted(this, new DepositCompletedEventArgs
                {
                    Account = CurrentAccount,
                    Amount = amount,
                    Success = false,
                    Message = "Банкомат не активний."
                });
                return;
            }

            if (CurrentAccount.Status != AccountStatus.Active)
            {
                _atmEventsPublichser.PublishDepositCompleted(this, new DepositCompletedEventArgs
                {
                    Account = CurrentAccount,
                    Amount = amount,
                    Success = false,
                    Message = "Картка не активна."
                });
                return;
            }

            if (amount <= 0 || !_depositService.CanDeposit(amount))
            {
                _atmEventsPublichser.PublishDepositCompleted(this, new DepositCompletedEventArgs
                {
                    Account = CurrentAccount,
                    Amount = amount,
                    Success = false,
                    Message = "Сума повинна бути більше нуля."
                });
                return;
            }

            try
            {
                _depositService.ProcessDeposit(CurrentAccount, amount);
                CashAvailable += amount;

                var transaction = new Transaction(
                    TransactionType.Deposit,
                    amount,
                    CurrentAccount.CardNumber ?? "0000000000000000",
                    CurrentAccount.CardNumber ?? "0000000000000000",
                    DateTime.Now,
                    0
                );
                _transactionService.RecordTransaction(CurrentAccount, transaction);
                AtmJournal.Add(transaction);
                OwnerBank.BackLedger?.Add(transaction);

                _atmEventsPublichser.PublishDepositCompleted(this, new DepositCompletedEventArgs
                {
                    Account = CurrentAccount,
                    Amount = amount,
                    Success = true,
                    Message = "Грошi успішно поповнено."
                });
            }
            catch (Exception ex)
            {
                _atmEventsPublichser.PublishDepositCompleted(this, new DepositCompletedEventArgs
                {
                    Account = CurrentAccount,
                    Amount = amount,
                    Success = false,
                    Message = $"Помилка при поповненні: {ex.Message}"
                });
            }
        }

        public void Transfer(string destinationCardNumber, decimal amount)
        {
            if (CurrentAccount == null || State != AtmState.Authenticated)
            {
                _atmEventsPublichser.PublishTransferCompleted(this, new TransferCompletedEventArgs
                {
                    FromAccount = CurrentAccount,
                    ToAccount = null,
                    Amount = amount,
                    Success = false,
                    Message = "Користувач не авторизований."
                });
                return;
            }

            if (!_isOnline)
            {
                State = AtmState.OutOfService;
                _atmEventsPublichser.PublishTransferCompleted(this, new TransferCompletedEventArgs
                {
                    FromAccount = CurrentAccount,
                    ToAccount = null,
                    Amount = amount,
                    Success = false,
                    Message = "Банкомат не активний."
                });
                return;
            }

            if (CurrentAccount.Status != AccountStatus.Active)
            {
                _atmEventsPublichser.PublishTransferCompleted(this, new TransferCompletedEventArgs
                {
                    FromAccount = CurrentAccount,
                    ToAccount = null,
                    Amount = amount,
                    Success = false,
                    Message = "Картка не активна."
                });
                return;
            }

            if (amount <= 0)
            {
                _atmEventsPublichser.PublishTransferCompleted(this, new TransferCompletedEventArgs
                {
                    FromAccount = CurrentAccount,
                    ToAccount = null,
                    Amount = amount,
                    Success = false,
                    Message = "Сума повинна бути більше нуля."
                });
                return;
            }

            if (string.IsNullOrWhiteSpace(destinationCardNumber))
            {
                _atmEventsPublichser.PublishTransferCompleted(this, new TransferCompletedEventArgs
                {
                    FromAccount = CurrentAccount,
                    ToAccount = null,
                    Amount = amount,
                    Success = false,
                    Message = "Номер картки одержувача не може бути порожнім."
                });
                return;
            }

            var toAccount = OwnerBank.FindAccount(destinationCardNumber);
            if (toAccount == null)
            {
                _atmEventsPublichser.PublishTransferCompleted(this, new TransferCompletedEventArgs
                {
                    FromAccount = CurrentAccount,
                    ToAccount = null,
                    Amount = amount,
                    Success = false,
                    Message = "Рахунок одержувача не знайдено."
                });
                return;
            }

            if (ReferenceEquals(CurrentAccount, toAccount) || (!string.IsNullOrEmpty(CurrentAccount.CardNumber) && CurrentAccount.CardNumber == toAccount.CardNumber))
            {
                _atmEventsPublichser.PublishTransferCompleted(this, new TransferCompletedEventArgs
                {
                    FromAccount = CurrentAccount,
                    ToAccount = toAccount,
                    Amount = amount,
                    Success = false,
                    Message = "Неможливо здійснити переказ на власну картку."
                });
                return;
            }

            if (toAccount.Status != AccountStatus.Active)
            {
                _atmEventsPublichser.PublishTransferCompleted(this, new TransferCompletedEventArgs
                {
                    FromAccount = CurrentAccount,
                    ToAccount = toAccount,
                    Amount = amount,
                    Success = false,
                    Message = "Картка одержувача не активна."
                });
                return;
            }

            decimal fee = _feePercent > 0 ? Math.Round(amount * (_feePercent / 100m), 2) : 0m;
            if (!_transferService.CanTransfer(CurrentAccount, toAccount, amount, fee))
            {
                _atmEventsPublichser.PublishTransferCompleted(this, new TransferCompletedEventArgs
                {
                    FromAccount = CurrentAccount,
                    ToAccount = toAccount,
                    Amount = amount,
                    Success = false,
                    Message = "Неможливо здійснити переказ. Перевірте баланс."
                });
                return;
            }

            try
            {
                _transferService.ProcessTransfer(CurrentAccount, toAccount, amount, fee);

                var transaction = new Transaction(
                    TransactionType.Transfer,
                    amount,
                    CurrentAccount.CardNumber ?? "0000000000000000",
                    toAccount.CardNumber ?? "0000000000000000",
                    DateTime.Now,
                    fee
                );
                _transactionService.RecordTransaction(CurrentAccount, transaction);
                _transactionService.RecordTransaction(toAccount, transaction);
                AtmJournal.Add(transaction);
                OwnerBank.BackLedger?.Add(transaction);

                _atmEventsPublichser.PublishTransferCompleted(this, new TransferCompletedEventArgs
                {
                    FromAccount = CurrentAccount,
                    ToAccount = toAccount,
                    Amount = amount,
                    Success = true,
                    Message = $"Переказ успішно здійснено на {toAccount.CardNumber}."
                });
            }
            catch (Exception ex)
            {
                _atmEventsPublichser.PublishTransferCompleted(this, new TransferCompletedEventArgs
                {
                    FromAccount = CurrentAccount,
                    ToAccount = toAccount,
                    Amount = amount,
                    Success = false,
                    Message = $"Помилка при переказі: {ex.Message}"
                });
            }
        }
    }
}
