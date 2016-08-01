using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using spaar.ModLoader.UI;
using Lench.AdvancedControls.Controls;
using Lench.AdvancedControls.Axes;

namespace Lench.AdvancedControls.UI
{
    internal class ControlMapper : SingleInstance<ControlMapper>
    {
        internal bool Visible { get; set; } = false;
        public override string Name { get { return "ACM: Control Mapper"; } }

        internal int windowID = spaar.ModLoader.Util.GetWindowID();
        internal Rect windowRect = new Rect(680, 115, 320, 50);

        internal BlockBehaviour Block;
        internal List<Control> controls;

        internal AxisSelector popup;

        internal void ShowBlockControls(BlockBehaviour b)
        {
            Block = b;
            controls = ControlManager.GetBlockControls(Block);
            if (controls.Count > 0)
                Visible = true;
            else if (Visible)
                Hide();
        }

        internal void Hide()
        {
            Visible = false;
            Block = null;
            Destroy(popup);
        }

        /// <summary>
        /// Render window.
        /// </summary>
        private void OnGUI()
        {
            if (Visible && Block != null)
            {
                GUI.skin = Util.Skin;
                windowRect = GUILayout.Window(windowID, windowRect, DoWindow, "Advanced Controls",
                    GUILayout.Width(320),
                    GUILayout.Height(50));
                if (popup != null && !popup.ContainsMouse)
                    Destroy(popup);
            }
        }

        private void DoWindow(int id)
        {
            // Draw controls
            foreach (Control c in controls)
            {
                DrawControl(c);
                GUILayout.Box(GUIContent.none, GUILayout.Height(20));
            }

            if (controls.Count == 0)
                GUILayout.Label("This block has no available controls.");

            // Draw overview button
            if (GUI.Button(new Rect(windowRect.width - 78, 8, 16, 16),
                GUIContent.none, Elements.Buttons.ArrowCollapsed) ||
                GUI.Button(new Rect(windowRect.width - 62, 12, 50, 16),
                "<size=9><b>OVERVIEW</b></size>", Elements.Labels.Default))
            {
                ControlOverview.Open();
            }

            // Drag window
            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }

        private void DrawControl(Control c)
        {
            // Draw control label
            GUILayout.Label(c.Name, Elements.Labels.Title);

            // Draw axis select button
            GUILayout.BeginHorizontal();

            var buttonRect = GUILayoutUtility.GetRect(GUIContent.none, Elements.Buttons.Default);
            if (c.Axis == null)
            {
                if (GUI.Button(buttonRect, "Select Input Axis", Elements.Buttons.Disabled))
                {
                    var callback = new SelectAxisDelegate((InputAxis axis) => { c.Axis = axis.Name; c.Enabled = true; });
                    if (popup == null)
                        popup = AxisSelector.Open(callback, true);
                    else
                        popup.Callback = callback;
                    popup.windowRect.x = windowRect.x + buttonRect.x - 8;
                    popup.windowRect.y = windowRect.y + buttonRect.y - 8;
                }
            }
            else
            {
                var a = AxisManager.Get(c.Axis);
                if (GUI.Button(buttonRect, c.Axis, a != null ? a.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled : Elements.Buttons.Red))
                {
                    var callback = new SelectAxisDelegate((InputAxis axis) => { c.Axis = axis.Name; c.Enabled = true; });
                    if (popup == null)
                        popup = AxisSelector.Open(callback, true);
                    else
                        popup.Callback = callback;
                    popup.windowRect.x = windowRect.x + buttonRect.x - 8;
                    popup.windowRect.y = windowRect.y + buttonRect.y - 8;
                }
                if (a != null && GUILayout.Button("✎", new GUIStyle(Elements.Buttons.Default) { fontSize = 20, padding = new RectOffset(-3, 0, 0, 0) }, GUILayout.Width(30), GUILayout.MaxHeight(28)))
                {
                    var Editor = ACM.Instance.gameObject.AddComponent<AxisEditorWindow>();
                    Editor.windowRect.x = Mathf.Clamp(windowRect.x + windowRect.width,
                                -320 + GUI.skin.window.padding.top, Screen.width - GUI.skin.window.padding.top);
                    Editor.windowRect.y = Mathf.Clamp(windowRect.y, 0, Screen.height - GUI.skin.window.padding.top);
                    Editor.EditAxis(a, new SelectAxisDelegate((InputAxis new_axis) => { c.Axis = new_axis.Name; }));
                }
                if (GUILayout.Button("×", Elements.Buttons.Red, GUILayout.Width(30)))
                {
                    c.Enabled = false;
                    c.Axis = null;
                }
            }

            GUILayout.EndHorizontal();

            if (c.Enabled)
            {
                // Draw graph
                var axis = AxisManager.Get(c.Axis);
                float axis_value = axis != null ? axis.OutputValue : 0;
                float control_value = 0;
                string text;
                if (axis == null)
                {
                    text = "NOT FOUND";
                }
                else
                {
                    if (axis_value > 0)
                        control_value = Mathf.Lerp(c.Center, c.Max, axis_value);
                    else
                        control_value = Mathf.Lerp(c.Center, c.Min, -axis_value);

                    if (axis.Status == AxisStatus.OK)
                        text = control_value.ToString("0.00");
                    else
                        text = InputAxis.GetStatusString(axis.Status);

                }

                GUILayout.Label("<color=#808080><b>" + text + "</b></color>",
                    new GUIStyle(Elements.Labels.Default) { padding = new RectOffset(38, 38, 4, 0), richText = true, alignment = TextAnchor.MiddleLeft },
                    GUILayout.Height(20));

                var graphRect = GUILayoutUtility.GetLastRect();
                graphRect.x += 30;
                graphRect.height += 44;
                graphRect.width -= 60;

                Util.DrawRect(graphRect, Color.gray);
                Util.FillRect(new Rect(
                            graphRect.x + graphRect.width / 2,
                            graphRect.y,
                            1,
                            graphRect.height),
                    Color.gray);

                if (axis != null && axis.Status == AxisStatus.OK)
                    Util.FillRect(new Rect(
                                          graphRect.x + graphRect.width / 2 + graphRect.width / 2 * axis_value,
                                          graphRect.y,
                                          1,
                                          graphRect.height),
                                 Color.yellow);

                // Draw interval input fields
                GUILayout.BeginHorizontal();
                {
                    var oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f, 0.7f);

                    float min_parsed = c.Min;
                    c.min_string = Regex.Replace(
                        GUILayout.TextField(
                            c.min_string, 
                            new GUIStyle(Elements.InputFields.Default) { alignment = TextAnchor.MiddleCenter },
                            GUILayout.Width(60)),
                        @"[^0-9\-.]", "");
                    if (c.min_string != c.Min.ToString() &&
                        !c.min_string.EndsWith(".0") &&
                        !c.min_string.EndsWith(".") &&
                        c.min_string != "-" &&
                        c.min_string != "-0")
                    {
                        float.TryParse(c.min_string, out min_parsed);
                        c.Min = min_parsed;
                        c.min_string = c.Min.ToString();
                    }

                    GUILayout.FlexibleSpace();

                    float cen_parsed = c.Center;
                    c.cen_string = Regex.Replace(
                        GUILayout.TextField(
                            c.cen_string, 
                            new GUIStyle(Elements.InputFields.Default) { alignment = TextAnchor.MiddleCenter },
                            GUILayout.Width(60)),
                        @"[^0-9\-.]", "");
                    if (c.cen_string != c.Center.ToString() &&
                        !c.cen_string.EndsWith(".0") &&
                        !c.cen_string.EndsWith(".") &&
                        c.cen_string != "-" &&
                        c.cen_string != "-0")
                    {
                        float.TryParse(c.cen_string, out cen_parsed);
                        c.Center = cen_parsed;
                        c.cen_string = c.Center.ToString();
                    }

                    GUILayout.FlexibleSpace();

                    float max_parsed = c.Max;
                    c.max_string = Regex.Replace(
                        GUILayout.TextField(
                            c.max_string, 
                            new GUIStyle(Elements.InputFields.Default) { alignment = TextAnchor.MiddleCenter },
                            GUILayout.Width(60)),
                        @"[^0-9\-.]", "");
                    if (c.max_string != c.Max.ToString() &&
                        !c.max_string.EndsWith(".0") &&
                        !c.max_string.EndsWith(".") &&
                        c.max_string != "-" &&
                        c.max_string != "-0")
                    {
                        float.TryParse(c.max_string, out max_parsed);
                        c.Max = max_parsed;
                        c.max_string = c.Max.ToString();
                    }

                    GUI.backgroundColor = oldColor;
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
