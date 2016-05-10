using System;
using UnityEngine;
using spaar.ModLoader.UI;
using AdvancedControls.Axes;

namespace AdvancedControls.UI
{
    public class OneKeyAxisEdit : AxisEdit
    {
        public new string name { get { return "Edit One Key Axis window"; } }

        protected override float DesiredWidth { get; } = 320;
        protected override float DesiredHeight { get; } = 250;

        private new OneKeyAxis axis = new OneKeyAxis(KeyCode.None);

        public override void SaveAxis()
        {
            Visible = false;
            axis.Name = axisName;
            AdvancedControlsMod.AxisList.SaveAxis(axis);
        }

        public override void EditAxis(Axes.Axis axis)
        {
            Visible = true;
            this.axisName = axis.Name;
            this.axis = (axis as OneKeyAxis).Clone();
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
        }
    }
}
