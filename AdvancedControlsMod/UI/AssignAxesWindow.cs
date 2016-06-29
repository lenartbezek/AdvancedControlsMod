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

        private Dictionary<string, List<Control>> Controls = new Dictionary<string, List<Control>>();
        private List<string> AxisList = new List<string>();

        private Vector2 scrollPosition = Vector2.zero;

        internal SelectAxisWindow popup;

        internal static AssignAxesWindow Open(bool onload = false)
        {
            BlockHandlers.InitializeBuildingBlockIDs();

            foreach (AssignAxesWindow x in ACM.Instance.gameObject.GetComponents<AssignAxesWindow>())
                Destroy(x);

            var instance = ACM.Instance.gameObject.AddComponent<AssignAxesWindow>();
            instance.windowRect.x = Screen.width - 280 - 200;
            instance.windowRect.y = 200;

            instance.RefreshOverview();

            if (onload)
            {
                bool all = true;
                foreach (string axis in instance.AxisList)
                    all &= AxisManager.Axes.ContainsKey(axis);
                if (all)
                {
                    Destroy(instance);
                    return null;
                }
            }

            instance.Visible = true;
            instance.enabled = true;
            return instance;
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

        private void RefreshOverview()
        {
            Controls.Clear();
            AxisList.Clear();
            foreach (KeyValuePair<Guid, List<Control>> entry in ControlManager.Blocks)
            {
                foreach (Control c in entry.Value)
                {
                    try { BlockHandlers.GetID(c.BlockGUID); } catch { continue; }
                    if (c.Axis == null)
                        continue;
                    if (!Controls.ContainsKey(c.Axis))
                        Controls[c.Axis] = new List<Control>();
                    if (!AxisList.Contains(c.Axis))
                        AxisList.Add(c.Axis);
                    if (!Controls[c.Axis].Contains(c))
                        Controls[c.Axis].Add(c);
                }
            }
        }

        private void DrawAxis(string axis)
        {
            GUILayout.BeginHorizontal();

            var a = AxisManager.Get(axis);
            if (GUILayout.Button(axis, a != null ? a.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled : Elements.Buttons.Red))
                popup = SelectAxisWindow.Open(new SelectAxisDelegate((InputAxis new_axis) => { AssignAxis(axis, new_axis.Name); }));
            Util.DrawEnabledBadge(a != null && a.Saveable);

            GUILayout.EndHorizontal();

            // Draw graph
            float axis_value = a != null ? a.OutputValue : 0;
            string text;
            if (a == null)
                text = "NOT FOUND";
            else if (!a.Connected)
                text = "DISCONNECTED";
            else
                text = axis_value.ToString("0.00");

            GUILayout.Label("  <color=#808080><b>" + text + "</b></color>",
                new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft },
                GUILayout.Height(20));

            var graphRect = GUILayoutUtility.GetLastRect();

            GUILayout.Box(GUIContent.none, GUILayout.Height(8));

            Util.DrawRect(graphRect, Color.gray);
            Util.FillRect(new Rect(
                        graphRect.x + graphRect.width / 2,
                        graphRect.y,
                        1,
                        graphRect.height),
                Color.gray);

            if (a != null && a.Connected)
                Util.FillRect(new Rect(
                                      graphRect.x + graphRect.width / 2 + graphRect.width / 2 * axis_value,
                                      graphRect.y,
                                      1,
                                      graphRect.height),
                             Color.yellow);

            // Draw assigned controls list
            foreach (Control c in Controls[axis])
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("×", new GUIStyle(Elements.Buttons.Red) { margin = new RectOffset(8, 8, 0, 0) }, GUILayout.Width(20), GUILayout.Height(20)))
                {
                    c.Axis = null;
                    c.Enabled = false;
                }
                GUILayout.Label("<b>" + c.Name + "</b> for " + BlockHandlers.GetID(c.BlockGUID), Elements.Labels.LogEntry);
                GUILayout.EndHorizontal();
            }

            GUILayout.Box(GUIContent.none, GUILayout.Height(8));
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
                windowRect = GUILayout.Window(windowID, windowRect, DoWindow, "ACM: Overview",
                    GUILayout.Width(320),
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
            RefreshOverview();

            if (AxisList.Count == 0)
                GUILayout.Label("<b>" + Machine.Active().Name + "</b> uses no advanced controls.");
            else
            {
                GUILayout.Label("<b>" + Machine.Active().Name + "</b> uses these input axes:");

                float viewHeight = 0;
                foreach (KeyValuePair<string, List<Control>> entry in Controls)
                    viewHeight += 84 + entry.Value.Count * 20;

                var oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f, 0.7f);
                scrollPosition = GUILayout.BeginScrollView(scrollPosition,
                    GUILayout.Height(Mathf.Clamp(viewHeight, 180, 640)));
                GUI.backgroundColor = oldColor;

                // Draw axes
                foreach (string axis in AxisList)
                    DrawAxis(axis);

                GUILayout.EndScrollView();
            }

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
