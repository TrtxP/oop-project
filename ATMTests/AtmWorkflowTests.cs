using ClassLibraryATM.Builders;
using ClassLibraryATM.Classes;
using ClassLibraryATM.Enums;
using ClassLibraryATM.Events;
using ClassLibraryATM.Interfaces;
using ClassLibraryATM.Repositories;
using ClassLibraryATM.Services;
using ClassLibraryATM.Validators;
using Xunit;

namespace ATMTests.AtmTests
{
    public class AtmWorkflowTests
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IBankRepository _bankRepository;
        private readonly IBank _bank;
        private readonly IAtm _atm;
        private readonly IAtmEventPublisher _eventPublisher;
        private readonly Account _account1;
        private readonly Account _account2;

        public AtmWorkflowTests()
        {
            _accountRepository = new AccountRepository();
            _bankRepository = new BankRepository();
            _bank = new Bank("Test Bank", _accountRepository);
            _bankRepository.SaveBank(_bank);

            _account1 = new AccountBuilder()
                .WithCardNumber("1111 2222 3333 4444")
                .WithOwnerFullName("Test Owner 1")
                .WithPinCode("1234")
                .WithBalance(5000m)
                .Build();

            _account2 = new AccountBuilder()
                .WithCardNumber("5555 6666 7777 8888")
                .WithOwnerFullName("Test Owner 2")
                .WithPinCode("5678")
                .WithBalance(1000m)
                .Build();

            _bank.RegisterAccount(_account1);
            _bank.RegisterAccount(_account2);

            _eventPublisher = new AtmEventPublisher();

            var settings = new AtmBuilder()
                .WithAtmId("ATM-01")
                .WithAddress("Test Street, 1")
                .WithCashAvailable(20000m)
                .WithMaxWithdrawPerOperation(10000m)
                .WithFeePercent(0m)
                .Build();

            _atm = new AutomatedTellerMachine(
                settings,
                _bank,
                new AuthenticationService(new PinValidator()),
                new WithdrawService(new AmountValidator()),
                new DepositService(new AmountValidator()),
                new TransferService(new AmountValidator()),
                new TransactionService(),
                _eventPublisher
            );
        }

        [Fact]
        public void InitialState_IsIdleAndCurrentAccountIsNull()
        {
            Assert.Equal(AtmState.Idle, _atm.State);
            Assert.Null(_atm.CurrentAccount);
        }

        [Fact]
        public void Authenticate_ValidCredentials_SetsStateAuthenticated()
        {
            bool success = _atm.Authenticate("1111 2222 3333 4444", "1234");

            Assert.True(success);
            Assert.Equal(AtmState.Authenticated, _atm.State);
            Assert.NotNull(_atm.CurrentAccount);
            Assert.Equal("1111 2222 3333 4444", _atm.CurrentAccount.CardNumber);
        }

        [Fact]
        public void Authenticate_WrongPin_FailsAndRemainsIdle()
        {
            bool success = _atm.Authenticate("1111 2222 3333 4444", "9999");

            Assert.False(success);
            Assert.Equal(AtmState.Idle, _atm.State);
            Assert.Null(_atm.CurrentAccount);
        }

        [Fact]
        public void Logout_ClearsCurrentAccountAndSetsIdleState()
        {
            _atm.Authenticate("1111 2222 3333 4444", "1234");
            _atm.Logout();

            Assert.Equal(AtmState.Idle, _atm.State);
            Assert.Null(_atm.CurrentAccount);
        }

        [Fact]
        public void Withdraw_WhenAuthenticated_DecreasesBalanceAndAtmCash()
        {
            _atm.Authenticate("1111 2222 3333 4444", "1234");
            decimal initialAtmCash = _atm.CashAvailable;

            _atm.Withdraw(1000m);

            Assert.Equal(4000m, _account1.Balance);
            Assert.Equal(initialAtmCash - 1000m, _atm.CashAvailable);
            Assert.Single(_atm.AtmJournal);
            Assert.Single(_account1.History);
        }

        [Fact]
        public void Deposit_WhenAuthenticated_IncreasesBalanceAndAtmCash()
        {
            _atm.Authenticate("1111 2222 3333 4444", "1234");
            decimal initialAtmCash = _atm.CashAvailable;

            _atm.Deposit(500m);

            Assert.Equal(5500m, _account1.Balance);
            Assert.Equal(initialAtmCash + 500m, _atm.CashAvailable);
            Assert.Single(_atm.AtmJournal);
            Assert.Single(_account1.History);
        }

        [Fact]
        public void Transfer_WhenAuthenticated_TransfersBetweenAccounts()
        {
            _atm.Authenticate("1111 2222 3333 4444", "1234");

            _atm.Transfer("5555 6666 7777 8888", 2000m);

            Assert.Equal(3000m, _account1.Balance);
            Assert.Equal(3000m, _account2.Balance);
            Assert.Single(_atm.AtmJournal);
            Assert.Single(_account1.History);
            Assert.Single(_account2.History);
        }

        [Fact]
        public void CheckBalance_RaisesBalanceCheckedEvent()
        {
            _atm.Authenticate("1111 2222 3333 4444", "1234");

            decimal reportedBalance = 0;
            bool eventRaised = false;

            _eventPublisher.BalanceChecked += (sender, e) =>
            {
                eventRaised = true;
                reportedBalance = e.Balance;
            };

            _atm.CheckBalance();

            Assert.True(eventRaised);
            Assert.Equal(5000m, reportedBalance);
        }
    }
}
