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

        private int windowID = spaar.ModLoader.Util.GetWindowID();
        private Rect windowRect = new Rect(100, 100, 100, 100);
        private int popupID = spaar.ModLoader.Util.GetWindowID();
        private Rect popupRect;

        private Vector2 popupScroll = Vector2.zero;

        private float DesiredWidth { get; } = 320;
        private float DesiredHeight { get; } = 50;

        internal GenericBlock block;
        internal Control control;

        public void ShowBlockControls(GenericBlock block)
        { 
            Visible = true;
            this.block = block;
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
                if (control != null)
                {
                    popupRect = GUI.ModalWindow(popupID, popupRect, DoPopup, GUIContent.none, Elements.Windows.ClearDark);
                }
            }
        }

        private void DoWindow(int id)
        {
            var controls = ControlManager.GetBlockControls(block.GetBlockID(), block.Guid);

            foreach(Control c in controls)
            {
                DrawControl(c);
                GUILayout.Label(" ");
            }

            if (controls.Count == 0)
                GUILayout.Label("This block has no available controls.");

            // Draw close button
            if (GUI.Button(new Rect(windowRect.width - 28, 8, 20, 20),
                "×", Elements.Buttons.Red))
                Visible = false;

            // Drag window
            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }

        private void DoPopup(int id)
        {
            string toBeRemoved = null;

            GUILayout.BeginScrollView(popupScroll);

            foreach (KeyValuePair<string, InputAxis> pair in AxisManager.Axes)
            {
                var name = pair.Key;
                var axis = pair.Value;

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(name, Elements.Buttons.Default))
                {
                    control.Axis = name;
                    control = null;
                }

                if (GUILayout.Button("Edit", Elements.Buttons.Default, GUILayout.Width(60)))
                {
                    AdvancedControlsMod.AxisEditor.EditAxis(axis.Clone());
                    control = null;
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
                AdvancedControlsMod.AxisEditor.CreateAxis();
                AdvancedControlsMod.AxisEditor.control = control;
                control = null;
            }

            GUILayout.EndScrollView();
        }

        private void DrawControl(Control c)
        {

            GUILayout.Label(c.Name, Elements.Labels.Title);

            GUILayout.BeginHorizontal();

            if (c.Axis == null)
            {
                var select = GUILayout.Button("Select Input Axis", Elements.Buttons.Disabled);
                if (select)
                {
                    var button_rect = GUILayoutUtility.GetLastRect();
                    popupRect = new Rect(
                        Input.mousePosition.x - 160,
                        Screen.height - (Input.mousePosition.y + 20),
                        320,
                        320);
                    control = c;
                }
            }
            else
            {
                var a = AxisManager.Get(c.Axis);
                var select = GUILayout.Button(c.Axis, a != null ? Elements.Buttons.Default : Elements.Buttons.Red);
                if (select)
                {
                    var button_rect = GUILayoutUtility.GetLastRect();
                    popupRect = new Rect(
                        Input.mousePosition.x - 160,
                        Screen.height - (Input.mousePosition.y - 50),
                        320,
                        320);
                    control = c;
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
