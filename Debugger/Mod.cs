using System;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Plugins;
using ICities;
using UnityEngine;

namespace ModTools
{
    public class Mod : LoadingExtensionBase, IUserMod
    {

        public string Name
        {
            get { ModToolsBootstrap.Bootstrap(); return "ModTools"; }
        }

        public string Description
        {
            get { return "Debugging toolkit for modders"; }
        }
    }

}
