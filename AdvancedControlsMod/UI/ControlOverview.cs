using System.Collections.Generic;
using Lench.AdvancedControls.Axes;
using Lench.AdvancedControls.Controls;
using spaar.ModLoader.UI;
using Steamworks;
using UnityEngine;
// ReSharper disable UnusedMember.Local

namespace Lench.AdvancedControls.UI
{
    internal class ControlOverview : MonoBehaviour
    {
        internal int WindowID = spaar.ModLoader.Util.GetWindowID();
        internal Rect WindowRect;

        private readonly Dictionary<string, List<Control>> _controls = new Dictionary<string, List<Control>>();
        private readonly List<string> _axisList = new List<string>();

        private Vector2 _scrollPosition = Vector2.zero;

        internal AxisSelector Popup;

        internal static ControlOverview Open(bool onload = false)
        {
            BlockHandlerController.InitializeBuildingBlockIDs();

            foreach (var x in ACM.Instance.gameObject.GetComponents<ControlOverview>())
                Destroy(x);

            var instance = ACM.Instance.gameObject.AddComponent<ControlOverview>();
            instance.enabled = false;

            instance.WindowRect.x = Screen.width - 280 - 400;
            instance.WindowRect.y = 200;

            instance.RefreshOverview();

            if (onload)
            {
                if (instance._axisList.TrueForAll(name =>
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

        private void AssignAxis(string oldAxis, string newAxis)
        {
            if (oldAxis == newAxis) return;
            int index = _axisList.FindIndex(x => x == oldAxis);

            foreach (Control c in _controls[oldAxis])
                c.Axis = newAxis;

            _controls[newAxis] = _controls[oldAxis];
            _controls.Remove(oldAxis);
            _axisList[index] = newAxis;
        }

        private void RefreshOverview()
        {
            _controls.Clear();
            _axisList.Clear();
            foreach (var entry in ControlManager.Blocks)
            {
                foreach (Control c in entry.Value)
                {
                    try { BlockHandlerController.GetID(c.BlockGUID); } catch { continue; }
                    if (c.Axis == null)
                        continue;
                    if (!_controls.ContainsKey(c.Axis))
                        _controls[c.Axis] = new List<Control>();
                    if (!_axisList.Contains(c.Axis))
                        _axisList.Add(c.Axis);
                    if (!_controls[c.Axis].Contains(c))
                        _controls[c.Axis].Add(c);
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
                var callback = new SelectAxisDelegate(newAxis => { AssignAxis(axis, newAxis.Name); });
                if (Popup == null)
                    Popup = AxisSelector.Open(callback, true);
                else
                    Popup.Callback = callback;
                Popup.WindowRect.x = WindowRect.x + buttonRect.x - 8;
                Popup.WindowRect.y = WindowRect.y + GUI.skin.window.padding.top + buttonRect.y - _scrollPosition.y - 8;
            }

            if (a != null && GUILayout.Button(Strings.ButtonText_EditAxis, new GUIStyle(Elements.Buttons.Default) { fontSize = 20, padding = new RectOffset(-3, 0, 0, 0) }, GUILayout.Width(30), GUILayout.MaxHeight(28)))
            {
                var editor = ACM.Instance.gameObject.AddComponent<AxisEditorWindow>();
                editor.WindowRect.x = Mathf.Clamp(WindowRect.x + WindowRect.width,
                            -320 + GUI.skin.window.padding.top, Screen.width - GUI.skin.window.padding.top);
                editor.WindowRect.y = Mathf.Clamp(WindowRect.y, 0, Screen.height - GUI.skin.window.padding.top);
                editor.EditAxis(a, newAxis => { AssignAxis(axis, newAxis.Name); });
            }

            GUILayout.EndHorizontal();

            // Draw graph
            string text;
            if (a == null)
                text = InputAxis.GetStatusString(AxisStatus.NotFound);
            else if (a.Status != AxisStatus.OK)
                text = InputAxis.GetStatusString(a.Status);
            else
                text = string.Empty;

            GUILayout.Label($"  <color=#808080><b>{text}</b></color>",
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
            foreach (Control c in _controls[axis])
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Strings.ButtonText_Close, new GUIStyle(Elements.Buttons.Red) { margin = new RectOffset(8, 8, 0, 0) }, GUILayout.Width(20), GUILayout.Height(20)))
                {
                    c.Axis = null;
                    c.Enabled = false;
                }
                string blockName;
                try
                {
                    blockName = BlockHandlerController.GetID(c.BlockGUID);
                }
                catch
                {
                    blockName = "...";
                }
                GUILayout.Label(string.Format(Strings.ControlOverview_List_Entry, c.Key, blockName), Elements.Labels.LogEntry);
                GUILayout.EndHorizontal();
            }

            GUILayout.Box(GUIContent.none, GUILayout.Height(8));
        }

        private void OnDestroy()
        {
            Destroy(Popup);
        }

        /// <summary>
        /// Render window.
        /// </summary>
        protected virtual void OnGUI()
        {
            GUI.skin = Util.Skin;
            WindowRect = GUILayout.Window(WindowID, WindowRect, DoWindow, Strings.ControlOverview_WindowTitle,
                GUILayout.Width(320),
                GUILayout.Height(42));
            if (Popup != null && !Popup.ContainsMouse)
                Destroy(Popup);
        }

        internal bool ContainsMouse
        {
            get
            {
                var mousePos = UnityEngine.Input.mousePosition;
                mousePos.y = Screen.height - mousePos.y;
                return WindowRect.Contains(mousePos);
            }
        }

        protected virtual void DoWindow(int id)
        {
            RefreshOverview();

            if (_axisList.Count == 0)
                GUILayout.Label(string.Format(Strings.ControlOverview_Label_NoControls, Machine.Active().Name));
            else
            {
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, new GUIStyle(Elements.Scrollview.ThumbVertical) { normal = new GUIStyleState(), padding = new RectOffset() }, GUILayout.Width(304), GUILayout.Height(500));
                
                GUILayout.Label(string.Format(Strings.ControlOverview_Message_IntroNote, Machine.Active().Name));

                // Draw axes
                foreach (string axis in _axisList)
                    DrawAxis(axis);

                GUILayout.EndScrollView();
            }

            // Draw help button
            if (GUI.Button(new Rect(WindowRect.width - 76, 8, 30, 30),
                Strings.ButtonText_Help, Elements.Buttons.Red))
            {
                var url = Strings.ControlOverview_HelpURL;
                try
                {
                    SteamFriends.ActivateGameOverlayToWebPage(url);
                }
                catch
                {
                    Application.OpenURL(url);
                }
            }

            // Draw close button
            if (GUI.Button(new Rect(WindowRect.width - 38, 8, 30, 30),
                Strings.ButtonText_Close, Elements.Buttons.Red))
            {
                Destroy(this);
            }

            // Drag window
            GUI.DragWindow(new Rect(0, 0, WindowRect.width, GUI.skin.window.padding.top));
        }
    }
}
