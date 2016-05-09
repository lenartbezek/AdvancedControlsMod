using System;
using UnityEngine;
using spaar.ModLoader.UI;

namespace AdvancedControlsMod.UI
{
    public class TwoKeyAxisEdit : MonoBehaviour
    {
        public bool Visible { get; set; } = false;

        public new string name { get { return "Edit Two Key Axis window"; } }

        private int windowID = spaar.ModLoader.Util.GetWindowID();
        private Rect windowRect = new Rect(100, 100, 320, 294);

        private string axisSaveName = "new axis";
        private TwoKeyAxis configuringAxis = new TwoKeyAxis(KeyCode.None, KeyCode.None);

        private void SaveAxis()
        {
            Visible = false;
            AdvancedControls.Instance.SaveAxis(configuringAxis, axisSaveName);
        }

        public void EditAxis(TwoKeyAxis axis)
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
                windowRect = GUI.Window(windowID, windowRect, DoWindow, "Edit Two Key Axis");
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
                windowRect.width / 2 - 10 + configuringAxis.Output * (windowRect.width - 2 * GUI.skin.window.padding.left - 20) / 2,
                vertical_offset,
                20, 20), Color.yellow);

            // Draw key mapper
            vertical_offset += GUI.skin.window.padding.left + 20;

            GUI.Button(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width * 0.5f - 1.5f * GUI.skin.window.padding.left,
                30),
                new GUIContent(configuringAxis.NegativeKey.ToString(), "Key Mapper Negative"), Elements.Buttons.Red);

            GUI.Button(new Rect(
                windowRect.width * 0.5f + 0.5f * GUI.skin.window.padding.left,
                vertical_offset,
                windowRect.width * 0.5f - 1.5f * GUI.skin.window.padding.left,
                30),
                new GUIContent(configuringAxis.PositiveKey.ToString(), "Key Mapper Positive"), Elements.Buttons.Red);

            if (GUI.tooltip == "Key Mapper Negative")
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                    if (Input.GetKey(key))
                    {
                        configuringAxis.NegativeKey = key == KeyCode.Backspace ? KeyCode.None : key;
                        break;
                    }
            }

            if (GUI.tooltip == "Key Mapper Positive")
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                    if (Input.GetKey(key))
                    {
                        configuringAxis.PositiveKey = key == KeyCode.Backspace ? KeyCode.None : key;
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

            vertical_offset += GUI.skin.window.padding.left + 20;

            // Draw Invert toggle
            configuringAxis.Snap = GUI.Toggle(new Rect(
                GUI.skin.window.padding.left,
                vertical_offset,
                20, 20),
                configuringAxis.Snap, "");

            GUI.Label(new Rect(
                GUI.skin.window.padding.left + 28,
                vertical_offset + 4,
                windowRect.width - 2f * GUI.skin.window.padding.left, 20),
                "Snap ");

            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }
    }
}
