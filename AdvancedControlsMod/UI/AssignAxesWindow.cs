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
        private int popupID = spaar.ModLoader.Util.GetWindowID();
        private Rect popupRect;

        private float DesiredWidth { get; } = 320;
        private float DesiredHeight { get; } = 50;

        private Dictionary<string, List<Control>> Controls = new Dictionary<string, List<Control>>();
        private List<string> AxisList = new List<string>();

        internal AxisEditorWindow.SelectAxis Select;

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
                Select = new AxisEditorWindow.SelectAxis((InputAxis new_axis) => { AssignAxis(axis, new_axis.Name); });
            Util.DrawEnabledBadge(a != null && a.Saveable);

            GUILayout.EndHorizontal();

            foreach (Control c in Controls[axis])
                GUILayout.Label("    <b>" + c.Name + "</b> for " + BlockHandlers.GetID(c.BlockGUID));
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
                if (Select != null)
                {
                    popupRect.x = windowRect.x + windowRect.width;
                    popupRect.y = windowRect.y;
                    popupRect = GUILayout.Window(popupID, popupRect, DoPopup, "Select axis",
                        GUILayout.Width(DesiredWidth),
                        GUILayout.Height(DesiredHeight));
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
                Select = null;
                Destroy(this);
            }
                
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

                if (GUILayout.Button(name, axis.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled))
                {
                    Select?.Invoke(axis);
                    Select = null;
                }

                if (GUILayout.Button("✎", new GUIStyle(Elements.Buttons.Default) { fontSize = 20, padding = new RectOffset(-3, 0, 0, 0) }, GUILayout.Width(30), GUILayout.MaxHeight(28)))
                {
                    var Editor = ACM.Instance.gameObject.AddComponent<AxisEditorWindow>();
                    Editor.windowRect.x = popupRect.x;
                    Editor.windowRect.y = popupRect.y;
                    Editor.EditAxis(axis);
                    Select = null;
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
                var Editor = ACM.Instance.gameObject.AddComponent<AxisEditorWindow>();
                Editor.windowRect.x = popupRect.x;
                Editor.windowRect.y = popupRect.y;
                Editor.CreateAxis(new AxisEditorWindow.SelectAxis(Select));
                Select = null;
            }

            // Draw close button
            if (GUI.Button(new Rect(popupRect.width - 38, 8, 30, 30),
                "×", Elements.Buttons.Red))
                Select = null;
        }
    }
}
