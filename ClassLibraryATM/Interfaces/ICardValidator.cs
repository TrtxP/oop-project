namespace ClassLibraryATM.Interfaces
{
    public interface ICardValidator
    {
        bool IsValid(string cardNumber);
        string GetValidationError(string cardNumber);
    }
}
