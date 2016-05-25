using UnityEngine;
using spaar.ModLoader.UI;
using AdvancedControls.Axes;
using AdvancedControls.Controls;

namespace AdvancedControls.UI
{
    public interface AxisEditor
    {
        void DrawAxis(Rect windowRect);
    }

    public class AxisEditorWindow : MonoBehaviour
    {
        public new string name { get { return "Edit Axis window"; } }

        public bool Visible { get; set; } = false;

        internal int _windowID = spaar.ModLoader.Util.GetWindowID();
        internal Rect windowRect = new Rect(100, 100, 100, 100);

        protected string WindowName = "Create new axis";
        protected string SaveName = "";
        protected InputAxis Axis;

        internal Control control;

        public void SaveAxis()
        {
            Axis.Name = SaveName;
            WindowName = "Edit " + SaveName;
            AxisManager.Put(Axis.Name, Axis.Clone());
            if (control != null)
            {
                control.Enabled = true;
                control.Axis = Axis.Name;
            }
        }

        public void CreateAxis()
        {
            Visible = true;
            WindowName = "Create new axis";
            Axis = null;
        }

        public void EditAxis(InputAxis axis)
        {
            Visible = true;
            WindowName = "Edit " + SaveName;
            Axis = axis;
            SaveName = Axis.Name;
        }

        /// <summary>
        /// Render window.
        /// </summary>
        protected virtual void OnGUI()
        {
            if (Visible)
            {
                GUI.skin = Util.Skin;
                windowRect = GUILayout.Window(_windowID, windowRect, DoWindow, WindowName,
                    GUILayout.Width(320),
                    GUILayout.Height(100));
            }
        }

        protected virtual void DoWindow(int id)
        {
            if(Axis == null)
            {
                // Draw add buttons
                GUILayout.Label("Create new axis", Elements.Labels.Title);
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Controller"))
                {
                    Axis = new ControllerAxis("new controller axis");
                    WindowName = "Create new controller axis";
                    SaveName = Axis.Name;
                }
                if (GUILayout.Button("Inertial"))
                {
                    Axis = new InertialAxis("new inertial axis");
                    WindowName = "Create new inertial key axis";
                    SaveName = Axis.Name;
                }
                if (GUILayout.Button("Standard"))
                {
                    Axis = new StandardAxis("new standard axis");
                    WindowName = "Create new standard axis";
                    SaveName = Axis.Name;
                }
                if (GUILayout.Button("Custom"))
                {
                    Axis = new CustomAxis("new custom axis");
                    WindowName = "Create new custom axis";
                    SaveName = Axis.Name;
                }

                GUILayout.EndHorizontal();
            }
            else
            {
                // Draw save text field and save button
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

                Axis.GetEditor().DrawAxis(windowRect);
            }

            // Draw close button
            if (GUI.Button(new Rect(windowRect.width - 28, 8, 20, 20),
                "×", Elements.Buttons.Red))
            {
                Visible = false;
                control = null;
            }

            // Drag window
            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }
    }
}
