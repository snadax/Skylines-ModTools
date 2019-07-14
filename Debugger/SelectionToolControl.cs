using ColossalFramework.UI;
using ModTools.Utils;
using UnityEngine;

namespace ModTools
{
    public class SelectionToolControl : MonoBehaviour
    {
        public void Awake()
        {
            var toolController = FindObjectOfType<ToolManager>().m_properties;
            if (toolController == null)
            {
                return;
            }

            toolController.AddTool<SelectionTool>();
            var selectionToolButton = SelectionToolButton.Create();
            selectionToolButton.eventClicked += (c, e) => ToggleTool();
        }

        public void Update()
        {
            if (!MainWindow.Instance.Config.SelectionTool)
            {
                return;
            }

            if ((!Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftControl)) ||
                !Input.GetKeyDown(KeyCode.M))
            {
                return;
            }

            ToggleTool();
        }

        private static void ToggleTool()
        {
            var tool = ToolsModifierControl.GetTool<SelectionTool>();
            if (tool == null)
            {
                return;
            }

            if (tool.enabled)
            {
                ToolsModifierControl.SetTool<DefaultTool>();
            }
            else
            {
                ToolsModifierControl.SetTool<SelectionTool>();
            }
        }
    }
}