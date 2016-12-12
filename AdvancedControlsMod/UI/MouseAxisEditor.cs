using Lench.AdvancedControls.Axes;
using spaar.ModLoader.UI;
using UnityEngine;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Lench.AdvancedControls.UI
{
    internal class MouseAxisEditor : IAxisEditor
    {
        private readonly MouseAxis _axis;

        public MouseAxisEditor(InputAxis axis)
        {
            _axis = axis as MouseAxis;
        }

        private string _centerString;
        private string _rangeString;

        public void Open()
        {
            _centerString = _axis.Center.ToString("0.00");
            _rangeString = _axis.Range.ToString("0.00");
        }

        public void Close() { }

        public void DrawAxis(Rect windowRect)
        {
            // Graph rect
            Rect graphRect = GUILayoutUtility.GetAspectRect(Screen.width / (float)Screen.height);

            // Axis value
            GUI.Label(new Rect(graphRect.x, graphRect.y, graphRect.width, 20),
                    "  <color=#808080><b>" + _axis.OutputValue.ToString("0.00") + "</b></color>",
                    new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft });

            // Screen size
            GUI.Label(new Rect(graphRect.x, graphRect.y + graphRect.height - 20, graphRect.width, 20),
                    "  <color=#808080><b>" + Screen.width + " / " + Screen.height + "</b></color>",
                    new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleCenter });

            // Screen frame
            Util.DrawRect(graphRect, Color.gray);

            float mousePos = _axis.Axis == Axis.X ? UnityEngine.Input.mousePosition.x : UnityEngine.Input.mousePosition.y;
            float screenSize = _axis.Axis == Axis.X ? Screen.width : Screen.height;
            float rangeSize = _axis.Range == 0 ? 1 : screenSize * _axis.Range / 2f;
            float center = screenSize / 2f + screenSize / 2f * _axis.Center;

            if (_axis.Axis == Axis.X)
            {
                // Draw range frame
                float frameLeft = Mathf.Clamp(graphRect.width * ((center - rangeSize) / screenSize), 0, graphRect.width);
                float frameRight = Mathf.Clamp(graphRect.width * ((center + rangeSize) / screenSize), 0, graphRect.width);
                Util.DrawRect(new Rect(
                                  graphRect.x + frameLeft,
                                  graphRect.y,
                                  frameRight - frameLeft,
                                  graphRect.height),
                         Color.white);

                // Draw center line
                Util.FillRect(new Rect(
                                  graphRect.x + graphRect.width  * (center / screenSize),
                                  graphRect.y,
                                  1,
                                  graphRect.height),
                         Color.white);

                // Draw mouse position
                float linePos = Mathf.Clamp((mousePos / screenSize), 0, 1);
                Util.FillRect(new Rect(
                                  graphRect.x + graphRect.width * linePos,
                                  graphRect.y,
                                  1,
                                  graphRect.height),
                         Color.yellow);
            }
            else
            {
                // Draw range frame
                float frameTop = Mathf.Clamp(graphRect.height * ((screenSize - center - rangeSize) / screenSize), 0, graphRect.height);
                float frameBottom = Mathf.Clamp(graphRect.height * ((screenSize - center + rangeSize) / screenSize), 0, graphRect.height);
                Util.DrawRect(new Rect(
                                  graphRect.x,
                                  graphRect.y + frameTop,
                                  graphRect.width,
                                  frameBottom - frameTop),
                         Color.white);

                // Draw center line
                Util.FillRect(new Rect(
                                  graphRect.x,
                                  graphRect.y + graphRect.height * ((screenSize - center) / screenSize),
                                  graphRect.width,
                                  1),
                         Color.white);

                // Draw mouse position
                float linePos = Mathf.Clamp(((screenSize - mousePos) / screenSize), 0, 1);
                Util.FillRect(new Rect(
                                  graphRect.x,
                                  graphRect.y + graphRect.height * linePos,
                                  graphRect.width,
                                  1),
                         Color.yellow);
            }

            // Draw axis toggles
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", _axis.Axis == Axis.X ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(80)))
                _axis.Axis = Axis.X;
            if (GUILayout.Button("Y", _axis.Axis == Axis.Y ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(80)))
                _axis.Axis = Axis.Y;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Draw sliders
            _axis.Center = Util.DrawSlider("Center", _axis.Center, -1, 1, _centerString, out _centerString);
            _axis.Range = Util.DrawSlider("Range", _axis.Range, 0, 1, _rangeString, out _rangeString);
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
