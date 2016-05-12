using System.Collections.Generic;
using UnityEngine;
using spaar.ModLoader.UI;
using AdvancedControls.Axes;
using AdvancedControls.Controls;

namespace AdvancedControls.UI
{
    public class AxisList : MonoBehaviour
    {
        public bool Visible { get; set; } = false;

        public new string name { get { return "Axis List window"; } }

        private int windowID = spaar.ModLoader.Util.GetWindowID();
        private Rect windowRect = new Rect(100, 100, 320, 440);
        private Vector2 scrollPosition = Vector2.zero;

        private Dictionary<string, Axes.Axis> SavedAxes = new Dictionary<string, Axes.Axis>();

        private Control control;

        public void SelectAxis(Control control)
        {
            this.control = control;
            Visible = true;
        }

        private void ReturnAxis(Axes.Axis axis)
        {
            Visible = false;
            control.Axis = axis;
        }

        private void EditAxis(Axes.Axis axis)
        {
            if (axis as OneKeyAxis != null)
                AdvancedControlsMod.OneKeyAxisEdit.EditAxis(axis);
            if (axis as TwoKeyAxis != null)
                AdvancedControlsMod.TwoKeyAxisEdit.EditAxis(axis);
            if (axis as ControllerAxis != null)
                AdvancedControlsMod.ControllerAxisEdit.EditAxis(axis);
            if (axis as CustomAxis != null)
                AdvancedControlsMod.CustomAxisEdit.EditAxis(axis);
        }

        public void SaveAxis(Axes.Axis axis)
        {
            if (SavedAxes.ContainsKey(axis.Name))
                SavedAxes[axis.Name] = axis;
            else
                SavedAxes.Add(axis.Name, axis);
        }

        private void OnGUI()
        {
            if (Visible)
            {
                GUI.skin = Util.Skin;
                windowRect = GUILayout.Window(windowID, windowRect, DoWindow, "Select Axis");
            }
        }

        private void DoWindow(int id)
        {
            // Draw list
            scrollPosition = GUILayout.BeginScrollView(scrollPosition,
                GUILayout.Height(windowRect.height - GUI.skin.window.padding.top - GUI.skin.window.padding.bottom));

            string toBeRemoved = null;

            foreach(KeyValuePair<string, Axes.Axis> entry in SavedAxes)
            {
                var name = entry.Key;
                var axis = entry.Value;
                GUILayout.BeginHorizontal();

                if (GUILayout.Button(name, Elements.Buttons.Default, GUILayout.MaxWidth(180)))
                    ReturnAxis(axis);
                if (GUILayout.Button("Edit", Elements.Buttons.Default, GUILayout.Width(50)))
                    EditAxis(axis);
                if (GUILayout.Button("×", Elements.Buttons.Red, GUILayout.Width(30)))
                    toBeRemoved = name;

                GUILayout.EndHorizontal();
            }

            if (toBeRemoved != null)
            {
                SavedAxes.Remove(toBeRemoved);
                toBeRemoved = null;
            }

            // Draw add buttons
            GUILayout.Label("Add new axis", Util.LabelStyle);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Controller", Elements.Buttons.ComponentField))
                EditAxis(new ControllerAxis("Vertical"));
            if (GUILayout.Button("One Key", Elements.Buttons.ComponentField))
                EditAxis(new OneKeyAxis(KeyCode.None));
            if (GUILayout.Button("Two Key", Elements.Buttons.ComponentField))
                EditAxis(new TwoKeyAxis(KeyCode.None, KeyCode.None));
            if (GUILayout.Button("Custom", Elements.Buttons.ComponentField))
                EditAxis(new CustomAxis());

            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            if (GUI.Button(new Rect(windowRect.width - 28, 8, 20, 20),
                "×", Elements.Buttons.Red))
                Visible = false;

            GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
        }
    }
}
