using System;
using UnityEngine;
using spaar.ModLoader.UI;

namespace AdvancedControlsMod.UI
{
    public class OneKeyAxisEdit : MonoBehaviour
    {
        public bool Visible { get; set; } = false;

        public new string name { get { return "Edit One Key Axis window"; } }

        private int windowID = spaar.ModLoader.Util.GetWindowID();
        private Rect windowRect = new Rect(100, 100, 320, 238);

        private string axisSaveName = "new axis";
        private OneKeyAxis configuringAxis = new OneKeyAxis(KeyCode.None);

        private void SaveAxis()
        {
            Visible = false;
            AdvancedControls.Instance.SaveAxis(configuringAxis, axisSaveName);
        }

        public void EditAxis(OneKeyAxis axis)
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
                windowRect = GUI.Window(windowID, windowRect, DoWindow, "Edit One Key Axis");
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

            // Draw visualisation
            vertical_offset += GUI.skin.window.padding.left + 20;

            Util.DrawRect(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width - 2 * GUI.skin.window.padding.left,
                20), Color.black);
            Util.DrawRect(new Rect(
                GUI.skin.window.padding.left + configuringAxis.Output * (windowRect.width - 2 * GUI.skin.window.padding.left - 20),
                vertical_offset,
                20, 20), Color.yellow);

            // Draw key mapper
            vertical_offset += GUI.skin.window.padding.left + 20;

            Rect keyMapperRect = new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width - 2 * GUI.skin.window.padding.left,
                30);

            GUI.Button(keyMapperRect, new GUIContent(configuringAxis.Key.ToString(), "Key Mapper"), Elements.Buttons.Red);

            if (GUI.tooltip == "Key Mapper")
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                    if (Input.GetKey(key))
                    {
                        configuringAxis.Key = key == KeyCode.Backspace ? KeyCode.None : key;
                        break;
                    }
            }

            // Draw Sensitivity slider
            vertical_offset += GUI.skin.window.padding.left + 30;

            GUI.Label(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width - 2f * GUI.skin.window.padding.left, 20),
                "Sensitivity: " + configuringAxis.Sensitivity);

            vertical_offset += GUI.skin.window.padding.left + 8;

            configuringAxis.Sensitivity = GUI.HorizontalSlider(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width - 2f * GUI.skin.window.padding.left, 20),
                configuringAxis.Sensitivity, 0, 10);

            vertical_offset += GUI.skin.window.padding.left + 20;

            // Draw Gravity slider
            GUI.Label(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width - 2f * GUI.skin.window.padding.left, 20),
                "Gravity: " + configuringAxis.Gravity);

            vertical_offset += GUI.skin.window.padding.left + 8;

            configuringAxis.Gravity = GUI.HorizontalSlider(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width - 2f * GUI.skin.window.padding.left, 20),
                configuringAxis.Gravity, 0, 10);

            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }
    }
}
