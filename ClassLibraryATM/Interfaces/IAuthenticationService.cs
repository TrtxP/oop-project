namespace ClassLibraryATM.Interfaces
{
    public interface IAuthenticationService
    {
        bool Authenticate(IAccount account, string pin);
        void ResetFailedAttempts(IAccount account);
    }
}
