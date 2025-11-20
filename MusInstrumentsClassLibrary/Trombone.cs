namespace MusInstrumentsClassLibrary
{
    public class Trombone : MusicalInstrument
    {

        public Trombone() 
        {
            SoundText = "Гучний звук.";
            Name = "Тромбон.";
            Description = "Музичний інструмент сімейства мідних духових.";
            MusHistory = "Походить від сакбута, який був розроблений у 15 ст. в Італії.";
        }

        public Trombone(string soundText, string name, string musHistory) 
        {
            SoundText = soundText;
            Name = name;
            Description = "Музичний інструмент з сімейства мідних духових.";
            MusHistory = musHistory;
        }

        public Trombone(string soundText, string name, string description, string musHistory) 
        {
            SoundText = soundText;
            Name = name;
            Description = description;
            MusHistory = musHistory;
        }

        public Trombone(Trombone other) 
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
