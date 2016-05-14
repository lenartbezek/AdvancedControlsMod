using UnityEngine;
using spaar.ModLoader.UI;
using AdvancedControls.Controls;

namespace AdvancedControls.UI
{
    public class ControlMapper : MonoBehaviour
    {
        public new string name { get { return "Control Mapper window"; } }

        public bool Visible { get; set; } = false;

        private int windowID = spaar.ModLoader.Util.GetWindowID();
        private Rect windowRect = new Rect(100, 100, 100, 100);

        private float DesiredWidth { get; } = 320;
        private float DesiredHeight { get; } = 100;

        private GenericBlock block;

        public void ShowBlockControls(GenericBlock block)
        { 
            Visible = true;
            this.block = block;
        }

        /// <summary>
        /// Render window.
        /// </summary>
        private void OnGUI()
        {
            if (Visible && block != null)
            {
                GUI.skin = Util.Skin;
                windowRect = GUILayout.Window(windowID, windowRect, DoWindow, block.MyBlockInfo.blockName.ToUpper(),
                    GUILayout.Width(DesiredWidth),
                    GUILayout.Height(DesiredHeight));
            }
        }

        private void DoWindow(int id)
        {
            var controls = ControlManager.GetBlockControls(block.GetBlockID(), block.Guid);

            foreach(Control c in controls)
            {
                c.Draw();
                GUILayout.Label(" ");
            }

            if (controls.Count == 0)
                GUILayout.Label("This block has no available controls.");

            // Draw close button
            if (GUI.Button(new Rect(windowRect.width - 28, 8, 20, 20),
                "×", Elements.Buttons.Red))
                Visible = false;

            // Drag window
            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }
    }
}
