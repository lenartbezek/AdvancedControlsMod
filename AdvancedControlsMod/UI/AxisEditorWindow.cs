using UnityEngine;
using spaar.ModLoader.UI;
using AdvancedControls.Axes;

namespace AdvancedControls.UI
{
    internal interface AxisEditor
    {
        void DrawAxis(Rect windowRect);
        string GetHelp();
        string GetNote();
        string GetError();
    }

    internal class AxisEditorWindow : MonoBehaviour
    {
        internal bool ShowHelp { get; set; } = false;

        internal int windowID = spaar.ModLoader.Util.GetWindowID();
        internal Rect windowRect = new Rect(100, 100, 100, 100);

        protected string WindowName = "Create new axis";
        protected string SaveName = "";
        protected InputAxis Axis;

        internal delegate void SelectAxis(InputAxis axis);
        private SelectAxis Select;

        internal void SaveAxis()
        {
            Axis = Axis.Clone();
            Axis.Name = SaveName;
            WindowName = "Edit " + SaveName;
            AxisManager.Put(Axis.Name, Axis);
            Select?.Invoke(Axis);
            Select = null;
        }

        internal void CreateAxis(SelectAxis selectAxis = null)
        {
            Select = selectAxis;
            WindowName = "Create new axis";
            Axis = null;
        }

        internal void EditAxis(InputAxis axis)
        {
            Select = null;
            WindowName = "Edit " + axis.Name;
            SaveName = axis.Name;
            Axis = axis;
        }

        /// <summary>
        /// Render window.
        /// </summary>
        protected virtual void OnGUI()
        {
            GUI.skin = Util.Skin;
            windowRect = GUILayout.Window(windowID, windowRect, DoWindow, WindowName,
                GUILayout.Width(320),
                GUILayout.Height(100));
        }

        protected virtual void DoWindow(int id)
        {
            if(Axis == null)
            {
                // Draw add buttons
                if (GUILayout.Button("Controller Axis", Elements.Buttons.ComponentField))
                {
                    Axis = new ControllerAxis("new controller axis");
                    WindowName = "Create new controller axis";
                }
                if (GUILayout.Button("Inertial Axis", Elements.Buttons.ComponentField))
                {
                    Axis = new InertialAxis("new inertial axis");
                    WindowName = "Create new inertial key axis";
                }
                if (GUILayout.Button("Standard Axis", Elements.Buttons.ComponentField))
                {
                    Axis = new StandardAxis("new standard axis");
                    WindowName = "Create new standard axis";
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

                // Draw help message
                if (ShowHelp && Axis.GetEditor().GetHelp() != null)
                {
                    if (GUILayout.Button(Axis.GetEditor().GetHelp(), new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(8, 8, 12, 8) }))
                        ShowHelp = false;
                }

                // Draw help button
                if (Axis.GetEditor().GetHelp() != null)
                    if (GUI.Button(new Rect(windowRect.width - 76, 8, 30, 30),
                        "?", Elements.Buttons.Red))
                    {
                        ShowHelp = !ShowHelp;
                    }
            }

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
    }
}
