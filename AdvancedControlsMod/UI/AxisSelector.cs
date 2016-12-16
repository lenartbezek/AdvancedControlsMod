using System.Collections.Generic;
using Lench.AdvancedControls.Axes;
using spaar.ModLoader.UI;
using UnityEngine;

// ReSharper disable UnusedMember.Local
// ReSharper disable LocalVariableHidesMember

namespace Lench.AdvancedControls.UI
{
    internal delegate void SelectAxisDelegate(InputAxis axis);

    internal class AxisSelector : MonoBehaviour
    {
        internal int WindowID = spaar.ModLoader.Util.GetWindowID();
        internal Rect WindowRect = new Rect(0, 0, 320, 42);

        internal bool ContainsMouse
        {
            get
            {
                var mousePos = UnityEngine.Input.mousePosition;
                mousePos.y = Screen.height - mousePos.y;
                return WindowRect.Contains(mousePos);
            }
        }

        internal SelectAxisDelegate Callback;

        private bool _compact;
        private Vector2 _localScrollPosition = Vector2.zero;
        private Vector2 _machineScrollPosition = Vector2.zero;

        internal static AxisSelector Open(SelectAxisDelegate callback, bool compact = false)
        {
            var window = ACM.Instance.gameObject.AddComponent<AxisSelector>();
            window.Callback = callback;
            window._compact = compact;
            return window;
        }

        private void OnGUI()
        {
            GUI.skin = Util.Skin;
            GUIStyle windowStyle = _compact ? Util.CompactWindowStyle : Util.FullWindowStyle;
            WindowRect = GUILayout.Window(WindowID, WindowRect, DoWindow, _compact ? string.Empty : Strings.AxisSelector_WindowTitle_SelectAxis, windowStyle,
                        GUILayout.Width(320),
                        GUILayout.Height(42));
        }

        private void DoWindow(int id)
        {
            // Draw local axes
            if (AxisManager.LocalAxes.Count > 0)
            {
                _localScrollPosition = GUILayout.BeginScrollView(_localScrollPosition,
                    GUILayout.Height(Mathf.Clamp(AxisManager.LocalAxes.Count * 36 + 30, 138, 246)));

                GUILayout.Label(Strings.AxisSelector_Label_LocallySavedAxes, new GUIStyle(Elements.Labels.Title) { alignment = TextAnchor.MiddleCenter });

                string toBeRemoved = null;

                foreach (KeyValuePair<string, InputAxis> pair in AxisManager.LocalAxes)
                {
                    var name = pair.Key;
                    var axis = pair.Value;

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button(name, axis.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled))
                    {
                        Callback?.Invoke(axis);
                        Destroy(this);
                    }

                    if (GUILayout.Button(Strings.ButtonText_EditAxis,
                        new GUIStyle(Elements.Buttons.Default) { fontSize = 20, padding = new RectOffset(-3, 0, 0, 0) },
                        GUILayout.Width(30), GUILayout.MaxHeight(28)))
                    {
                        var editor = ACM.Instance.gameObject.AddComponent<AxisEditorWindow>();
                        editor.WindowRect.x = Mathf.Clamp(WindowRect.x + WindowRect.width,
                            -320 + GUI.skin.window.padding.top, Screen.width - GUI.skin.window.padding.top);
                        editor.WindowRect.y = Mathf.Clamp(WindowRect.y - 40, 0, Screen.height - GUI.skin.window.padding.top);
                        editor.EditAxis(axis);
                    }

                    if (GUILayout.Button(Strings.ButtonText_Close, Elements.Buttons.Red, GUILayout.Width(30)))
                    {
                        toBeRemoved = name;
                    }

                    GUILayout.EndHorizontal();
                }

                if (toBeRemoved != null)
                    AxisManager.RemoveLocalAxis(toBeRemoved);

                GUILayout.EndScrollView();
            }

            // Draw machine axes
            if (AxisManager.MachineAxes.Count > 0)
            {
                _machineScrollPosition = GUILayout.BeginScrollView(_machineScrollPosition,
                    GUILayout.Height(Mathf.Clamp(AxisManager.MachineAxes.Count * 36 + 30, 138, 246)));

                GUILayout.Label(Strings.AxisSelector_Label_MachineEmbeddedAxes,
                    new GUIStyle(Elements.Labels.Title) { alignment = TextAnchor.MiddleCenter });

                string toBeRemoved = null;

                foreach (KeyValuePair<string, InputAxis> pair in AxisManager.MachineAxes)
                {
                    var name = pair.Key;
                    var axis = pair.Value;

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button(name, axis.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled))
                    {
                        Callback?.Invoke(axis);
                        Destroy(this);
                    }

                    if (GUILayout.Button(Strings.ButtonText_EditAxis,
                        new GUIStyle(Elements.Buttons.Default) { fontSize = 20, padding = new RectOffset(-3, 0, 0, 0) },
                        GUILayout.Width(30), GUILayout.MaxHeight(28)))
                    {
                        var editor = ACM.Instance.gameObject.AddComponent<AxisEditorWindow>();
                        editor.WindowRect.x = Mathf.Clamp(WindowRect.x + WindowRect.width,
                            -320 + GUI.skin.window.padding.top, Screen.width - GUI.skin.window.padding.top);
                        editor.WindowRect.y = Mathf.Clamp(WindowRect.y - 40, 0, Screen.height - GUI.skin.window.padding.top);
                        editor.EditAxis(axis);
                    }

                    if (GUILayout.Button(Strings.ButtonText_Close, Elements.Buttons.Red, GUILayout.Width(30)))
                    {
                        toBeRemoved = name;
                    }

                    GUILayout.EndHorizontal();
                }

                if (toBeRemoved != null)
                    AxisManager.RemoveMachineAxis(toBeRemoved);

                GUILayout.EndScrollView();
            }

            if (GUILayout.Button(Strings.AxisEditorWindow_WindowTitle_CreateNewAxis, Elements.Buttons.Disabled))
            {
                var editor = ACM.Instance.gameObject.AddComponent<AxisEditorWindow>();
                editor.WindowRect.x = Mathf.Clamp(WindowRect.x + WindowRect.width,
                    -320 + GUI.skin.window.padding.top, Screen.width - GUI.skin.window.padding.top);
                editor.WindowRect.y = Mathf.Clamp(WindowRect.y - 40, 0, Screen.height - GUI.skin.window.padding.top);
                editor.CreateAxis(Callback);
                Destroy(this);
            }

            // Draw close button
            if (!_compact)
                if (GUI.Button(new Rect(WindowRect.width - 38, 8, 30, 30),
                    Strings.ButtonText_Close, Elements.Buttons.Red))
                    Destroy(this);
        }
    }
}
