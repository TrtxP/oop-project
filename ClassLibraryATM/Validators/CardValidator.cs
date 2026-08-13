using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Validators
{
    public class CardValidator : ICardValidator
    {
        public bool IsValid(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return false;

            string cleanedCard = cardNumber.Replace(" ", "");

            return cleanedCard.Length == 16 && cleanedCard.All(char.IsDigit);
        }

        public string GetValidationError(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return "Номер картки не може бути порожній.";

            string cleanedCard = cardNumber.Replace(" ", "");

            if (cleanedCard.Length != 16)
                return $"Номер картки повинен мати 16 цифр, отримано: {cleanedCard.Length}";

            if (!cleanedCard.All(char.IsDigit))
                return "Номер картки повинен містити тільки цифри.";

            return string.Empty;
        }
    }
}
