using UnityEngine;
using LenchScripter;
using AdvancedControls.Axes;
using spaar.ModLoader.UI;

namespace AdvancedControls.UI
{
    public class CustomAxisEditor : AxisEditor
    {
        public CustomAxisEditor(InputAxis axis)
        {
            Axis = axis as CustomAxis;
        }

        private CustomAxis Axis;

        private Vector2 initialisationScrollPosition = Vector2.zero;
        private Vector2 updateScrollPosition = Vector2.zero;

        public void DrawAxis(Rect windowRect)
        {
            // Draw initialisation code text area
            GUILayout.Label("Initialisation code ",
                Util.LabelStyle);

            initialisationScrollPosition = GUILayout.BeginScrollView(initialisationScrollPosition,
                GUILayout.Height(100));
            Axis.InitialisationCode = GUILayout.TextArea(Axis.InitialisationCode);
            GUILayout.EndScrollView();

            // Draw update code text area
            GUILayout.Label("Update code ",
                Util.LabelStyle);

            updateScrollPosition = GUILayout.BeginScrollView(updateScrollPosition,
                GUILayout.Height(200));
            Axis.UpdateCode = GUILayout.TextArea(Axis.UpdateCode);
            GUILayout.EndScrollView();
            
            // Draw notes
            if (!Lua.Enabled)
            {
                GUILayout.Label("<color=#FFFF00>Note</color>", new GUIStyle(Elements.Labels.Title) { richText = true });
                GUILayout.Label("Lua needs to be enabled in the settings menu.");
            }

            if (Axis.Exception != null)
            {
                GUILayout.Label("<color=#FF0000>Exception</color>", new GUIStyle(Elements.Labels.Title) { richText = true });
                GUILayout.Label(Axis.Exception.Message);
            }
        }
    }
}