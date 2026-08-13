namespace MusInstrumentsClassLibrary
{
    public class Cello : MusicalInstrument
    {
        public Cello()
        {
            SoundText = "Чаруючий звук, схожий на звук скрипки.";
            Name = "Віолончель.";
            Description = "Струнний смичковий музичний інструмент.";
            MusHistory = "З'явилася в Італії в 16 ст. від Віоли Да Гамба.";
        }

        public Cello(string soundText, string name, string musHistory)
        {
            SoundText = soundText;
            Name = name;
            Description = "Струнний музичний інструмент.";
            MusHistory = musHistory;
        }

        public Cello(string soundText, string name, string description, string musHistory)
        {
            SoundText = soundText;
            Name = name;
            Description = description;
            MusHistory = musHistory;
        }

        public Cello(Cello other) 
        {
            this.SoundText = other.SoundText;
            this.Name = other.Name;
            this.Description = other.Description;
            this.MusHistory = other.MusHistory;
        }

        public override void Sound()
        {
            Console.WriteLine($"{SoundText}");
        }

        public override void Show()
        {
            Console.WriteLine($"{Name}");
        }

        public override void Desc()
        {
            Console.WriteLine($"{Description}");
        }

        public override void History()
        {
            Console.WriteLine($"{MusHistory}");
        }

        public override void ShowInfo()
        {
            base.ShowInfo();
        }
    }
}
