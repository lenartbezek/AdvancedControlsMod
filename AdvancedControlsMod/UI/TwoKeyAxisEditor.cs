using System;
using UnityEngine;
using spaar.ModLoader.UI;
using AdvancedControls.Axes;
using AdvancedControls.Input;
using System.Collections.Generic;

namespace AdvancedControls.UI
{
    public class TwoKeyAxisEditor : AxisEditor
    {
        public TwoKeyAxisEditor(InputAxis axis)
        {
            Axis = axis as StandardAxis;
        }

        private StandardAxis Axis;

        internal string help;
        internal string error;
        internal string note;

        public void DrawAxis(Rect windowRect)
        {
            // Draw graph
            Rect graphRect = new Rect(
                GUI.skin.window.padding.left,
                GUI.skin.window.padding.top + 36,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                20);

            GUILayout.Label("  <color=#808080><b>"+Axis.OutputValue.ToString("0.00") + "</b></color>",
                new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft },
                GUILayout.Height(20));

            Util.DrawRect(graphRect, Color.gray);

            Util.FillRect(new Rect(
                                  graphRect.x + graphRect.width / 2 + graphRect.width / 2 * Axis.OutputValue,
                                  graphRect.y,
                                  1,
                                  graphRect.height),
                         Color.yellow);

            // Draw key mappers 
            GUILayout.BeginHorizontal();

            GUILayout.Button(new GUIContent(Axis.NegativeBind != null ? Axis.NegativeBind.Name : "None", "Key Mapper Negative"), Elements.Buttons.Red);
            if (GUI.tooltip == "Key Mapper Negative")
            {
                foreach (KeyValuePair<Guid, Controller> entry in Controller.Devices)
                    foreach (Button b in entry.Value.Buttons)
                    {
                        if (b.IsDown) Axis.NegativeBind = b;
                    }
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (key >= KeyCode.JoystickButton0 && key <= KeyCode.Joystick8Button19)
                        continue;
                    if (UnityEngine.Input.GetKey(key))
                    {
                        Axis.NegativeBind = key == KeyCode.Backspace ? null : new Key(key);
                        break;
                    }
                }
            }
            GUILayout.Button(new GUIContent(Axis.PositiveBind != null ? Axis.PositiveBind.Name : "None", "Key Mapper Positive"), Elements.Buttons.Red);
            if (GUI.tooltip == "Key Mapper Positive")
            {
                foreach (KeyValuePair<Guid, Controller> entry in Controller.Devices)
                    foreach (Button b in entry.Value.Buttons)
                    {
                        if (b.IsDown) Axis.PositiveBind = b;
                    }
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (key >= KeyCode.JoystickButton0 && key <= KeyCode.Joystick8Button19)
                        continue;
                    if (UnityEngine.Input.GetKey(key))
                    {
                        Axis.PositiveBind = key == KeyCode.Backspace ? null : new Key(key);
                        break;
                    }
                }   
            }

            GUILayout.EndHorizontal();

            // Draw Sensitivity slider
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sensitivity", Util.LabelStyle);
            GUILayout.Label((Mathf.Round(Axis.Sensitivity * 100) / 100).ToString(),
                Util.LabelStyle,
                GUILayout.Width(60));
            GUILayout.EndHorizontal();

            Axis.Sensitivity = GUILayout.HorizontalSlider(Axis.Sensitivity, 0, 10);

            // Draw Curvature slider
            GUILayout.BeginHorizontal();
            GUILayout.Label("Gravity", Util.LabelStyle);
            GUILayout.Label((Mathf.Round(Axis.Gravity * 100) / 100).ToString(),
                Util.LabelStyle,
                GUILayout.Width(60));
            GUILayout.EndHorizontal();

            Axis.Gravity = GUILayout.HorizontalSlider(Axis.Gravity, 0, 10);

            // Draw toggles
            GUILayout.BeginHorizontal();

            Axis.Invert = GUILayout.Toggle(Axis.Invert, "",
                Util.ToggleStyle,
                GUILayout.Width(20),
                GUILayout.Height(20));

            GUILayout.Label("Invert ",
                new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });

            Axis.Snap = GUILayout.Toggle(Axis.Snap, "",
                Util.ToggleStyle,
                GUILayout.Width(20),
                GUILayout.Height(20));

            GUILayout.Label("Snap ",
                new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });

            GUILayout.EndHorizontal();

            // Set notes
            note = "<color=#FFFF00><b>Device disconnected</b></color>";
            var disconnected = false;
            if (Axis.PositiveBind != null && !Axis.PositiveBind.Connected)
            {
                disconnected = true;
                note += "\n'" + Axis.PositiveBind.Name + "' is not connected.";
            }
            if (Axis.NegativeBind != null && !Axis.NegativeBind.Connected)
            {
                disconnected = true;
                note += "\n'" + Axis.NegativeBind.Name + "' is not connected.";
            }
            if (!disconnected)
                note = null;
        }

        public string GetHelp()
        {
            return help;
        }

        public string GetNote()
        {
            return note;
        }

        public string GetError()
        {
            return error;
        }
    }
}
