using UnityEngine;
using spaar.ModLoader.UI;
using Lench.AdvancedControls.Axes;

namespace Lench.AdvancedControls.UI
{
    internal interface AxisEditor
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

        internal int windowID = spaar.ModLoader.Util.GetWindowID();
        internal Rect windowRect = new Rect(0, 0, 320, 100);

        private string WindowName = "Create new axis";
        private string SaveName = "";
        private InputAxis Axis;

        internal SelectAxisDelegate Callback;

        internal void SaveAxis()
        {
            if (Axis.Name == SaveName || !AxisManager.LocalAxes.ContainsKey(Axis.Name))
                Axis.Dispose();
            Axis = Axis.Clone();
            Axis.editor.Open();
            Axis.Name = SaveName;
            AxisManager.AddLocalAxis(Axis);
            Callback?.Invoke(Axis);
            Destroy(this);
        }

        internal void CreateAxis(SelectAxisDelegate selectAxis = null)
        {
            Callback = selectAxis;
            WindowName = "Create new axis";
            Axis = null;
        }

        internal void EditAxis(InputAxis axis, SelectAxisDelegate selectAxis = null)
        {
            Callback = selectAxis;
            WindowName = "Edit " + axis.Name;
            SaveName = axis.Name;
            Axis = axis;
            Axis.editor.Open();
        }

        /// <summary>
        /// Render window.
        /// </summary>
        private void OnGUI()
        {
            GUI.skin = Util.Skin;
            windowRect = GUILayout.Window(windowID, windowRect, DoWindow, WindowName,
                GUILayout.Width(320),
                GUILayout.Height(100));
        }

        private void OnDestroy()
        {
            Axis?.editor.Close();
        }

        private void DoWindow(int id)
        {
            if(Axis == null)
            {
                // Draw add buttons
                if (GUILayout.Button("Controller Axis", Elements.Buttons.ComponentField))
                {
                    Axis = new ControllerAxis("new controller axis");
                    WindowName = "Create new controller axis";
                }
                if (GUILayout.Button("Key Axis", Elements.Buttons.ComponentField))
                {
                    Axis = new KeyAxis("new key axis");
                    WindowName = "Create new key axis";
                }
                if (GUILayout.Button("Mouse Axis", Elements.Buttons.ComponentField))
                {
                    Axis = new MouseAxis("new mouse axis");
                    WindowName = "Create new mouse axis";
                }
                if (GUILayout.Button("Chain Axis", Elements.Buttons.ComponentField))
                {
                    Axis = new ChainAxis("new chain axis");
                    WindowName = "Create new chain axis";
                }
                if (GUILayout.Button("Custom Axis", Elements.Buttons.ComponentField))
                {
                    Axis = new CustomAxis("new custom axis");
                    WindowName = "Create new custom axis";
                }
                if (Axis != null)
                {
                    SaveName = Axis.Name;
                    Axis.editor.Open();
                }
            }
            else
            {
                // Draw save text field and save button
                if (Axis.Saveable)
                {
                    GUILayout.BeginHorizontal();
                    SaveName = GUILayout.TextField(SaveName,
                        Elements.InputFields.Default);

                    if (GUILayout.Button("Save",
                        Elements.Buttons.Default,
                        GUILayout.Width(80))
                        && SaveName != "")
                    {
                        SaveAxis();
                    }
                    GUILayout.EndHorizontal();
                }

                // Draw axis editor
                Axis.GetEditor().DrawAxis(windowRect);

                // Draw error message
                if (Axis.GetEditor().GetError() != null)
                {
                    GUILayout.Label(Axis.GetEditor().GetError(), new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(8, 8, 12, 8) });
                }

                // Draw note message
                if (Axis.GetEditor().GetNote() != null)
                {
                    GUILayout.Label(Axis.GetEditor().GetNote(), new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(8, 8, 12, 8) });
                }

                // Draw help button
                if (Axis.GetEditor().GetHelpURL() != null)
                    if (GUI.Button(new Rect(windowRect.width - 76, 8, 30, 30),
                        "?", Elements.Buttons.Red))
                    {
                        Application.OpenURL(Axis.GetEditor().GetHelpURL());
                    }
            }


            // Draw close button
            if (GUI.Button(new Rect(windowRect.width - 38, 8, 30, 30),
                "×", Elements.Buttons.Red))
            {
                Callback = null;
                Destroy(this);
            }

            // Drag window
            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }
    }
}
