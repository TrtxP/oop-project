namespace ClassLibraryATM.Delegates
{
    public delegate void AtmEventHandler<TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs;
}
