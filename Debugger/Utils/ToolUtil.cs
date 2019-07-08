using System.Reflection;

namespace ModTools.Utils
{
    internal static class ToolUtil
    {
        public static void AddTool<T>(this ToolController toolController)
            where T : ToolBase
        {
            if (toolController.GetComponent<T>() != null)
            {
                return;
            }

            toolController.gameObject.AddComponent<T>();

            var toolControllerField = typeof(ToolController).GetField("m_tools", BindingFlags.Instance | BindingFlags.NonPublic);
            toolControllerField?.SetValue(toolController, toolController.GetComponents<ToolBase>());
            var toolModifierDictionary = typeof(ToolsModifierControl).GetField("m_Tools", BindingFlags.Static | BindingFlags.NonPublic);
            toolModifierDictionary?.SetValue(null, null); // to force a refresh
            ToolsModifierControl.SetTool<DefaultTool>();
        }
    }
}