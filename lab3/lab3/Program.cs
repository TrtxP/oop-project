using System.Text;

class Program
{
    static void Main()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("----------------------------------------------------------");
        Console.WriteLine("Лабораторну роботу №3\nВиконав:\ncтудент: Черепанов І.І.\nгрупа: ЗІПЗ-24-1");
        Console.WriteLine("----------------------------------------------------------");

        while (true)
        {
            Console.Write("Введіть рядок: ");
            string? str = Console.ReadLine();

            if (string.IsNullOrEmpty(str))
            {
                Console.WriteLine("Рядок не має бути пустим.");
                continue;
            }

            char c;

            Console.WriteLine("Інвертований рядок: " + ClassLibraryStr.String.StringInvert(str));

            Console.Write("Введіть символ для пірахунку: ");
            string? inputChar = Console.ReadLine();
            if (string.IsNullOrEmpty(inputChar) || inputChar.Length != 1)
            {
                Console.WriteLine("Введіть один символ.");
                continue;
            }
            c = inputChar[0];

            Console.WriteLine($"Кількість символів '{c}' у рядка " + $"'{str}': " + ClassLibraryStr.String.CountChars(str, c));
            Console.WriteLine("----------------------------------------------------------");
            break;
        }
    }
}