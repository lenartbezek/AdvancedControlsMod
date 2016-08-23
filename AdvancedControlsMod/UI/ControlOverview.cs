using Lench.AdvancedControls.Axes;
using Lench.AdvancedControls.Controls;
using spaar.ModLoader.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lench.AdvancedControls.UI
{
    internal class ControlOverview : MonoBehaviour
    {
        internal int windowID = spaar.ModLoader.Util.GetWindowID();
        internal Rect windowRect = new Rect();

        private Dictionary<string, List<Control>> Controls = new Dictionary<string, List<Control>>();
        private List<string> AxisList = new List<string>();

        private Vector2 scrollPosition = Vector2.zero;

        internal AxisSelector popup;

        internal static ControlOverview Open(bool onload = false)
        {
            BlockHandlerController.InitializeBuildingBlockIDs();

            foreach (ControlOverview x in ACM.Instance.gameObject.GetComponents<ControlOverview>())
                Destroy(x);

            var instance = ACM.Instance.gameObject.AddComponent<ControlOverview>();
            instance.enabled = false;

            instance.windowRect.x = Screen.width - 280 - 400;
            instance.windowRect.y = 200;

            instance.RefreshOverview();

            if (onload)
            {
                if (instance.AxisList.TrueForAll(name =>
                {
                    var axis = AxisManager.Get(name);
                    return axis != null && (axis.Status == AxisStatus.OK || axis.Status == AxisStatus.NotRunning);
                }))
                {
                    Destroy(instance);
                    return null;
                }
            }

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
                    try { BlockHandlerController.GetID(c.BlockGUID); } catch { continue; }
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

            var buttonRect = GUILayoutUtility.GetRect(new GUIContent(" "), Elements.Buttons.Default);
            var a = AxisManager.Get(axis);
            if (GUI.Button(buttonRect, axis, a != null ? a.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled : Elements.Buttons.Red))
            {
                var callback = new SelectAxisDelegate((InputAxis new_axis) => { AssignAxis(axis, new_axis.Name); });
                if (popup == null)
                    popup = AxisSelector.Open(callback, true);
                else
                    popup.Callback = callback;
                popup.windowRect.x = windowRect.x + buttonRect.x - 8;
                popup.windowRect.y = windowRect.y + GUI.skin.window.padding.top + buttonRect.y - scrollPosition.y - 8;
            }

            if (a != null && GUILayout.Button("✎", new GUIStyle(Elements.Buttons.Default) { fontSize = 20, padding = new RectOffset(-3, 0, 0, 0) }, GUILayout.Width(30), GUILayout.MaxHeight(28)))
            {
                var Editor = ACM.Instance.gameObject.AddComponent<AxisEditorWindow>();
                Editor.windowRect.x = Mathf.Clamp(windowRect.x + windowRect.width,
                            -320 + GUI.skin.window.padding.top, Screen.width - GUI.skin.window.padding.top);
                Editor.windowRect.y = Mathf.Clamp(windowRect.y, 0, Screen.height - GUI.skin.window.padding.top);
                Editor.EditAxis(a, new SelectAxisDelegate((InputAxis new_axis) => { AssignAxis(axis, new_axis.Name); }));
            }

            GUILayout.EndHorizontal();

            // Draw graph
            string text;
            if (a == null)
                text = "NOT FOUND";
            else if (a.Status != AxisStatus.OK)
                text = InputAxis.GetStatusString(a.Status);
            else
                text = "";

            GUILayout.Label("  <color=#808080><b>" + text + "</b></color>",
                new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft, margin = new RectOffset(8, 8, 8, 8) },
                GUILayout.Height(20));

            var graphRect = GUILayoutUtility.GetLastRect();

            Util.DrawRect(graphRect, Color.gray);
            Util.FillRect(new Rect(
                        graphRect.x + graphRect.width / 2,
                        graphRect.y,
                        1,
                        graphRect.height),
                Color.gray);

            if (a != null && a.Status == AxisStatus.OK)
                Util.FillRect(new Rect(
                                      graphRect.x + graphRect.width / 2 + graphRect.width / 2 * a.OutputValue,
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
                string block_name;
                try
                {
                    block_name = BlockHandlerController.GetID(c.BlockGUID);
                }
                catch
                {
                    block_name = "...";
                }
                GUILayout.Label("<b>" + c.Name + "</b> for " + block_name, Elements.Labels.LogEntry);
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
            GUI.skin = Util.Skin;
            windowRect = GUILayout.Window(windowID, windowRect, DoWindow, "Overview",
                GUILayout.Width(320),
                GUILayout.Height(42));
            if (popup != null && !popup.ContainsMouse)
                Destroy(popup);
        }

        internal bool ContainsMouse
        {
            get
            {
                var mousePos = UnityEngine.Input.mousePosition;
                mousePos.y = Screen.height - mousePos.y;
                return windowRect.Contains(mousePos);
            }
        }

        protected virtual void DoWindow(int id)
        {
            RefreshOverview();

            if (AxisList.Count == 0)
                GUILayout.Label("<b>" + Machine.Active().Name + "</b> uses no advanced controls.");
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, new GUIStyle(Elements.Scrollview.ThumbVertical) { normal = new GUIStyleState(), padding = new RectOffset() }, GUILayout.Width(304), GUILayout.Height(500));
                
                GUILayout.Label("To use this machine as intended,\nmake sure all axes report no problems.\n\n<b>" + Machine.Active().Name + "</b> uses these input axes:");

                // Draw axes
                foreach (string axis in AxisList)
                    DrawAxis(axis);

                GUILayout.EndScrollView();
            }

            // Draw help button
            if (GUI.Button(new Rect(windowRect.width - 76, 8, 30, 30),
                "?", Elements.Buttons.Red))
            {
                Application.OpenURL("https://github.com/lench4991/AdvancedControlsMod/wiki/Sharing");
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
