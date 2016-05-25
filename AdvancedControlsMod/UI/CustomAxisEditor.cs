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
            if (!PythonEnvironment.Loaded)
            {
                GUILayout.Label("<color=#FFFF00><b>Python engine not available.</b></color>\nInstall full LenchScripterMod with Python binaries to enable custom axes.");
            }
            else
            {
                // Draw initialisation code text area
                GUILayout.Label("Initialisation code",
                    Util.LabelStyle);

                initialisationScrollPosition = GUILayout.BeginScrollView(initialisationScrollPosition,
                    GUILayout.Height(100));
                Axis.InitialisationCode = GUILayout.TextArea(Axis.InitialisationCode);
                GUILayout.EndScrollView();

                // Draw update code text area
                GUILayout.Label("Update code",
                    Util.LabelStyle);

                updateScrollPosition = GUILayout.BeginScrollView(updateScrollPosition,
                    GUILayout.Height(200));
                Axis.UpdateCode = GUILayout.TextArea(Axis.UpdateCode);
                GUILayout.EndScrollView();

                // Draw Global toggle
                GUILayout.BeginHorizontal();

                Axis.GlobalScope = GUILayout.Toggle(Axis.GlobalScope, "",
                    Util.ToggleStyle,
                    GUILayout.Width(20),
                    GUILayout.Height(20));

                GUILayout.Label("Run in global scope",
                    new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });

                GUILayout.EndHorizontal();

                GUILayout.Box(GUIContent.none, GUILayout.Height(20));

                // Draw notes
                if (!PythonEnvironment.Enabled && Axis.GlobalScope)
                {
                    GUILayout.Label("<color=#FFFF00>Note</color>", new GUIStyle(Elements.Labels.Title) { richText = true });
                    GUILayout.Label("Machine script not enabled in settings menu.\nAxis code will be run in local scope.");
                }

                if (Axis.Error != null)
                {
                    GUILayout.Label("<color=#FF0000>Python error</color>", new GUIStyle(Elements.Labels.Title) { richText = true });
                    GUILayout.Label(Axis.Error);
                }
            }
        }
    }
}