namespace ModTools.Scripting
{
    public interface IModEntryPoint
    {
        void OnModLoaded();

        void OnModUnloaded();
    }
}