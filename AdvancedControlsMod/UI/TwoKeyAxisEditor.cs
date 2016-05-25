using System;
using UnityEngine;
using spaar.ModLoader.UI;
using AdvancedControls.Axes;

namespace AdvancedControls.UI
{
    public class TwoKeyAxisEditor : AxisEditor
    {
        public TwoKeyAxisEditor(InputAxis axis)
        {
            Axis = axis as StandardAxis;
        }

        private StandardAxis Axis;

        public void DrawAxis(Rect windowRect)
        {
            // Draw graph
            Rect graphRect = new Rect(
                GUI.skin.window.padding.left,
                GUI.skin.window.padding.top + 36,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                20);

            GUILayout.Box(GUIContent.none, GUILayout.Height(20));

            Util.FillRect(new Rect(
                graphRect.x + graphRect.width / 2 - 10 + Axis.OutputValue * (graphRect.width - 20) / 2,
                graphRect.y,
                20, 20), Color.yellow);

            // Draw key mappers
            GUILayout.BeginHorizontal();

            GUILayout.Button(new GUIContent(Axis.NegativeKey.ToString(), "Key Mapper Negative"), Elements.Buttons.Red);
            if (GUI.tooltip == "Key Mapper Negative")
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                    if (Input.GetKey(key))
                    {
                        Axis.NegativeKey = key == KeyCode.Backspace ? KeyCode.None : key;
                        break;
                    }
            }
            GUILayout.Button(new GUIContent(Axis.PositiveKey.ToString(), "Key Mapper Positive"), Elements.Buttons.Red);
            if (GUI.tooltip == "Key Mapper Positive")
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                    if (Input.GetKey(key))
                    {
                        Axis.PositiveKey = key == KeyCode.Backspace ? KeyCode.None : key;
                        break;
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
        }
    }
}
