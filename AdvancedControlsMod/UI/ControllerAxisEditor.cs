using UnityEngine;
using spaar.ModLoader.UI;
using AdvancedControls.Axes;
using AdvancedControls.Input;

namespace AdvancedControls.UI
{
    public class ControllerAxisEditor : AxisEditor
    {

#if windows
        private const string sdl_load_error_message = "place SDL2.dll in Mods/Resources/AdvancedControls/lib/";
        private const string platform = "Windows";
#elif linux
        private const string sdl_load_error_message = "run `sudo apt-get install libsdl2-2.0-0` command";
        private const string platform = "Linux";
#elif osx
        private const string sdl_load_error_message = "download and install runtime binaries from www.libsdl.org/download-2.0.php";
        private const string platform = "Mac OSX";
#endif

        public ControllerAxisEditor(InputAxis axis)
        {
            Axis = axis as ControllerAxis;
        }

        private ControllerAxis Axis;

        private Rect graphRect;
        private Rect last_graphRect;
        private Texture2D graphTex;

        private string help;
        private string note;
        private string error;

        private ControllerAxis.Param last_parameters;

        private void DrawGraph()
        {
            if (graphTex == null || graphRect != last_graphRect)
                graphTex = new Texture2D((int)graphRect.width, (int)graphRect.height);
            if (!Axis.Parameters.Equals(last_parameters) || graphRect != last_graphRect)
            {
                for (int i = 0; i < graphRect.width; i++)
                {
                    var value = Axis.Process((i - graphRect.width / 2) / (graphRect.width / 2));
                    var point = graphRect.height / 2 - (graphRect.height-1) / 2 * value;
                    for (int j = 0; j < graphRect.height; j++)
                    {
                        if ((int)point == j)
                            graphTex.SetPixel((int)graphRect.width - i - 1, j, Color.white);
                        else
                            graphTex.SetPixel((int)graphRect.width - i - 1, j, new Color(0, 0, 0, 0));
                    }
                }
                graphTex.Apply();
                last_graphRect = graphRect;
                last_parameters = Axis.Parameters;
            }
            GUILayout.Box(graphTex);
        }

        public void DrawAxis(Rect windowRect)
        {
            if (!AdvancedControlsMod.EventManager.SDL_Initialized)
            {
                error = "<color=#FF0000><b>SDL2 library not found.</b></color>\n" +
                        "Make sure SDL2 library is properly installed.\n" +
                        "To install it, " + sdl_load_error_message;
                note =  "<color=#FFFF00><b>" + platform + "</b></color>\n" +
                        "You are using " + platform + " version of Advanced Controls Mod.\n"+
                        "If you are using some other operating system, download the correct version of the mod.";
            }
            else if (Controller.NumDevices == 0)
            {
                note =  "<color=#FFFF00><b>No controllers connected.</b></color>\n"+
                        "You must connect a joystick or controller to use this axis.";
            }
            else
            {
                error = null;
                note = null;

                // Draw graph
                graphRect = new Rect(
                GUI.skin.window.padding.left,
                GUI.skin.window.padding.top + 36,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right);

                DrawGraph();

                graphRect = GUILayoutUtility.GetLastRect();

                Util.FillRect(new Rect(graphRect.x + graphRect.width / 2 + graphRect.width / 2 * Axis.InputValue,
                                  graphRect.y,
                                  1,
                                  graphRect.height),
                         Color.yellow);

                Util.DrawRect(graphRect, Color.gray);

                // Draw controller selection
                Axis.ControllerID = Axis.ControllerID % Controller.NumDevices;

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("<", Axis.ControllerID > 0 ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(30)) 
                    && Axis.ControllerID > 0)
                    Axis.ControllerID--;

                GUILayout.Label(Controller.Get(Axis.ControllerID).Name, new GUIStyle(Elements.InputFields.Default) { alignment = TextAnchor.MiddleCenter });

                if (GUILayout.Button(">", Axis.ControllerID < Controller.NumDevices - 1 ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(30)) 
                    && Axis.ControllerID < Controller.NumDevices - 1)
                    Axis.ControllerID++;

                GUILayout.EndHorizontal();

                // Draw axis selection
                Axis.AxisID = Axis.AxisID % Controller.Get(Axis.ControllerID).NumAxes;

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("<", Axis.AxisID > 0 ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(30)) 
                    && Axis.AxisID > 0)
                    Axis.AxisID--;

                GUILayout.Label(Controller.Get(Axis.ControllerID).AxisNames[Axis.AxisID], new GUIStyle(Elements.InputFields.Default) { alignment = TextAnchor.MiddleCenter });

                if (GUILayout.Button(">", Axis.AxisID < Controller.Get(Axis.ControllerID).NumAxes - 1 ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(30)) 
                    && Axis.AxisID < Controller.Get(Axis.ControllerID).NumAxes - 1)
                    Axis.AxisID++;

                GUILayout.EndHorizontal();

                // Draw Sensitivity slider
                GUILayout.BeginHorizontal();
                GUILayout.Label("Sensitivity", Util.LabelStyle);
                GUILayout.Label((Mathf.Round(Axis.Sensitivity * 100) / 100).ToString(),
                    Util.LabelStyle,
                    GUILayout.Width(60));
                GUILayout.EndHorizontal();

                Axis.Sensitivity = GUILayout.HorizontalSlider(Axis.Sensitivity, 0, 5);

                // Draw Curvature slider
                GUILayout.BeginHorizontal();
                GUILayout.Label("Curvature", Util.LabelStyle);
                GUILayout.Label((Mathf.Round(Axis.Curvature * 100) / 100).ToString(),
                    Util.LabelStyle,
                    GUILayout.Width(60));
                GUILayout.EndHorizontal();

                Axis.Curvature = GUILayout.HorizontalSlider(Axis.Curvature, 0, 3);

                // Draw Deadzone slider
                GUILayout.BeginHorizontal();
                GUILayout.Label("Deadzone", Util.LabelStyle);
                GUILayout.Label((Mathf.Round(Axis.Deadzone * 100) / 100).ToString(),
                    Util.LabelStyle,
                    GUILayout.Width(60));
                GUILayout.EndHorizontal();

                Axis.Deadzone = GUILayout.HorizontalSlider(Axis.Deadzone, 0, 0.5f);

                GUILayout.BeginHorizontal();

                // Draw Invert toggle
                Axis.Invert = GUILayout.Toggle(Axis.Invert, "",
                    Util.ToggleStyle,
                    GUILayout.Width(20),
                    GUILayout.Height(20));

                GUILayout.Label("Invert",
                    new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });

                // Draw Raw toggle
                Axis.Smooth = GUILayout.Toggle(Axis.Smooth, "",
                    Util.ToggleStyle,
                    GUILayout.Width(20),
                    GUILayout.Height(20));

                GUILayout.Label("Smooth",
                    new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });

                GUILayout.EndHorizontal();
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
