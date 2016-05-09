using UnityEngine;
using spaar.ModLoader.UI;
using LenchScripter;

namespace AdvancedControlsMod.UI
{
    public class CustomAxisEdit : MonoBehaviour
    {
        public bool Visible { get; set; } = true;

        public new string name { get { return "Edit Custom Axis window"; } }

        private int windowID = spaar.ModLoader.Util.GetWindowID();
        private Rect windowRect = new Rect(100, 100, 100, 100);

        private CustomAxis axis = new CustomAxis();

        private Vector2 initialisationScrollPosition = Vector2.zero;
        private Vector2 updateScrollPosition = Vector2.zero;

        private void SaveAxis()
        {
            Visible = false;
            AdvancedControls.Instance.SaveAxis(axis);
        }

        public void EditAxis(CustomAxis axis)
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
                windowRect = GUILayout.Window(windowID, windowRect, DoWindow, "Edit Custom Axis",
                    GUILayout.Width(320),
                    GUILayout.Height(466));
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

            // Draw initialisation code text area
            GUILayout.Label("Initialisation code ",
                Util.LabelStyle);

            initialisationScrollPosition = GUILayout.BeginScrollView(initialisationScrollPosition,
                GUILayout.Height(100));
            axis.InitialisationCode = GUILayout.TextArea(axis.InitialisationCode);
            GUILayout.EndScrollView();

            // Draw update code text area
            GUILayout.Label("Update code ",
                Util.LabelStyle);

            updateScrollPosition = GUILayout.BeginScrollView(updateScrollPosition,
                GUILayout.Height(200));
            axis.UpdateCode = GUILayout.TextArea(axis.UpdateCode);
            GUILayout.EndScrollView();

            // Draw notes
            if (!Lua.Enabled)
                GUILayout.Label(
@"<color=#FFFF00>Note:</color>
Lua needs to be enabled in the settings menu.",
                    Util.LabelStyle);

            if (GUI.Button(new Rect(windowRect.width - 28, 8, 20, 20),
                "×", Elements.Buttons.Red))
                Visible = false;

            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }
    }
}