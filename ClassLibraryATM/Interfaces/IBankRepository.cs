namespace ClassLibraryATM.Interfaces
{
    public interface IBankRepository
    {
        void SaveBank(IBank bank);
        IBank? GetBank(string bankName);
        List<IBank> GetAllBanks();
    }
}
