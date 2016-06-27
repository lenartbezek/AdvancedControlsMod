using AdvancedControls.Axes;
using AdvancedControls.Controls;
using LenchScripter;
using spaar.ModLoader.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedControls.UI
{
    internal class AssignAxesWindow : MonoBehaviour
    {
        internal new string name { get { return "Assign Axes window"; } }
        internal bool Visible { get; set; } = false;

        internal int windowID = spaar.ModLoader.Util.GetWindowID();
        internal Rect windowRect = new Rect(360, 200, 100, 100);

        private float DesiredWidth { get; } = 320;
        private float DesiredHeight { get; } = 50;

        private Dictionary<string, List<Control>> Controls = new Dictionary<string, List<Control>>();
        private List<string> AxisList = new List<string>();

        internal SelectAxisWindow popup;

        internal static void Open()
        {
            BlockHandlers.InitializeBuildingBlockIDs();

            foreach (AssignAxesWindow x in ACM.Instance.gameObject.GetComponents<AssignAxesWindow>())
                Destroy(x);

            var instance = ACM.Instance.gameObject.AddComponent<AssignAxesWindow>();

            foreach (KeyValuePair<Guid, List<Control>> entry in ControlManager.Blocks)
            {
                foreach (Control c in entry.Value)
                {
                    try { BlockHandlers.GetID(c.BlockGUID); } catch { continue; }
                    if (c.Axis == null)
                        continue;
                    if (!instance.Controls.ContainsKey(c.Axis))
                        instance.Controls[c.Axis] = new List<Control>();
                    if (!instance.AxisList.Contains(c.Axis))
                        instance.AxisList.Add(c.Axis);
                    instance.Controls[c.Axis].Add(c);
                }
            }

            bool all = true;
            foreach (string axis in instance.AxisList)
                all &= AxisManager.Axes.ContainsKey(axis);
            if (all)
                Destroy(instance);

            instance.Visible = true;
        }

        private void AssignAxis(string old_axis, string new_axis)
        {
            if (old_axis == new_axis) return;
            int index = AxisList.FindIndex(x => x == old_axis);

            foreach (Control c in Controls[old_axis])
                c.Axis = new_axis;

            Controls[new_axis] = Controls[old_axis];
            Controls.Remove(old_axis);
            AxisList[index] = new_axis;
        }

        private void DrawAxis(string axis)
        {
            GUILayout.Box(GUIContent.none, GUILayout.Height(20));

            GUILayout.BeginHorizontal();

            var a = AxisManager.Get(axis);
            if (GUILayout.Button(axis, a != null ? a.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled : Elements.Buttons.Red))
                popup = SelectAxisWindow.Open(new SelectAxisDelegate((InputAxis new_axis) => { AssignAxis(axis, new_axis.Name); }));
            Util.DrawEnabledBadge(a != null && a.Saveable);

            GUILayout.EndHorizontal();

            foreach (Control c in Controls[axis])
                GUILayout.Label("    <b>" + c.Name + "</b> for " + BlockHandlers.GetID(c.BlockGUID));
        }

        private void OnDestroy()
        {
            Destroy(popup);
        }

        /// <summary>
        /// Render window.
        /// </summary>
        protected virtual void OnGUI()
        {
            if (Visible)
            {
                GUI.skin = Util.Skin;
                windowRect = GUILayout.Window(windowID, windowRect, DoWindow, "ACM:  Assign axes",
                    GUILayout.Width(280),
                    GUILayout.Height(100));
                if (popup != null)
                {
                    popup.windowRect.x = windowRect.x + windowRect.width;
                    popup.windowRect.y = windowRect.y;
                }
            }
        }

        protected virtual void DoWindow(int id)
        {
            GUILayout.Label("<b>" + Machine.Active().Name + "</b> uses these input axes:");

            // Draw axes
            foreach (string axis in AxisList)
                DrawAxis(axis);

            // Draw close button
            if (GUI.Button(new Rect(windowRect.width - 38, 8, 30, 30),
                "×", Elements.Buttons.Red))
            {
                Destroy(this);
            }
                
            // Drag window
            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }
    }
}
