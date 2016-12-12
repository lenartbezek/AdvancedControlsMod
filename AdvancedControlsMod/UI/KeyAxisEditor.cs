using System;
using UnityEngine;
using spaar.ModLoader.UI;
using Lench.AdvancedControls.Axes;
using Lench.AdvancedControls.Input;

namespace Lench.AdvancedControls.UI
{
    internal class KeyAxisEditor : IAxisEditor
    {
        public KeyAxisEditor(InputAxis axis)
        {
            _axis = axis as KeyAxis;
        }

        private readonly KeyAxis _axis;

        internal string Note;

        private string _sensString;
        private string _gravString;
        private string _momnString;

        public void Open()
        {
            _sensString = _axis.Sensitivity.ToString("0.00");
            _gravString = _axis.Gravity.ToString("0.00");
            _momnString = _axis.Momentum.ToString("0.00");
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

            GUILayout.Label("  <color=#808080><b>"+ _axis.OutputValue.ToString("0.00") + "</b></color>",
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
                                  graphRect.x + graphRect.width / 2 + graphRect.width / 2 * _axis.OutputValue,
                                  graphRect.y,
                                  1,
                                  graphRect.height),
                         Color.yellow);

            // Draw key mappers 
            GUILayout.BeginHorizontal();

            GUILayout.Button(new GUIContent(_axis.NegativeBind != null ? _axis.NegativeBind.Name : "None", "Key Mapper Negative"), Elements.Buttons.Red);
            if (GUI.tooltip == "Key Mapper Negative")
            {
                foreach (var c in Controller.ControllerList)
                    foreach (var b in c.Buttons)
                    {
                        if (b.IsDown) _axis.NegativeBind = b;
                    }
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (key >= KeyCode.JoystickButton0 && key <= KeyCode.Joystick8Button19)
                        continue;
                    if (UnityEngine.Input.GetKey(key))
                    {
                        _axis.NegativeBind = key == KeyCode.Backspace ? null : new Key(key);
                        break;
                    }
                }
            }
            GUILayout.Button(new GUIContent(_axis.PositiveBind != null ? _axis.PositiveBind.Name : "None", "Key Mapper Positive"), Elements.Buttons.Red);
            if (GUI.tooltip == "Key Mapper Positive")
            {
                foreach (var c in Controller.ControllerList)
                    foreach (var b in c.Buttons)
                    {
                        if (b.IsDown) _axis.PositiveBind = b;
                    }
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (key >= KeyCode.JoystickButton0 && key <= KeyCode.Joystick8Button19)
                        continue;
                    if (UnityEngine.Input.GetKey(key))
                    {
                        _axis.PositiveBind = key == KeyCode.Backspace ? null : new Key(key);
                        break;
                    }
                }   
            }

            GUILayout.EndHorizontal();

            // Draw Sensitivity slider
            _axis.Sensitivity = Util.DrawSlider("Sensitivity", _axis.Sensitivity, 0, 10, _sensString, out _sensString);

            // Draw Gravity slider
            _axis.Gravity = Util.DrawSlider("Gravity", _axis.Gravity, 0, 10, _gravString, out _gravString);

            // Draw Momentum slider
            _axis.Momentum = Util.DrawSlider("Momentum", _axis.Momentum, 0, 10, _momnString, out _momnString);

            // Draw toggles
            GUILayout.BeginHorizontal();

            _axis.Raw = GUILayout.Toggle(_axis.Raw, "",
                Util.ToggleStyle,
                GUILayout.Width(20),
                GUILayout.Height(20));

            GUILayout.Label("Raw ",
                new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });

            _axis.Snap = GUILayout.Toggle(_axis.Snap, "",
                Util.ToggleStyle,
                GUILayout.Width(20),
                GUILayout.Height(20));

            GUILayout.Label("Snap ",
                new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });

            GUILayout.EndHorizontal();

            // Set notes
            Note = "<color=#FFFF00><b>Device disconnected</b></color>";
            var disconnected = false;
            if (_axis.PositiveBind != null && !_axis.PositiveBind.Connected)
            {
                disconnected = true;
                Note += "\n'" + _axis.PositiveBind.Name + "' is not connected.";
            }
            if (_axis.NegativeBind != null && !_axis.NegativeBind.Connected)
            {
                disconnected = true;
                Note += "\n'" + _axis.NegativeBind.Name + "' is not connected.";
            }
            if (!disconnected)
                Note = null;
        }

        public string GetHelpURL()
        {
            return "https://github.com/lench4991/AdvancedControlsMod/wiki/Key-Axes";
        }

        public string GetNote()
        {
            return Note;
        }

        public string GetError()
        {
            return null;
        }
    }
}
