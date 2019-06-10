using ICities;

namespace ModTools
{
    public sealed class Mod : IUserMod
    {
        public Mod()
        {
            ModToolsBootstrap.Bootstrap();
        }

        public string Name => "ModTools";

        public string Description => "Debugging toolkit for modders";
    }
}