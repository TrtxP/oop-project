using ClassLibraryATM.Classes;
using ClassLibraryATM.Repositories;
using Xunit;

namespace ATMTests.RepositoriesTests
{
    public class AccountRepositoryTests
    {
        private readonly AccountRepository _repository;
        private readonly Account _testAccount;

        public AccountRepositoryTests()
        {
            _repository = new AccountRepository();
            _testAccount = new Account("1234 5678 9012 3456", "Test User", 1000m, "1234");
        }

        [Fact]
        public void Add_ValidAccount_AddsAccount()
        {
            // Act
            _repository.Add(_testAccount);

            // Assert
            Assert.NotNull(_repository.FindByCardNumber("1234 5678 9012 3456"));
        }

        [Fact]
        public void Add_NullAccount_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _repository.Add(null!));
        }

        [Fact]
        public void Add_DuplicateAccount_ThrowsException()
        {
            // Arrange
            _repository.Add(_testAccount);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _repository.Add(_testAccount));
        }

        [Fact]
        public void FindByCardNumber_ExistingAccount_ReturnsAccount()
        {
            // Arrange
            _repository.Add(_testAccount);

            // Act
            var result = _repository.FindByCardNumber("1234 5678 9012 3456");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testAccount.OwnerFullName, result.OwnerFullName);
        }

        [Fact]
        public void FindByCardNumber_NonExistentAccount_ReturnsNull()
        {
            // Act
            var result = _repository.FindByCardNumber("9999 9999 9999 9999");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void FindByCardNumber_NullCardNumber_ReturnsNull()
        {
            // Act
            var result = _repository.FindByCardNumber(null!);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAll_MultipleAccounts_ReturnsAll()
        {
            // Arrange
            var account1 = new Account("1111 1111 1111 1111", "User One", 100m, "1111");
            var account2 = new Account("2222 2222 2222 2222", "User Two", 200m, "2222");
            _repository.Add(account1);
            _repository.Add(account2);

            // Act
            var result = _repository.GetAll();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void Exists_ExistingAccount_ReturnsTrue()
        {
            // Arrange
            _repository.Add(_testAccount);

            // Act
            bool result = _repository.Exists("1234 5678 9012 3456");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Exists_NonExistentAccount_ReturnsFalse()
        {
            // Act
            bool result = _repository.Exists("9999 9999 9999 9999");

            // Assert
            Assert.False(result);
        }
    }

    public class BankRepositoryTests
    {
        private readonly BankRepository _repository;
        private readonly Bank _testBank;

        public BankRepositoryTests()
        {
            _repository = new BankRepository();
            _testBank = new Bank("Test Bank");
        }

        [Fact]
        public void SaveBank_ValidBank_SavesBank()
        {
            // Act
            _repository.SaveBank(_testBank);

            // Assert
            Assert.NotNull(_repository.GetBank("Test Bank"));
        }

        [Fact]
        public void SaveBank_NullBank_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _repository.SaveBank(null!));
        }

        [Fact]
        public void GetBank_ExistingBank_ReturnsBank()
        {
            // Arrange
            _repository.SaveBank(_testBank);

            // Act
            var result = _repository.GetBank("Test Bank");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Bank", result.Name);
        }

        [Fact]
        public void GetBank_NonExistentBank_ReturnsNull()
        {
            // Act
            var result = _repository.GetBank("Non-Existent Bank");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAllBanks_MultipleBanks_ReturnsAll()
        {
            // Arrange
            var bank1 = new Bank("Bank One");
            var bank2 = new Bank("Bank Two");
            _repository.SaveBank(bank1);
            _repository.SaveBank(bank2);

            // Act
            var result = _repository.GetAllBanks();

            // Assert
            Assert.Equal(2, result.Count);
        }
    }
}
