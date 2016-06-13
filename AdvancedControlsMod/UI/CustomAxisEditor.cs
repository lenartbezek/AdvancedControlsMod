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

        internal string help =
@"Program your own input axis with Python 2.7.
Initialisation code is run once at the start.
Update code is run on every frame.
It's result is used as the axis value.

Value should be in range [-1, 1].
You can run axis code in global scope to 
interact with other custom axes and 
scripts in Lench Scripter Mod.";
        internal string note;
        internal string error;

        private Vector2 initialisationScrollPosition = Vector2.zero;
        private Vector2 updateScrollPosition = Vector2.zero;

        public void DrawAxis(Rect windowRect)
        {
            if (!PythonEnvironment.Loaded)
            {
                note = "<color=#FFFF00><b>Python engine not available.</b></color>\n"+
                        "Install full Lench Scripter Mod with Python binaries to enable custom axes.";
            }
            else
            {
                error = null;
                note = null;

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
                    note = "<color=#FFFF00><b>Script not enabled in settings menu</b></color>\n" +
                           "Axis code will be run in local scope.";
                }

                if (Axis.Error != null)
                {
                    error = "<color=#FF0000><b>Python error</b></color>\n" +
                            Axis.Error;
                }
            }
        }

        public string GetHelp()
        {
            return help;
        }

        public string GetNote()
        {
            return note;
        }

        public string GetError()
        {
            return error;
        }
    }
}