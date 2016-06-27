using System.Collections.Generic;
using UnityEngine;
using spaar.ModLoader.UI;
using AdvancedControls.Controls;
using AdvancedControls.Axes;
using System.Text.RegularExpressions;

namespace AdvancedControls.UI
{
    internal class ControlMapperWindow : MonoBehaviour
    {
        internal bool Visible { get; set; } = false;

        internal int windowID = spaar.ModLoader.Util.GetWindowID();
        internal Rect windowRect = new Rect(680, 115, 100, 100);

        private float DesiredWidth { get; } = 320;
        private float DesiredHeight { get; } = 50;

        internal GenericBlock Block;
        internal List<Control> controls;

        internal SelectAxisWindow popup;

        internal void ShowBlockControls(GenericBlock b)
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
                windowRect = GUILayout.Window(windowID, windowRect, DoWindow, Block.MyBlockInfo.blockName.ToUpper(),
                    GUILayout.Width(DesiredWidth),
                    GUILayout.Height(DesiredHeight));
                if (popup != null)
                {
                    popup.windowRect.x = windowRect.x + windowRect.width;
                    popup.windowRect.y = windowRect.y;
                }
            }
        }

        private void DoWindow(int id)
        {
            foreach(Control c in controls)
            {
                DrawControl(c);
                GUILayout.Label(" ");
            }

            if (controls.Count == 0)
                GUILayout.Label("This block has no available controls.");

            // Drag window
            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }

        private void DrawControl(Control c)
        {

            GUILayout.Label(c.Name, Elements.Labels.Title);

            GUILayout.BeginHorizontal();

            if (c.Axis == null)
            {
                if (GUILayout.Button("Select Input Axis", Elements.Buttons.Disabled))
                {
                    Destroy(popup);
                    popup = SelectAxisWindow.Open(
                        new SelectAxisDelegate((InputAxis axis) => { c.Axis = axis.Name; c.Enabled = true; }));
                }
            }
            else
            {
                var a = AxisManager.Get(c.Axis);
                if (GUILayout.Button(c.Axis, a != null ? a.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled : Elements.Buttons.Red))
                {
                    Destroy(popup);
                    popup = SelectAxisWindow.Open(
                        new SelectAxisDelegate((InputAxis axis) => { c.Axis = axis.Name; c.Enabled = true; }));
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
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Mininum");
                    float min_parsed = c.Min;
                    c.min_string = Regex.Replace(GUILayout.TextField(c.min_string), @"[^0-9\-.]", "");
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
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    GUILayout.Label("Center");
                    float cen_parsed = c.Center;
                    c.cen_string = Regex.Replace(GUILayout.TextField(c.cen_string), @"[^0-9\-.]", "");
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
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    GUILayout.Label("Maximum");
                    float max_parsed = c.Max;
                    c.max_string = Regex.Replace(GUILayout.TextField(c.max_string), @"[^0-9\-.]", "");
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
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
