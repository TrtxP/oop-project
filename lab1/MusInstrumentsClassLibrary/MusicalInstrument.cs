namespace MusInstrumentsClassLibrary
{
    public abstract class MusicalInstrument
    {
        protected string? SoundText { set; get; }
        protected string? Name { set; get; }
        protected string? Description { set; get; }
        protected string? MusHistory { set; get; }

        protected MusicalInstrument() { }

        public abstract void Sound();
        public abstract void Show();
        public abstract void Desc();
        public abstract void History();
        public virtual void ShowInfo()
        {
            Show();
            Sound();
            Desc();
            History();
        }
    }
}
