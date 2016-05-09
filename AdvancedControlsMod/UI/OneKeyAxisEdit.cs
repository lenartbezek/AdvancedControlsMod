using System;
using UnityEngine;
using spaar.ModLoader.UI;

namespace AdvancedControlsMod.UI
{
    public class OneKeyAxisEdit : MonoBehaviour
    {
        public bool Visible { get; set; } = true;

        public new string name { get { return "Edit One Key Axis window"; } }

        private int windowID = spaar.ModLoader.Util.GetWindowID();
        private Rect windowRect = new Rect(100, 100, 320, 250);

        private OneKeyAxis axis = new OneKeyAxis(KeyCode.None);

        private void SaveAxis()
        {
            Visible = false;
            AdvancedControls.Instance.SaveAxis(axis);
        }

        public void EditAxis(OneKeyAxis axis)
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
                windowRect = GUILayout.Window(windowID, windowRect, DoWindow, "Edit One Key Axis");
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
                20);

            Util.DrawRect(new Rect(
                graphRect.x + axis.Output * (graphRect.width - 20),
                graphRect.y,
                20, 20), Color.yellow);

            // Draw axis controls
            GUILayout.BeginArea(new Rect(
                GUI.skin.window.padding.left,
                graphRect.y + graphRect.height + GUI.skin.window.padding.bottom,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                400));

            // Draw key mapper
            GUILayout.Button(new GUIContent(axis.Key.ToString(), "Key Mapper"), Elements.Buttons.Red);

            if (GUI.tooltip == "Key Mapper")
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                    if (Input.GetKey(key))
                    {
                        axis.Key = key == KeyCode.Backspace ? KeyCode.None : key;
                        break;
                    }
            }

            // Draw Sensitivity slider
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sensitivity", Util.LabelStyle);
            GUILayout.Label((Mathf.Round(axis.Sensitivity * 100) / 100).ToString(),
                Util.LabelStyle,
                GUILayout.Width(60));
            GUILayout.EndHorizontal();

            axis.Sensitivity = GUILayout.HorizontalSlider(axis.Sensitivity, 0, 10);

            // Draw Curvature slider
            GUILayout.BeginHorizontal();
            GUILayout.Label("Gravity", Util.LabelStyle);
            GUILayout.Label((Mathf.Round(axis.Gravity * 100) / 100).ToString(),
                Util.LabelStyle,
                GUILayout.Width(60));
            GUILayout.EndHorizontal();

            axis.Gravity = GUILayout.HorizontalSlider(axis.Gravity, 0, 10);

            GUILayout.EndArea();

            if (GUI.Button(new Rect(windowRect.width - 28, 8, 20, 20),
                "×", Elements.Buttons.Red))
                Visible = false;

            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }
    }
}
