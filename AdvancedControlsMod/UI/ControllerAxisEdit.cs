using UnityEngine;
using spaar.ModLoader.UI;

namespace AdvancedControlsMod.UI
{
    public class ControllerAxisEdit : MonoBehaviour
    {
        public bool Visible { get; set; } = true;

        public new string name { get { return "Edit Controller Axis window"; } }

        private int windowID = spaar.ModLoader.Util.GetWindowID();
        private Rect windowRect = new Rect(100, 100, 320, 610);

        private ControllerAxis axis = new ControllerAxis("vertical");

        private bool vertical = true;

        private void SaveAxis()
        {
            Visible = false;
            AdvancedControls.Instance.SaveAxis(axis);
        }

        public void EditAxis(ControllerAxis axis)
        {
            Visible = true;
            this.axis = axis;
        }

        /// <summary>
        /// Render window.
        /// </summary>
        private void OnGUI()
        {
            if (Visible)
            {
                GUI.skin = Util.Skin;
                windowRect = GUILayout.Window(windowID, windowRect, DoWindow, "Edit Controller Axis");
            }
        }

        private void DoWindow(int id)
        {
            // Draw save text field and save button
            GUILayout.BeginHorizontal();
            axis.Name = GUILayout.TextField(axis.Name,
                Elements.InputFields.Default);

            if (GUILayout.Button("Save",
                Elements.Buttons.Default,
                GUILayout.Width(80))
                && axis.Name != "")
            {
                SaveAxis();
            }
            GUILayout.EndHorizontal();

            // Draw graph
            Rect graphRect = new Rect(
                GUI.skin.window.padding.left,
                GUI.skin.window.padding.top + 36,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right);

            Util.DrawRect(new Rect(graphRect.x + graphRect.width / 2 + graphRect.width / 2 * axis.Input,
                              graphRect.y,
                              1,
                              graphRect.height),
                     Color.yellow);

            for(int i = 0; i <= graphRect.width; i++)
            {
                float value = axis.Process((i - graphRect.width / 2) / (graphRect.width / 2));
                Util.DrawPixel(new Vector2(graphRect.x + i, graphRect.y + graphRect.height / 2 - graphRect.height / 2 * value), Color.white);
            }

            // Draw axis controls
            GUILayout.BeginArea(new Rect(
                GUI.skin.window.padding.left,
                graphRect.y + graphRect.height + GUI.skin.window.padding.bottom,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                400));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Vertical",
                vertical ? Elements.Buttons.Default : Elements.Buttons.Disabled))
            {
                vertical = true;
            }

            if (GUILayout.Button("Horizontal",
                !vertical ? Elements.Buttons.Default : Elements.Buttons.Disabled))
            {
                vertical = false;
            }
            axis.Axis = vertical ? "Vertical" : "Horizontal";
            GUILayout.EndHorizontal();

            // Draw Sensitivity slider
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sensitivity ", Util.LabelStyle);
            GUILayout.Label((Mathf.Round(axis.Sensitivity * 100) / 100).ToString(),
                Util.LabelStyle,
                GUILayout.Width(60));
            GUILayout.EndHorizontal();

            axis.Sensitivity = GUILayout.HorizontalSlider(axis.Sensitivity, 0, 5);

            // Draw Curvature slider
            GUILayout.BeginHorizontal();
            GUILayout.Label("Curvature ", Util.LabelStyle);
            GUILayout.Label((Mathf.Round(axis.Curvature * 100) / 100).ToString(),
                Util.LabelStyle,
                GUILayout.Width(60));
            GUILayout.EndHorizontal();

            axis.Curvature = GUILayout.HorizontalSlider(axis.Curvature, 0, 3);
            
            // Draw Deadzone slider
            GUILayout.BeginHorizontal();
            GUILayout.Label("Deadzone ", Util.LabelStyle);
            GUILayout.Label((Mathf.Round(axis.Deadzone * 100) / 100).ToString(),
                Util.LabelStyle,
                GUILayout.Width(60));
            GUILayout.EndHorizontal();

            axis.Deadzone = GUILayout.HorizontalSlider(axis.Deadzone, 0, 0.5f);
            
            // Draw Invert toggle
            GUILayout.BeginHorizontal();
            axis.Invert = GUILayout.Toggle(axis.Invert, "",
                Util.ToggleStyle,
                GUILayout.Width(20),
                GUILayout.Height(20));

            GUILayout.Label("Invert ",
                new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            if (GUI.Button(new Rect(windowRect.width - 28, 8, 20, 20),
                "×", Elements.Buttons.Red))
                Visible = false;

            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }
    }
}
