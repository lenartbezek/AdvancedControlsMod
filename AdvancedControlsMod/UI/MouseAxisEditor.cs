using System;
using UnityEngine;
using Lench.AdvancedControls.Axes;
using spaar.ModLoader.UI;

namespace Lench.AdvancedControls.UI
{
    internal class MouseAxisEditor : IAxisEditor
    {
        private MouseAxis Axis;

        public MouseAxisEditor(InputAxis axis)
        {
            Axis = axis as MouseAxis;
        }

        private string center_string;
        private string range_string;

        public void Open()
        {
            center_string = Axis.Center.ToString("0.00");
            range_string = Axis.Range.ToString("0.00");
        }

        public void Close() { }

        public void DrawAxis(Rect windowRect)
        {
            // Graph rect
            Rect graphRect = GUILayoutUtility.GetAspectRect((float)Screen.width / (float)Screen.height);

            // Axis value
            GUI.Label(new Rect(graphRect.x, graphRect.y, graphRect.width, 20),
                    "  <color=#808080><b>" + Axis.OutputValue.ToString("0.00") + "</b></color>",
                    new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft });

            // Screen size
            GUI.Label(new Rect(graphRect.x, graphRect.y + graphRect.height - 20, graphRect.width, 20),
                    "  <color=#808080><b>" + Screen.width + " / " + Screen.height + "</b></color>",
                    new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleCenter });

            // Screen frame
            Util.DrawRect(graphRect, Color.gray);

            float mouse_pos = Axis.Axis == global::Axis.X ? UnityEngine.Input.mousePosition.x : UnityEngine.Input.mousePosition.y;
            float screen_size = Axis.Axis == global::Axis.X ? Screen.width : Screen.height;
            float range_size = Axis.Range == 0 ? 1 : screen_size * Axis.Range / 2f;
            float center = screen_size / 2f + screen_size / 2f * Axis.Center;

            if (Axis.Axis == global::Axis.X)
            {
                // Draw range frame
                float frame_left = Mathf.Clamp(graphRect.width * ((center - range_size) / screen_size), 0, graphRect.width);
                float frame_right = Mathf.Clamp(graphRect.width * ((center + range_size) / screen_size), 0, graphRect.width);
                Util.DrawRect(new Rect(
                                  graphRect.x + frame_left,
                                  graphRect.y,
                                  frame_right - frame_left,
                                  graphRect.height),
                         Color.white);

                // Draw center line
                Util.FillRect(new Rect(
                                  graphRect.x + graphRect.width  * (center / screen_size),
                                  graphRect.y,
                                  1,
                                  graphRect.height),
                         Color.white);

                // Draw mouse position
                float line_pos = Mathf.Clamp((mouse_pos / screen_size), 0, 1);
                Util.FillRect(new Rect(
                                  graphRect.x + graphRect.width * line_pos,
                                  graphRect.y,
                                  1,
                                  graphRect.height),
                         Color.yellow);
            }
            else
            {
                // Draw range frame
                float frame_top = Mathf.Clamp(graphRect.height * ((screen_size - center - range_size) / screen_size), 0, graphRect.height);
                float frame_bottom = Mathf.Clamp(graphRect.height * ((screen_size - center + range_size) / screen_size), 0, graphRect.height);
                Util.DrawRect(new Rect(
                                  graphRect.x,
                                  graphRect.y + frame_top,
                                  graphRect.width,
                                  frame_bottom - frame_top),
                         Color.white);

                // Draw center line
                Util.FillRect(new Rect(
                                  graphRect.x,
                                  graphRect.y + graphRect.height * ((screen_size - center) / screen_size),
                                  graphRect.width,
                                  1),
                         Color.white);

                // Draw mouse position
                float line_pos = Mathf.Clamp(((screen_size - mouse_pos) / screen_size), 0, 1);
                Util.FillRect(new Rect(
                                  graphRect.x,
                                  graphRect.y + graphRect.height * line_pos,
                                  graphRect.width,
                                  1),
                         Color.yellow);
            }

            // Draw axis toggles
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", Axis.Axis == global::Axis.X ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(80)))
                Axis.Axis = global::Axis.X;
            if (GUILayout.Button("Y", Axis.Axis == global::Axis.Y ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(80)))
                Axis.Axis = global::Axis.Y;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Draw sliders
            Axis.Center = Util.DrawSlider("Center", Axis.Center, -1, 1, center_string, out center_string);
            Axis.Range = Util.DrawSlider("Range", Axis.Range, 0, 1, range_string, out range_string);
        }

        public string GetError()
        {
            return null;
        }

        public string GetHelpURL()
        {
            return "https://github.com/lench4991/AdvancedControlsMod/wiki/Mouse-axis";
        }

        public string GetNote()
        {
            return null;
        }
    }
}
