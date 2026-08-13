namespace ClassLibraryATM.Interfaces
{
    public interface IAmountValidator
    {
        bool IsValid(decimal amount);
        string GetValidationError(decimal amount);
    }
}
