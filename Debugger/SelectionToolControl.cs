using UnityEngine;

namespace ModTools
{
    public class SelectionToolControl : MonoBehaviour
    {

        public void Update()
        {
            if (!MainWindow.Instance.Config.SelectionTool)
            {
                return;
            }
            if ((!Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftControl)) || !Input.GetKeyDown(KeyCode.M))
            {
                return;
            }
            var tool = ToolsModifierControl.GetTool<SelectionTool>();
            if (tool == null)
            {
                return;
            }
            if (tool.enabled)
            {
                var defaultTool = ToolsModifierControl.GetTool<DefaultTool>();
                if (defaultTool != null)
                {
                    defaultTool.enabled = true;
                }
            }
            else
            {
                tool.enabled = true;
            }
        }

    }
}