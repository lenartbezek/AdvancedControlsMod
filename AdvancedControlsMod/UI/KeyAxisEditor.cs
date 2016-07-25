using System;
using UnityEngine;
using spaar.ModLoader.UI;
using Lench.AdvancedControls.Axes;
using Lench.AdvancedControls.Input;
using System.Collections.Generic;

namespace Lench.AdvancedControls.UI
{
    internal class KeyAxisEditor : AxisEditor
    {
        public KeyAxisEditor(InputAxis axis)
        {
            Axis = axis as KeyAxis;
        }

        private KeyAxis Axis;

        internal string note;

        private string sens_string;
        private string grav_string;
        private string momn_string;

        public void Open()
        {
            sens_string = Axis.Sensitivity.ToString("0.00");
            grav_string = Axis.Gravity.ToString("0.00");
            momn_string = Axis.Momentum.ToString("0.00");
        }

        public void Close() { }

        public void DrawAxis(Rect windowRect)
        {
            // Draw graph
            Rect graphRect = new Rect(
                GUI.skin.window.padding.left,
                GUI.skin.window.padding.top + 36,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                20);

            GUILayout.Label("  <color=#808080><b>"+ Axis.OutputValue.ToString("0.00") + "</b></color>",
                new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft },
                GUILayout.Height(20));

            Util.DrawRect(graphRect, Color.gray);
            Util.FillRect(new Rect(
                    graphRect.x + graphRect.width / 2,
                    graphRect.y,
                    1,
                    graphRect.height),
                Color.gray);

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
            Axis.Sensitivity = Util.DrawSlider("Sensitivity", Axis.Sensitivity, 0, 10, sens_string, out sens_string);

            // Draw Gravity slider
            Axis.Gravity = Util.DrawSlider("Gravity", Axis.Gravity, 0, 10, grav_string, out grav_string);

            // Draw Momentum slider
            Axis.Momentum = Util.DrawSlider("Momentum", Axis.Momentum, 0, 10, momn_string, out momn_string);

            // Draw toggles
            GUILayout.BeginHorizontal();

            Axis.Raw = GUILayout.Toggle(Axis.Raw, "",
                Util.ToggleStyle,
                GUILayout.Width(20),
                GUILayout.Height(20));

            GUILayout.Label("Raw ",
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

        public string GetHelpURL()
        {
            return "https://github.com/lench4991/AdvancedControlsMod/wiki/Key-Axes";
        }

        public string GetNote()
        {
            return note;
        }

        public string GetError()
        {
            return null;
        }
    }
}
