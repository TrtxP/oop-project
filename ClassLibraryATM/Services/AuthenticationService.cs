using ClassLibraryATM.Enums;
using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IPinValidator _pinValidator;

        public AuthenticationService(IPinValidator pinValidator)
        {
            _pinValidator = pinValidator ?? throw new ArgumentNullException(nameof(pinValidator));
        }

        public bool Authenticate(IAccount account, string pin)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (account.Status == AccountStatus.Blocked || account.Status == AccountStatus.Expired)
                return false;

            if (!_pinValidator.IsValid(pin))
            {
                account.VerifyPin(pin ?? string.Empty);
                return false;
            }

            return account.VerifyPin(pin);
        }

        public void ResetFailedAttempts(IAccount account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
        }
    }
}
