using ClassLibraryATM.Classes;
using ClassLibraryATM.Services;
using ClassLibraryATM.Validators;
using Xunit;

namespace ATMTests.ServicesTests
{
    public class WithdrawServiceTests
    {
        private readonly WithdrawService _withdrawService;
        private readonly Account _testAccount;

        public WithdrawServiceTests()
        {
            _withdrawService = new WithdrawService(new AmountValidator());
            _testAccount = new Account("1234 5678 9012 3456", "Test User", 1000m, "1234");
        }

        [Fact]
        public void CanWithdraw_ValidAmount_ReturnsTrue()
        {
            // Arrange
            decimal amount = 100m;
            decimal atmCash = 5000m;

            // Act
            bool result = _withdrawService.CanWithdraw(_testAccount, amount, atmCash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanWithdraw_InsufficientAccountBalance_ReturnsFalse()
        {
            // Arrange
            decimal amount = 2000m;
            decimal atmCash = 5000m;

            // Act
            bool result = _withdrawService.CanWithdraw(_testAccount, amount, atmCash);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanWithdraw_InsufficientAtmCash_ReturnsFalse()
        {
            // Arrange
            decimal amount = 100m;
            decimal atmCash = 50m;

            // Act
            bool result = _withdrawService.CanWithdraw(_testAccount, amount, atmCash);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ProcessWithdraw_ValidAmount_DecreasesBalance()
        {
            // Arrange
            decimal initialBalance = _testAccount.Balance;
            decimal amount = 100m;

            // Act
            _withdrawService.ProcessWithdraw(_testAccount, amount);

            // Assert
            Assert.Equal(initialBalance - amount, _testAccount.Balance);
        }

        [Fact]
        public void ProcessWithdraw_InvalidAmount_ThrowsException()
        {
            // Arrange
            decimal invalidAmount = -100m;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _withdrawService.ProcessWithdraw(_testAccount, invalidAmount));
        }
    }

    public class DepositServiceTests
    {
        private readonly DepositService _depositService;
        private readonly Account _testAccount;

        public DepositServiceTests()
        {
            _depositService = new DepositService(new AmountValidator());
            _testAccount = new Account("1234 5678 9012 3456", "Test User", 1000m, "1234");
        }

        [Fact]
        public void CanDeposit_ValidAmount_ReturnsTrue()
        {
            // Arrange
            decimal amount = 500m;

            // Act
            bool result = _depositService.CanDeposit(amount);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanDeposit_ZeroAmount_ReturnsFalse()
        {
            // Arrange
            decimal amount = 0m;

            // Act
            bool result = _depositService.CanDeposit(amount);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ProcessDeposit_ValidAmount_IncreasesBalance()
        {
            // Arrange
            decimal initialBalance = _testAccount.Balance;
            decimal amount = 500m;

            // Act
            _depositService.ProcessDeposit(_testAccount, amount);

            // Assert
            Assert.Equal(initialBalance + amount, _testAccount.Balance);
        }

        [Fact]
        public void ProcessDeposit_InvalidAmount_ThrowsException()
        {
            // Arrange
            decimal invalidAmount = -500m;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _depositService.ProcessDeposit(_testAccount, invalidAmount));
        }
    }

    public class TransferServiceTests
    {
        private readonly TransferService _transferService;
        private readonly Account _fromAccount;
        private readonly Account _toAccount;

        public TransferServiceTests()
        {
            _transferService = new TransferService(new AmountValidator());
            _fromAccount = new Account("1111 1111 1111 1111", "User One", 1000m, "1111");
            _toAccount = new Account("2222 2222 2222 2222", "User Two", 500m, "2222");
        }

        [Fact]
        public void CanTransfer_ValidTransfer_ReturnsTrue()
        {
            // Arrange
            decimal amount = 100m;

            // Act
            bool result = _transferService.CanTransfer(_fromAccount, _toAccount, amount);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanTransfer_InsufficientBalance_ReturnsFalse()
        {
            // Arrange
            decimal amount = 2000m;

            // Act
            bool result = _transferService.CanTransfer(_fromAccount, _toAccount, amount);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ProcessTransfer_ValidTransfer_TransfersAmount()
        {
            // Arrange
            decimal initialFromBalance = _fromAccount.Balance;
            decimal initialToBalance = _toAccount.Balance;
            decimal amount = 100m;

            // Act
            _transferService.ProcessTransfer(_fromAccount, _toAccount, amount);

            // Assert
            Assert.Equal(initialFromBalance - amount, _fromAccount.Balance);
            Assert.Equal(initialToBalance + amount, _toAccount.Balance);
        }

        [Fact]
        public void ProcessTransfer_InvalidAmount_ThrowsException()
        {
            // Arrange
            decimal invalidAmount = -100m;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _transferService.ProcessTransfer(_fromAccount, _toAccount, invalidAmount));
        }
    }
}
