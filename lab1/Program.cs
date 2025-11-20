using System.Text;
using MusInstrumentsClassLibrary;

class Program
{
    static void Main()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("-------------------------------------------------------------");
        Console.WriteLine("Лабораторна робота №1 \nВиконав: \nстудент: Черепанов І.І.\nгрупа: ЗІПЗ-24-1");
        Console.WriteLine("-------------------------------------------------------------\n");

        MusicalInstrument[] instruments = { new Violin(), new Trombone(), new Ukulele(), new Cello() };

        for (int i = 0; i < instruments.Length; i++)
        {
            instruments[i].ShowInfo();
            Console.WriteLine();
        }

        Console.WriteLine("-------------------------------------------------------------");
    }
}