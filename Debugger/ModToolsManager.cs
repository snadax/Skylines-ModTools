using ColossalFramework.IO;
using UnityEngine;

namespace ModTools
{
    public class ModToolsManager : ISimulationManager
    {
        public string GetName()
        {
            return "ModToolsManager";
        }

        public ThreadProfiler GetSimulationProfiler()
        {
            return null;
        }

        public void SimulationStep(int subStep)
        {
        }

        public void UpdateData(SimulationManager.UpdateMode mode)
        {
            ModToolsBootstrap.inMainMenu = false;
            ModToolsBootstrap.InitModTools(mode);
        }

        public void LateUpdateData(SimulationManager.UpdateMode mode)
        {
        }

        public void GetData(FastList<IDataContainer> data)
        {
        }
    }
}