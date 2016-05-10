using System;
using UnityEngine;
using spaar.ModLoader.UI;

namespace AdvancedControlsMod.UI
{
    public class TwoKeyAxisEdit : AxisEdit
    {
        public new string name { get { return "Edit Two Key Axis window"; } }

        protected override float DesiredWidth { get; } = 320;
        protected override float DesiredHeight { get; } = 286;

        private new TwoKeyAxis axis = new TwoKeyAxis(KeyCode.None, KeyCode.None);

        public override void SaveAxis()
        {
            Visible = false;
            axis.Name = axisName;
            AdvancedControlsMod.AxisList.SaveAxis(axis);
        }

        public override void EditAxis(Axis axis)
        {
            Visible = true;
            this.axisName = axis.Name;
            this.axis = (axis as TwoKeyAxis).Clone();
        }

        protected override void DoWindow(int id)
        {
            base.DoWindow(id);

            // Draw graph
            Rect graphRect = new Rect(
                GUI.skin.window.padding.left,
                GUI.skin.window.padding.top + 36,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                20);

            Util.DrawRect(new Rect(
                graphRect.x + graphRect.width / 2 - 10 + axis.Output * (graphRect.width - 20) / 2,
                graphRect.y,
                20, 20), Color.yellow);

            // Draw axis controls
            GUILayout.BeginArea(new Rect(
                GUI.skin.window.padding.left,
                graphRect.y + graphRect.height + GUI.skin.window.padding.bottom,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                400));

            // Draw key mappers
            GUILayout.BeginHorizontal();

            GUILayout.Button(new GUIContent(axis.NegativeKey.ToString(), "Key Mapper Negative"), Elements.Buttons.Red, GUILayout.Width(140));
            if (GUI.tooltip == "Key Mapper Negative")
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                    if (Input.GetKey(key))
                    {
                        axis.NegativeKey = key == KeyCode.Backspace ? KeyCode.None : key;
                        break;
                    }
            }
            GUILayout.Button(new GUIContent(axis.PositiveKey.ToString(), "Key Mapper Positive"), Elements.Buttons.Red, GUILayout.Width(140));
            if (GUI.tooltip == "Key Mapper Positive")
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                    if (Input.GetKey(key))
                    {
                        axis.PositiveKey = key == KeyCode.Backspace ? KeyCode.None : key;
                        break;
                    }
            }

            GUILayout.EndHorizontal();

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

            // Draw toggles
            GUILayout.BeginHorizontal();

            axis.Invert = GUILayout.Toggle(axis.Invert, "",
                Util.ToggleStyle,
                GUILayout.Width(20),
                GUILayout.Height(20));

            GUILayout.Label("Invert ",
                new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });

            axis.Snap = GUILayout.Toggle(axis.Snap, "",
                Util.ToggleStyle,
                GUILayout.Width(20),
                GUILayout.Height(20));

            GUILayout.Label("Snap ",
                new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}
