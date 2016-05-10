using UnityEngine;
using spaar.ModLoader.UI;

namespace AdvancedControlsMod.UI
{
    public class ControllerAxisEdit : AxisEdit
    {
        public new string name { get { return "Edit Controller Axis window"; } }

        protected new ControllerAxis axis = new ControllerAxis("Vertical");

        protected override float DesiredWidth { get; } = 320;
        protected override float DesiredHeight { get; } = 611;

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
            this.axis = (axis as ControllerAxis).Clone();
        }

        private Rect graphRect;
        private Rect last_graphRect;
        private GUIStyle graphStyle = new GUIStyle();
        private Texture2D graphTex;

        private ControllerAxis.Param last_parameters;

        private bool vertical = true;

        private void DrawGraph()
        {
            if (graphTex == null || graphRect != last_graphRect)
                graphTex = new Texture2D((int)graphRect.width, (int)graphRect.height);
            if (!axis.Parameters.Equals(last_parameters) || graphRect != last_graphRect)
            {
                for (int i = 0; i < graphRect.width; i++)
                {
                    var value = axis.Process((i - graphRect.width / 2) / (graphRect.width / 2));
                    var point = graphRect.height / 2 - (graphRect.height-1) / 2 * value;
                    for (int j = 0; j < graphRect.height; j++)
                    {
                        if ((int)point == j)
                            graphTex.SetPixel((int)graphRect.width - i - 1, j, Color.white);
                        else
                            graphTex.SetPixel((int)graphRect.width - i - 1, j, new Color(0, 0, 0, 0));
                    }
                }
                graphTex.Apply();
                last_graphRect = graphRect;
                last_parameters = axis.Parameters;
            }
            graphStyle.normal.background = graphTex;
            GUI.Box(graphRect, GUIContent.none, graphStyle);
        }

        protected override void DoWindow(int id)
        {
            base.DoWindow(id);

            // Draw graph
            graphRect = new Rect(
                GUI.skin.window.padding.left,
                GUI.skin.window.padding.top + 36,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right);

            Util.DrawRect(new Rect(graphRect.x + graphRect.width / 2 + graphRect.width / 2 * axis.Input,
                              graphRect.y,
                              1,
                              graphRect.height),
                     Color.yellow);

            DrawGraph();

            // Draw axis controls
            GUILayout.BeginArea(new Rect(
                GUI.skin.window.padding.left,
                graphRect.y + graphRect.height + GUI.skin.window.padding.bottom,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                400));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Vertical",
                vertical ? Elements.Buttons.Default : Elements.Buttons.Disabled))
            {
                vertical = true;
            }

            if (GUILayout.Button("Horizontal",
                !vertical ? Elements.Buttons.Default : Elements.Buttons.Disabled))
            {
                vertical = false;
            }
            axis.Axis = vertical ? "Vertical" : "Horizontal";
            GUILayout.EndHorizontal();

            // Draw Sensitivity slider
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sensitivity ", Util.LabelStyle);
            GUILayout.Label((Mathf.Round(axis.Sensitivity * 100) / 100).ToString(),
                Util.LabelStyle,
                GUILayout.Width(60));
            GUILayout.EndHorizontal();

            axis.Sensitivity = GUILayout.HorizontalSlider(axis.Sensitivity, 0, 5);

            // Draw Curvature slider
            GUILayout.BeginHorizontal();
            GUILayout.Label("Curvature ", Util.LabelStyle);
            GUILayout.Label((Mathf.Round(axis.Curvature * 100) / 100).ToString(),
                Util.LabelStyle,
                GUILayout.Width(60));
            GUILayout.EndHorizontal();

            axis.Curvature = GUILayout.HorizontalSlider(axis.Curvature, 0, 3);
            
            // Draw Deadzone slider
            GUILayout.BeginHorizontal();
            GUILayout.Label("Deadzone ", Util.LabelStyle);
            GUILayout.Label((Mathf.Round(axis.Deadzone * 100) / 100).ToString(),
                Util.LabelStyle,
                GUILayout.Width(60));
            GUILayout.EndHorizontal();

            axis.Deadzone = GUILayout.HorizontalSlider(axis.Deadzone, 0, 0.5f);
            
            // Draw Invert toggle
            GUILayout.BeginHorizontal();
            axis.Invert = GUILayout.Toggle(axis.Invert, "",
                Util.ToggleStyle,
                GUILayout.Width(20),
                GUILayout.Height(20));

            GUILayout.Label("Invert ",
                new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}
