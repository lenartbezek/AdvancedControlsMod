using Lench.AdvancedControls.Axes;
using spaar.ModLoader.UI;
using System;
using UnityEngine;

// ReSharper disable UnusedMember.Local
// ReSharper disable LocalVariableHidesMember

namespace Lench.AdvancedControls.UI
{
    internal class AxisSelector : MonoBehaviour
    {
        private readonly int _windowID = spaar.ModLoader.Util.GetWindowID();

        private Vector2 _localScrollPosition = Vector2.zero;
        private Vector2 _machineScrollPosition = Vector2.zero;
        private Rect _windowRect = new Rect(0, 0, 320, 42);

        public Vector2 Position
        {
            get { return _windowRect.position; }
            set { _windowRect.position = value; }
        }

        public Action<InputAxis> OnAxisSelect;

        public bool ContainsMouse
        {
            get
            {
                var mousePos = UnityEngine.Input.mousePosition;
                mousePos.y = Screen.height - mousePos.y;
                return _windowRect.Contains(mousePos);
            }
        }

        public static AxisSelector Open(Action<InputAxis> callback)
        {
            var window = Mod.Controller.AddComponent<AxisSelector>();
            window.OnAxisSelect = callback;
            return window;
        }

        private void OnGUI()
        {
            GUI.skin = Util.Skin;
            _windowRect = GUILayout.Window(_windowID, _windowRect, DoWindow, string.Empty, Util.CompactWindowStyle,
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

                GUILayout.Label(Strings.AxisSelector_Label_LocallySavedAxes,
                    new GUIStyle(Elements.Labels.Title) {alignment = TextAnchor.MiddleCenter});

                string toBeRemoved = null;

                foreach (var pair in AxisManager.LocalAxes)
                {
                    var name = pair.Key;
                    var axis = pair.Value;

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button(name, axis.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled))
                    {
                        OnAxisSelect?.Invoke(axis);
                        Destroy(this);
                    }

                    if (GUILayout.Button(Strings.ButtonText_EditAxis,
                        new GUIStyle(Elements.Buttons.Default) {fontSize = 20, padding = new RectOffset(-3, 0, 0, 0)},
                        GUILayout.Width(30), GUILayout.MaxHeight(28)))
                    {
                        var editor = AxisEditorWindow.EditAxis(axis);
                        editor.Position = new Vector2(
                            Mathf.Clamp(_windowRect.x + _windowRect.width,
                                -320 + GUI.skin.window.padding.top, Screen.width - GUI.skin.window.padding.top),
                            Mathf.Clamp(_windowRect.y - 40, 0,
                            Screen.height - GUI.skin.window.padding.top));
                    }

                    if (GUILayout.Button(Strings.ButtonText_Close, Elements.Buttons.Red, GUILayout.Width(30)))
                        toBeRemoved = name;

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
                    new GUIStyle(Elements.Labels.Title) {alignment = TextAnchor.MiddleCenter});

                string toBeRemoved = null;

                foreach (var pair in AxisManager.MachineAxes)
                {
                    var name = pair.Key;
                    var axis = pair.Value;

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button(name, axis.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled))
                    {
                        OnAxisSelect?.Invoke(axis);
                        Destroy(this);
                    }

                    if (GUILayout.Button(Strings.ButtonText_EditAxis,
                        new GUIStyle(Elements.Buttons.Default) {fontSize = 20, padding = new RectOffset(-3, 0, 0, 0)},
                        GUILayout.Width(30), GUILayout.MaxHeight(28)))
                    {
                        var editor = AxisEditorWindow.EditAxis(axis);
                        editor.Position = new Vector2(
                            Mathf.Clamp(_windowRect.x + _windowRect.width,
                                -320 + GUI.skin.window.padding.top, Screen.width - GUI.skin.window.padding.top),
                            Mathf.Clamp(_windowRect.y - 40, 0,
                            Screen.height - GUI.skin.window.padding.top));
                    }

                    if (GUILayout.Button(Strings.ButtonText_Close, Elements.Buttons.Red, GUILayout.Width(30)))
                        toBeRemoved = name;

                    GUILayout.EndHorizontal();
                }

                if (toBeRemoved != null)
                    AxisManager.RemoveMachineAxis(toBeRemoved);

                GUILayout.EndScrollView();
            }

            if (GUILayout.Button(Strings.AxisEditorWindow_WindowTitle_CreateNewAxis, Elements.Buttons.Disabled))
            {
                var editor = AxisEditorWindow.CreateAxis(OnAxisSelect);
                editor.Position = new Vector2(
                    Mathf.Clamp(_windowRect.x + _windowRect.width,
                        -320 + GUI.skin.window.padding.top, Screen.width - GUI.skin.window.padding.top),
                    Mathf.Clamp(_windowRect.y - 40, 0,
                    Screen.height - GUI.skin.window.padding.top));
                Destroy(this);
            }
        }
    }
}