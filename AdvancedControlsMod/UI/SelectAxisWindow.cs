﻿using AdvancedControls.Axes;
using spaar.ModLoader.UI;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedControls.UI
{
    internal delegate void SelectAxisDelegate(InputAxis axis);

    internal class SelectAxisWindow : MonoBehaviour
    {
        internal int windowID = spaar.ModLoader.Util.GetWindowID();
        internal Rect windowRect = new Rect(100, 100, 320, 100);

        internal bool ContainsMouse
        {
            get
            {
                var mousePos = UnityEngine.Input.mousePosition;
                mousePos.y = Screen.height - mousePos.y;
                return windowRect.Contains(mousePos);
            }
        }
        internal SelectAxisDelegate Callback;

        private bool compact;

        private Vector2 scrollPosition = Vector2.zero;

        internal static SelectAxisWindow Open(SelectAxisDelegate callback, bool compact = false)
        {
            var window = ACM.Instance.gameObject.AddComponent<SelectAxisWindow>();
            window.Callback = callback;
            window.compact = compact;
            return window;
        }

        private void OnGUI()
        {
            GUI.skin = Util.Skin;
            GUIStyle windowStyle = compact ? Util.CompactWindowStyle : Util.FullWindowStyle;
            windowRect = GUILayout.Window(windowID, windowRect, DoWindow, compact ? "" : "Select axis", windowStyle,
                        GUILayout.Width(320),
                        GUILayout.Height(42));
        }

        private void DoWindow(int id)
        {
            if (AxisManager.Axes.Count > 0)
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition,
                    GUILayout.Height(Mathf.Clamp(AxisManager.Axes.Count * 36 + 8, 180, 480)));

                string toBeRemoved = null;

                foreach (KeyValuePair<string, InputAxis> pair in AxisManager.Axes)
                {
                    var name = pair.Key;
                    var axis = pair.Value;

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button(name, axis.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled))
                    {
                        Callback?.Invoke(axis);
                        Destroy(this);
                    }

                    if (GUILayout.Button("✎", new GUIStyle(Elements.Buttons.Default) { fontSize = 20, padding = new RectOffset(-3, 0, 0, 0) }, GUILayout.Width(30), GUILayout.MaxHeight(28)))
                    {
                        var Editor = ACM.Instance.gameObject.AddComponent<AxisEditorWindow>();
                        Editor.windowRect.x = windowRect.x + windowRect.width;
                        Editor.windowRect.y = windowRect.y;
                        Editor.EditAxis(axis);
                    }

                    if (GUILayout.Button("×", Elements.Buttons.Red, GUILayout.Width(30)))
                    {
                        toBeRemoved = name;
                    }

                    GUILayout.EndHorizontal();
                }

                if (toBeRemoved != null)
                    AxisManager.Remove(toBeRemoved);

                GUILayout.EndScrollView();
            }

            if (GUILayout.Button("Create new axis", Elements.Buttons.Disabled))
            {
                var Editor = ACM.Instance.gameObject.AddComponent<AxisEditorWindow>();
                Editor.windowRect.x = windowRect.x;
                Editor.windowRect.y = windowRect.y;
                Editor.CreateAxis(new SelectAxisDelegate(Callback));
                Destroy(this);
            }

            // Draw close button
            if (!compact)
                if (GUI.Button(new Rect(windowRect.width - 38, 8, 30, 30),
                    "×", Elements.Buttons.Red))
                    Destroy(this);
        }
    }
}
