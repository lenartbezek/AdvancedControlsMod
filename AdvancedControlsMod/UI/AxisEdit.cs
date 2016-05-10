using UnityEngine;
using spaar.ModLoader.UI;

namespace AdvancedControls.UI
{
    public abstract class AxisEdit : MonoBehaviour
    {
        public new string name { get { return "Edit Axis window"; } }

        public bool Visible { get; set; } = false;

        protected int windowID = spaar.ModLoader.Util.GetWindowID();
        protected Rect windowRect = new Rect(100, 100, 100, 100);

        protected virtual float DesiredWidth { get; } = 100;
        protected virtual float DesiredHeight { get; } = 100;

        protected string axisName = "";
        protected Axes.Axis axis;

        public abstract void SaveAxis();

        public abstract void EditAxis(Axes.Axis axis);

        /// <summary>
        /// Render window.
        /// </summary>
        protected virtual void OnGUI()
        {
            if (Visible)
            {
                GUI.skin = Util.Skin;
                windowRect = GUILayout.Window(windowID, windowRect, DoWindow, "Edit " + axisName,
                    GUILayout.Width(DesiredWidth),
                    GUILayout.Height(DesiredHeight));
            }
        }

        protected virtual void DoWindow(int id)
        {
            // Draw save text field and save button
            GUILayout.BeginHorizontal();
            axisName = GUILayout.TextField(axisName,
                Elements.InputFields.Default);

            if (GUILayout.Button("Save",
                Elements.Buttons.Default,
                GUILayout.Width(80))
                && axisName != "")
            {
                SaveAxis();
            }
            GUILayout.EndHorizontal();

            // Draw close button
            if (GUI.Button(new Rect(windowRect.width - 28, 8, 20, 20),
                "×", Elements.Buttons.Red))
                Visible = false;

            // Drag window
            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }
    }
}
