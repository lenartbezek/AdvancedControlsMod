using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using spaar.ModLoader.UI;
using Lench.AdvancedControls.Controls;
using Lench.AdvancedControls.Axes;
using Lench.AdvancedControls.Resources;

// ReSharper disable UnusedMember.Local
// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace Lench.AdvancedControls.UI
{
    internal class ControlMapper : MonoBehaviour
    {
        internal bool Visible { get; set; }

        private readonly int _windowID = spaar.ModLoader.Util.GetWindowID();
        private Rect _windowRect = new Rect(680, 115, 320, 50);

        public BlockBehaviour Block;
        public List<Control> Controls;

        public AxisSelector Popup;

        public void ShowBlockControls(BlockBehaviour b)
        {
            Block = b;
            Controls = ControlManager.GetBlockControls(Block);
            if (Controls.Count > 0)
                Visible = true;
            else if (Visible)
                Hide();
        }

        public void Hide()
        {
            Visible = false;
            Block = null;
            Destroy(Popup);
        }

        public bool ContainsMouse
        {
            get
            {
                var mousePos = UnityEngine.Input.mousePosition;
                mousePos.y = Screen.height - mousePos.y;
                return _windowRect.Contains(mousePos);
            }
        }

        private void OnGUI()
        {
            if (!Visible || Block == null) return;
            GUI.skin = Util.Skin;
            _windowRect = GUILayout.Window(_windowID, _windowRect, DoWindow, Strings.ControlMapper_WindowTitle_AdvancedControls,
                GUILayout.Width(320),
                GUILayout.Height(50));
            if (Popup != null && !Popup.ContainsMouse)
                Destroy(Popup);
        }

        private void DoWindow(int id)
        {
            // Draw controls
            foreach (Control c in Controls)
            {
                DrawControl(c);
                GUILayout.Box(GUIContent.none, GUILayout.Height(20));
            }

            if (Controls.Count == 0)
                GUILayout.Label(Strings.ControlMapper_Message_NoAvailableControls);

            // Draw overview button
            if (GUI.Button(new Rect(_windowRect.width - 78, 8, 16, 16),
                GUIContent.none, Elements.Buttons.ArrowCollapsed) ||
                GUI.Button(new Rect(_windowRect.width - 62, 8, 50, 16),
                $"<size=9><b>{Strings.ControlMapper_ButtonText_Overview}</b></size>", Elements.Labels.Default))
            {
                ControlOverview.Open();
            }

            // Drag window
            GUI.DragWindow(new Rect(0, 0, _windowRect.width, GUI.skin.window.padding.top));
        }

        private void DrawControl(Control c)
        {
            // Draw control label
            GUILayout.Label(c.Key, Elements.Labels.Title);

            // Draw axis select button
            GUILayout.BeginHorizontal();

            var buttonRect = GUILayoutUtility.GetRect(GUIContent.none, Elements.Buttons.Default);
            var selectAxisClicked = false;
            if (c.Axis == null)
                selectAxisClicked |= GUI.Button(buttonRect, Strings.ButtonText_SelectInputAxis,
                    Elements.Buttons.Disabled);
            else
            {
                var a = AxisManager.Get(c.Axis);
                selectAxisClicked |= GUI.Button(buttonRect, c.Axis,
                    a != null ? a.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled : Elements.Buttons.Red);

                if (a != null && GUILayout.Button(Strings.ButtonText_EditAxis, new GUIStyle(Elements.Buttons.Default) { fontSize = 20, padding = new RectOffset(-3, 0, 0, 0) }, GUILayout.Width(30), GUILayout.MaxHeight(28)))
                {
                    var editor = AxisEditorWindow.EditAxis(a, (newAxis) => { c.Axis = newAxis.Name; });
                    editor.Position = new Vector2(
                        Mathf.Clamp(_windowRect.x + _windowRect.width,
                            -320 + GUI.skin.window.padding.top, Screen.width - GUI.skin.window.padding.top),
                        Mathf.Clamp(_windowRect.y - 40, 0,
                        Screen.height - GUI.skin.window.padding.top));
                }
                if (GUILayout.Button(Strings.ButtonText_Close, Elements.Buttons.Red, GUILayout.Width(30)))
                {
                    c.Enabled = false;
                    c.Axis = null;
                }
            }

            if (selectAxisClicked)
            {
                Action<InputAxis> callback = axis => { c.Axis = axis.Name; c.Enabled = true; };
                if (Popup == null)
                {
                    Popup = AxisSelector.Open(callback);
                    Popup.Position = new Vector2(
                        _windowRect.x + buttonRect.x - 8,
                        _windowRect.y + buttonRect.y - 8);
                }
                else
                    Popup.OnAxisSelect = callback;
            }

            GUILayout.EndHorizontal();

            if (c.Enabled)
            {
                // Draw graph
                var axis = AxisManager.Get(c.Axis);
                var axisValue = axis?.OutputValue ?? 0;
                string text;
                if (axis == null)
                {
                    text = InputAxis.GetStatusString(AxisStatus.NotFound);
                }
                else
                {
                    var controlValue = axisValue > 0 
                        ? Mathf.Lerp(c.Center, c.Max, axisValue) 
                        : Mathf.Lerp(c.Center, c.Min, -axisValue);

                    text = axis.Status == AxisStatus.OK 
                        ? controlValue.ToString("0.00") 
                        : InputAxis.GetStatusString(axis.Status);
                }

                GUILayout.Label($"<color=#808080><b>{text}</b></color>",
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
                                          graphRect.x + graphRect.width / 2 + graphRect.width / 2 * axisValue,
                                          graphRect.y,
                                          1,
                                          graphRect.height),
                                 Color.yellow);

                // Draw invert button
                if (GUI.Button(new Rect(graphRect.x + graphRect.width - 28, graphRect.y, 28, 28), "<size=24><color=#808080>⇄</color></size>", Elements.Labels.Default))
                    c.Invert();

                // Draw interval input fields
                GUILayout.BeginHorizontal();
                {
                    var oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f, 0.7f);

                    c.MinString = Regex.Replace(
                        GUILayout.TextField(
                            c.MinString, 
                            new GUIStyle(Elements.InputFields.Default) { alignment = TextAnchor.MiddleCenter },
                            GUILayout.Width(60)),
                        @"[^0-9\-.]", "");
                    if (c.MinString != c.Min.ToString() &&
                        !c.MinString.EndsWith(".0") &&
                        !c.MinString.EndsWith(".") &&
                        c.MinString != "-" &&
                        c.MinString != "-0")
                    {
                        float.TryParse(c.MinString, out float minParsed);
                        c.Min = minParsed;
                        c.MinString = c.Min.ToString();
                    }

                    GUILayout.FlexibleSpace();

                    c.CenString = Regex.Replace(
                        GUILayout.TextField(
                            c.CenString, 
                            new GUIStyle(Elements.InputFields.Default) { alignment = TextAnchor.MiddleCenter },
                            GUILayout.Width(60)),
                        @"[^0-9\-.]", "");
                    if (c.CenString != c.Center.ToString() &&
                        !c.CenString.EndsWith(".0") &&
                        !c.CenString.EndsWith(".") &&
                        c.CenString != "-" &&
                        c.CenString != "-0")
                    {
                        float.TryParse(c.CenString, out float cenParsed);
                        c.Center = cenParsed;
                        c.CenString = c.Center.ToString();
                    }

                    GUILayout.FlexibleSpace();

                    c.MaxString = Regex.Replace(
                        GUILayout.TextField(
                            c.MaxString, 
                            new GUIStyle(Elements.InputFields.Default) { alignment = TextAnchor.MiddleCenter },
                            GUILayout.Width(60)),
                        @"[^0-9\-.]", "");
                    if (c.MaxString != c.Max.ToString() &&
                        !c.MaxString.EndsWith(".0") &&
                        !c.MaxString.EndsWith(".") &&
                        c.MaxString != "-" &&
                        c.MaxString != "-0")
                    {
                        float.TryParse(c.MaxString, out float maxParsed);
                        c.Max = maxParsed;
                        c.MaxString = c.Max.ToString();
                    }

                    GUI.backgroundColor = oldColor;
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
