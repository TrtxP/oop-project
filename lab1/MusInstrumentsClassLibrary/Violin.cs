namespace MusInstrumentsClassLibrary
{
    public class Violin : MusicalInstrument
    {

        public Violin() 
        {
            SoundText = "Чарующий звук.";
            Name = "Скрипка.";
            Description = "Скри́пка — струнний музичний смичковий інструмент.";
            MusHistory = "Скрипка з'явилася у Північній Італії в середині XVI століття.";
        }

        public Violin(string soundText, string name, string musHistory) 
        {
            SoundText = soundText;
            Name = name;
            Description = "Струнний музичний інструмент.";
            MusHistory = musHistory;
        }

        public Violin(string sound, string name, string description, string history) 
        {
            SoundText = sound;
            Name = name;
            Description = description;
            MusHistory = history;
        }

        public Violin(Violin other) 
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
