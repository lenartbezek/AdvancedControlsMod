using UnityEngine;
using spaar.ModLoader.UI;

namespace AdvancedControlsMod.UI
{
    public class ControllerAxisEdit : MonoBehaviour
    {
        public bool Visible { get; set; } = false;

        public new string name { get { return "Edit Controller Axis window"; } }

        private int windowID = spaar.ModLoader.Util.GetWindowID();
        private Rect windowRect = new Rect(100, 100, 320, 594);

        private string axisSaveName = "new axis";
        private ControllerAxis configuringAxis = new ControllerAxis("vertical");
        private bool vertical = true;

        private void SaveAxis()
        {
            Visible = false;
            AdvancedControls.Instance.SaveAxis(configuringAxis, axisSaveName);
        }

        public void EditAxis(ControllerAxis axis)
        {
            Visible = true;
            configuringAxis = axis;
        }

        /// <summary>
        /// Render window.
        /// </summary>
        private void OnGUI()
        {
            if (Visible)
            {
                GUI.skin = ModGUI.Skin;
                GUI.skin.window.padding.left = 8;
                windowRect = GUI.Window(windowID, windowRect, DoWindow, "Edit Controller Axis");
            }
        }

        private void DoWindow(int id)
        {

            // Draw save text field and button
            float vertical_offset = GUI.skin.window.padding.top;

            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f, 1);
            axisSaveName = GUI.TextField(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width * 0.75f - 2 * GUI.skin.window.padding.left, 20),
                axisSaveName, Elements.InputFields.ComponentField);
            GUI.backgroundColor = oldColor;

            if (GUI.Button(new Rect(
                windowRect.width * 0.75f - 0.5f * GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width * 0.25f - 0.5f * GUI.skin.window.padding.left, 20),
                "Save", Elements.Buttons.Default) && axisSaveName != "")
            {
                SaveAxis();
            }

            /// Draw graph
            vertical_offset += GUI.skin.window.padding.left + 20;

            Rect graphRect = new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width - 2 * GUI.skin.window.padding.left,
                windowRect.width - 2 * GUI.skin.window.padding.left);

            Util.DrawRect(new Rect(graphRect.x + graphRect.width / 2 + graphRect.width / 2 * configuringAxis.Input,
                              graphRect.y,
                              1,
                              graphRect.height),
                     Color.yellow);

            for(int i = 0; i <= graphRect.width; i++)
            {
                float value = configuringAxis.Process((i - graphRect.width/2) / (graphRect.width));
                Util.DrawPixel(new Vector2(graphRect.x + i, graphRect.y + graphRect.height / 2 - graphRect.height / 2 * value), Color.white);
            }

            // Draw axis toggles
            vertical_offset += GUI.skin.window.padding.left + windowRect.width - 2 * GUI.skin.window.padding.left;

            if (GUI.Button(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width * 0.5f - 1.5f * GUI.skin.window.padding.left, 30),
                "Vertical",
                vertical ? Elements.Buttons.Default : Elements.Buttons.Disabled))
            {
                vertical = true;
            }

            if (GUI.Button(new Rect(
                windowRect.width * 0.5f + 0.5f * GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width * 0.5f - 1.5f * GUI.skin.window.padding.left, 30),
                "Horizontal",
                !vertical ? Elements.Buttons.Default : Elements.Buttons.Disabled))
            {
                vertical = false;
            }

            configuringAxis.Axis = vertical ? "Vertical" : "Horizontal";


            // Draw Sensitivity slider
            vertical_offset += GUI.skin.window.padding.left + 30;

            GUI.Label(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width - 2f * GUI.skin.window.padding.left, 20),
                "Sensitivity: "+ configuringAxis.Sensitivity);

            vertical_offset += GUI.skin.window.padding.left + 8;

            configuringAxis.Sensitivity = GUI.HorizontalSlider(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width - 2f * GUI.skin.window.padding.left, 20),
                configuringAxis.Sensitivity, 0, 5);

            vertical_offset += GUI.skin.window.padding.left + 20;

            // Draw Curvature slider
            GUI.Label(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width - 2f * GUI.skin.window.padding.left, 20),
                "Curvature: "+ configuringAxis.Curvature);

            vertical_offset += GUI.skin.window.padding.left + 8;

            configuringAxis.Curvature = GUI.HorizontalSlider(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width - 2f * GUI.skin.window.padding.left, 20),
                configuringAxis.Curvature, 0, 3);

            vertical_offset += GUI.skin.window.padding.left + 20;

            // Draw Deadzone slider
            GUI.Label(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width - 2f * GUI.skin.window.padding.left, 20),
                "Deadzone: "+ configuringAxis.Deadzone);

            vertical_offset += GUI.skin.window.padding.left + 8;

            configuringAxis.Deadzone = GUI.HorizontalSlider(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width - 2f * GUI.skin.window.padding.left, 20),
                configuringAxis.Deadzone, 0, 0.5f);


            vertical_offset += GUI.skin.window.padding.left + 20;

            // Draw Invert toggle
            configuringAxis.Invert = GUI.Toggle(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                20, 20),
                configuringAxis.Invert, "");

            GUI.Label(new Rect(
                GUI.skin.window.padding.left + 28,
                vertical_offset + 4,
                windowRect.width - 2f * GUI.skin.window.padding.left, 20),
                "Invert ");

            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }
    }
}
