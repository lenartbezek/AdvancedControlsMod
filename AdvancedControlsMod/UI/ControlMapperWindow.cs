using System.Collections.Generic;
using UnityEngine;
using spaar.ModLoader.UI;
using AdvancedControls.Controls;
using AdvancedControls.Axes;

namespace AdvancedControls.UI
{
    public class ControlMapperWindow : MonoBehaviour
    {
        public new string name { get { return "Control Mapper window"; } }

        public bool Visible { get; set; } = false;

        internal int windowID = spaar.ModLoader.Util.GetWindowID();
        internal Rect windowRect = new Rect(680, 115, 100, 100);
        private int popupID = spaar.ModLoader.Util.GetWindowID();
        private Rect popupRect;

        private Vector2 popupScroll = Vector2.zero;

        private float DesiredWidth { get; } = 320;
        private float DesiredHeight { get; } = 50;

        private GenericBlock block;
        private List<Control> controls;

        internal Control selectedControl;

        public void ShowBlockControls(GenericBlock b)
        { 
            Visible = true;
            block = b;
            controls = ControlManager.GetBlockControls(block.GetBlockID(), block.Guid);
        }

        public void Hide()
        {
            Visible = false;
            block = null;
            selectedControl = null;
        }

        /// <summary>
        /// Render window.
        /// </summary>
        private void OnGUI()
        {
            if (Visible && block != null)
            {
                GUI.skin = Util.Skin;
                windowRect = GUILayout.Window(windowID, windowRect, DoWindow, block.MyBlockInfo.blockName.ToUpper(),
                    GUILayout.Width(DesiredWidth),
                    GUILayout.Height(DesiredHeight));
                if (selectedControl != null)
                {
                    popupRect.x = windowRect.x + windowRect.width;
                    popupRect.y = windowRect.y;
                    popupRect = GUILayout.Window(popupID, popupRect, DoPopup, "Select axis",
                        GUILayout.Width(DesiredWidth),
                        GUILayout.Height(DesiredHeight));
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

        private void DoPopup(int id)
        {
            string toBeRemoved = null;

            foreach (KeyValuePair<string, InputAxis> pair in AxisManager.Axes)
            {
                var name = pair.Key;
                var axis = pair.Value;

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(name, Elements.Buttons.Default))
                {
                    selectedControl.Axis = name;
                    selectedControl.Enabled = true;
                    selectedControl = null;
                }

                if (GUILayout.Button("Edit", Elements.Buttons.Default, GUILayout.Width(60)))
                {
                    if (!AdvancedControlsMod.AxisEditor.Visible)
                    {
                        AdvancedControlsMod.AxisEditor.windowRect.x = popupRect.x;
                        AdvancedControlsMod.AxisEditor.windowRect.y = popupRect.y;
                    }
                    AdvancedControlsMod.AxisEditor.EditAxis(axis.Clone());
                    selectedControl = null;
                }

                if (GUILayout.Button("×", Elements.Buttons.Red, GUILayout.Width(30)))
                {
                    toBeRemoved = name;
                }

                GUILayout.EndHorizontal();
            }

            if (toBeRemoved != null)
                AxisManager.Remove(toBeRemoved);

            if (GUILayout.Button("Create new axis", Elements.Buttons.Disabled))
            {
                if (!AdvancedControlsMod.AxisEditor.Visible)
                {
                    AdvancedControlsMod.AxisEditor.windowRect.x = popupRect.x;
                    AdvancedControlsMod.AxisEditor.windowRect.y = popupRect.y;
                }
                AdvancedControlsMod.AxisEditor.CreateAxis();
                AdvancedControlsMod.AxisEditor.control = selectedControl;
                selectedControl = null;
            }

            // Draw close button
            if (GUI.Button(new Rect(popupRect.width - 28, 8, 20, 20),
                "×", Elements.Buttons.Red))
                selectedControl = null;
        }

        private void DrawControl(Control c)
        {

            GUILayout.Label(c.Name, Elements.Labels.Title);

            GUILayout.BeginHorizontal();

            if (c.Axis == null)
            {
                if (GUILayout.Button("Select Input Axis", Elements.Buttons.Disabled))
                {
                    selectedControl = c;
                }
            }
            else
            {
                var a = AxisManager.Get(c.Axis);
                if (GUILayout.Button(c.Axis, a != null ? Elements.Buttons.Default : Elements.Buttons.Red))
                {
                    selectedControl = c;
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
                    c.min = GUILayout.TextField(c.min);
                    if (!c.min.EndsWith(".") && !c.min.EndsWith("-"))
                    {
                        float.TryParse(c.min, out min_parsed);
                        c.Min = min_parsed;
                        c.min = (Mathf.Round(c.Min * 100) / 100).ToString();
                    }
                    GUILayout.EndVertical();

                    if (!c.PositiveOnly)
                    {
                        GUILayout.BeginVertical();
                        GUILayout.Label("Center");
                        float cen_parsed = c.Center;
                        c.cen = GUILayout.TextField(c.cen);
                        if (!c.cen.EndsWith(".") && !c.cen.EndsWith("-"))
                        {
                            float.TryParse(c.cen, out cen_parsed);
                            c.Center = cen_parsed;
                            c.cen = (Mathf.Round(c.Center * 100) / 100).ToString();
                        }
                        GUILayout.EndVertical();
                    }

                    GUILayout.BeginVertical();
                    GUILayout.Label("Maximum");
                    float max_parsed = c.Max;
                    c.max = GUILayout.TextField(c.max);
                    if (!c.max.EndsWith(".") && !c.max.EndsWith("-"))
                    {
                        float.TryParse(c.max, out max_parsed);
                        c.Max = max_parsed;
                        c.max = (Mathf.Round(c.Max * 100) / 100).ToString();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
