using UnityEngine;
using spaar.ModLoader.UI;
using Lench.AdvancedControls.Axes;
// ReSharper disable UnusedMember.Local

namespace Lench.AdvancedControls.UI
{
    internal interface IAxisEditor
    {
        void DrawAxis(Rect windowRect);
        void Open();
        void Close();
        string GetHelpURL();
        string GetNote();
        string GetError();
    }

    internal class AxisEditorWindow : MonoBehaviour
    {
        internal bool ShowHelp { get; set; } = false;

        internal int WindowID = spaar.ModLoader.Util.GetWindowID();
        internal Rect WindowRect = new Rect(0, 0, 320, 100);

        private string _windowName = "Create new axis";
        private string _saveName = "";
        private InputAxis _axis;

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

        internal void SaveAxis()
        {
            if (_axis.Name == _saveName || !AxisManager.LocalAxes.ContainsKey(_axis.Name))
                _axis.Dispose();
            _axis = _axis.Clone();
            _axis.Editor.Open();
            _axis.Name = _saveName;
            AxisManager.AddLocalAxis(_axis);
            Callback?.Invoke(_axis);
            Destroy(this);
        }

        internal void CreateAxis(SelectAxisDelegate selectAxis = null)
        {
            Callback = selectAxis;
            _windowName = "Create new axis";
            _axis = null;
        }

        internal void EditAxis(InputAxis axis, SelectAxisDelegate selectAxis = null)
        {
            Callback = selectAxis;
            _windowName = "Edit " + axis.Name;
            _saveName = axis.Name;
            _axis = axis;
            _axis.Editor.Open();
        }

        /// <summary>
        /// Render window.
        /// </summary>
        private void OnGUI()
        {
            GUI.skin = Util.Skin;
            WindowRect = GUILayout.Window(WindowID, WindowRect, DoWindow, _windowName,
                GUILayout.Width(320),
                GUILayout.Height(100));
        }

        private void OnDestroy()
        {
            _axis?.Editor.Close();
        }

        private void DoWindow(int id)
        {
            if(_axis == null)
            {
                // Draw add buttons
                if (GUILayout.Button("Controller Axis", Elements.Buttons.ComponentField))
                {
                    _axis = new ControllerAxis("new controller axis");
                    _windowName = "Create new controller axis";
                }
                if (GUILayout.Button("Key Axis", Elements.Buttons.ComponentField))
                {
                    _axis = new KeyAxis("new key axis");
                    _windowName = "Create new key axis";
                }
                if (GUILayout.Button("Mouse Axis", Elements.Buttons.ComponentField))
                {
                    _axis = new MouseAxis("new mouse axis");
                    _windowName = "Create new mouse axis";
                }
                if (GUILayout.Button("Chain Axis", Elements.Buttons.ComponentField))
                {
                    _axis = new ChainAxis("new chain axis");
                    _windowName = "Create new chain axis";
                }
                if (GUILayout.Button("Custom Axis", Elements.Buttons.ComponentField))
                {
                    _axis = new CustomAxis("new custom axis");
                    _windowName = "Create new custom axis";
                }
                if (_axis != null)
                {
                    _saveName = _axis.Name;
                    _axis.Editor.Open();
                }
            }
            else
            {
                // Draw save text field and save button
                if (_axis.Saveable)
                {
                    GUILayout.BeginHorizontal();
                    _saveName = GUILayout.TextField(_saveName,
                        Elements.InputFields.Default);

                    if (GUILayout.Button("Save",
                        Elements.Buttons.Default,
                        GUILayout.Width(80))
                        && _saveName != "")
                    {
                        SaveAxis();
                    }
                    GUILayout.EndHorizontal();
                }

                // Draw axis editor
                _axis.GetEditor().DrawAxis(WindowRect);

                // Draw error message
                if (_axis.GetEditor().GetError() != null)
                {
                    GUILayout.Label(_axis.GetEditor().GetError(), new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(8, 8, 12, 8) });
                }

                // Draw note message
                if (_axis.GetEditor().GetNote() != null)
                {
                    GUILayout.Label(_axis.GetEditor().GetNote(), new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(8, 8, 12, 8) });
                }

                // Draw help button
                if (_axis.GetEditor().GetHelpURL() != null)
                    if (GUI.Button(new Rect(WindowRect.width - 76, 8, 30, 30),
                        "?", Elements.Buttons.Red))
                    {
                        Application.OpenURL(_axis.GetEditor().GetHelpURL());
                    }
            }


            // Draw close button
            if (GUI.Button(new Rect(WindowRect.width - 38, 8, 30, 30),
                "×", Elements.Buttons.Red))
            {
                Callback = null;
                Destroy(this);
            }

            // Drag window
            GUI.DragWindow(new Rect(0, 0, WindowRect.width, GUI.skin.window.padding.top));
        }
    }
}
