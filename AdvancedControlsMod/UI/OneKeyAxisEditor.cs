using System;
using UnityEngine;
using spaar.ModLoader.UI;
using AdvancedControls.Axes;

namespace AdvancedControls.UI
{
    public class OneKeyAxisEditor : AxisEditor
    {
        public OneKeyAxisEditor(InputAxis axis)
        {
            Axis = axis as OneKeyAxis;
        }

        private OneKeyAxis Axis;

        public void DrawAxis(Rect windowRect)
        {
            // Draw graph
            Rect graphRect = new Rect(
                GUI.skin.window.padding.left,
                GUI.skin.window.padding.top + 36,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                20);

            GUILayout.Box(GUIContent.none, GUILayout.Height(20));

            Util.DrawRect(new Rect(
                graphRect.x + Axis.OutputValue * (graphRect.width - 20),
                graphRect.y,
                20, 20), Color.yellow);

            // Draw key mapper
            GUILayout.Button(new GUIContent(Axis.Key.ToString(), "Key Mapper"), Elements.Buttons.Red);

            if (GUI.tooltip == "Key Mapper")
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                    if (UnityEngine.Input.GetKey(key))
                    {
                        Axis.Key = key == KeyCode.Backspace ? KeyCode.None : key;
                        break;
                    }
            }

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
        }
    }
}
