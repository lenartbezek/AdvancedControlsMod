using UnityEngine;
using spaar.ModLoader.UI;

namespace AdvancedControls.UI
{
    public class ControlMapper : MonoBehaviour
    {
        public new string name { get { return "Control Mapper window"; } }

        public bool Visible { get; set; } = false;

        private int windowID = spaar.ModLoader.Util.GetWindowID();
        private Rect windowRect = new Rect(100, 100, 100, 100);

        private float DesiredWidth { get; } = 400;
        private float DesiredHeight { get; } = 100;

        private GenericBlock block;

        public void ShowBlockControls(GenericBlock block)
        { 
            Visible = true;
        }

        /// <summary>
        /// Render window.
        /// </summary>
        private void OnGUI()
        {
            if (Visible && block != null)
            {
                GUI.skin = Util.Skin;
                windowRect = GUILayout.Window(windowID, windowRect, DoWindow, block.MyBlockInfo.name,
                    GUILayout.Width(DesiredWidth),
                    GUILayout.Height(DesiredHeight));
            }
        }

        private void DoWindow(int id)
        {
            // Draw GUID Text field

        }
    }
}
