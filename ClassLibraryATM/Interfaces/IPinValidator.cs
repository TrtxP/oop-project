namespace ClassLibraryATM.Interfaces
{
    public interface IPinValidator
    {
        bool IsValid(string pin);
        string GetValidationError(string pin);
    }
}
