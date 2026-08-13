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

            if (!_pinValidator.IsValid(pin))
                return false;

            return account.VerifyPin(pin);
        }

        public void ResetFailedAttempts(IAccount account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            // This method can be used to reset failed attempts if needed
            // Implementation depends on Account's internal state management
        }
    }
}
