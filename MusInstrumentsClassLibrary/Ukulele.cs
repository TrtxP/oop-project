namespace MusInstrumentsClassLibrary
{
    public class Ukulele : MusicalInstrument
    {
        public Ukulele() 
        {
            SoundText = "Грайливий звук.";
            Name = "Укулеле.";
            Description = "Музичний інструмент, схожий на гітару.";
            MusHistory = "Зв'явилося на Гавайях у другій половині 19 ст. з Португалії.";
        }

        public Ukulele(string soundText, string name, string musHistory) 
        {
            SoundText = soundText;
            Name = name;
            Description = "Музичний інструмент, подібний до гітари.";
            MusHistory = musHistory;
        }

        public Ukulele(string soundText, string name, string description, string musHistory) 
        {
            SoundText = soundText;
            Name = name;
            Description = description;
            MusHistory = musHistory;
        }

        public Ukulele(Ukulele other) 
        {
            SoundText = other.SoundText;
            Name = other.Name;
            Description = other.Description;
            MusHistory = other.MusHistory;
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
