using UnityEngine;
using spaar.ModLoader.UI;
using Lench.AdvancedControls.Axes;
using Lench.AdvancedControls.Input;

namespace Lench.AdvancedControls.UI
{
    internal class ControllerAxisEditor : AxisEditor
    {

        internal static bool downloading_in_progress = false;
        internal static string download_button_text = "Download";

        public ControllerAxisEditor(InputAxis axis)
        {
            Axis = axis as ControllerAxis;

            FindIndex();
            DeviceManager.OnDeviceAdded += (SDL.SDL_Event e) => FindIndex();
            DeviceManager.OnDeviceRemoved += (SDL.SDL_Event e) => FindIndex();
        }

        private ControllerAxis Axis;
        private Controller controller;

        private Rect graphRect;
        private Rect last_graphRect;
        private Texture2D graphTex;
        private Color[] resetTex;

        private int controller_index = -1;

        internal string note;
        internal string error;
         
        private Vector2 click_position;
        private bool dragging;

        private string sens_string;
        private string curv_string;
        private string dead_string;

        private void FindIndex()
        {
            if (Controller.DeviceList.Contains(Axis.GUID))
                controller_index = Controller.DeviceList.FindIndex(guid => guid == Axis.GUID);
            else
                controller_index = -1;
        }

        private void DrawGraph()
        {
            if (graphTex == null || graphRect != last_graphRect)
            {
                graphTex = new Texture2D((int)graphRect.width, (int)graphRect.height);
                for (int i = 0; i < graphTex.width; i++)
                    for (int j = 0; j < graphTex.height; j++)
                        graphTex.SetPixel(i, j, Color.clear);
                resetTex = graphTex.GetPixels();
            }
            if (Axis.Changed || graphRect != last_graphRect)
            {
                graphTex.SetPixels(resetTex);
                float step = 0.5f / graphTex.width;
                for (float x_value = -1; x_value < 1; x_value += step)
                {
                    float y_value = Axis.Process(x_value);
                    if (y_value <= -1f || y_value >= 1f) continue;
                    float x_pixel = (x_value + 1) * graphTex.width / 2;
                    float y_pixel = (y_value + 1) * graphTex.height / 2;
                    graphTex.SetPixel(Mathf.RoundToInt(x_pixel), Mathf.RoundToInt(y_pixel), Color.white);
                }
                graphTex.Apply();
            }
            last_graphRect = graphRect;
            GUILayout.Box(graphTex);
        }

        public void Open()
        {
            sens_string = Axis.Sensitivity.ToString("0.00");
            curv_string = Axis.Curvature.ToString("0.00");
            dead_string = Axis.Deadzone.ToString("0.00");
        }

        public void Close() { }

        public void DrawAxis(Rect windowRect)
        {
            if (!DeviceManager.SDL_Initialized)
            {
#if windows
                GUILayout.Label("<b>Additional library needed</b>\n" +
                                "Controller axis requires SDL2 library to work.\n" +
                                "Press download to install it automatically.\n\n"+
                                "<b>Platform</b>\n" +
                                "You are using Windows version of ACM.\n"+
                                "If you are using some other operating system,\n"+
                                "download the correct version of the mod.");
                if (GUILayout.Button(download_button_text) && !downloading_in_progress && !DeviceManager.SDL_Installed)
                    DeviceManager.InstallSDL();
#elif linux
                GUILayout.Label("<b>Additional library needed</b>\n" +
                                "Controller axis requires SDL2 library to work.\n" +
                                "Run the command below to install it.\n\n"+
                                "<b>Platform</b>\n" +
                                "You are using Linux version of ACM.\n" +
                                "If you are using some other operating system,\n" +
                                "download the correct version of the mod.");
                GUILayout.TextField("sudo apt-get install libsdl2-2.0-0");
#elif osx
                GUILayout.Label("<b>Additional library needed</b>\n" +
                                "Controller axis requires SDL2 library to work.\n" +
                                "Download it at the link below.\n\n"+
                                "<b>Platform</b>\n" +
                                "You are using OSX version of ACM.\n" +
                                "If you are using some other operating system,\n" +
                                "download the correct version of the mod.");
                if (GUILayout.Button("www.libsdl.org/download-2.0.php."))
                    Application.OpenURL("www.libsdl.org/download-2.0.php.");
#endif
            }
            else if (Controller.NumDevices == 0)
            {
                note =  "<color=#FFFF00><b>No controllers connected.</b></color>\n"+
                        "Connect a joystick or controller to use this axis.";
            }
            else if (controller_index < 0)
            {
                note = "<color=#FFFF00><b>Associated controller not connected.</b></color>\n" +
                        "The device this axis is bound to is not found.\n"+
                        "\n<b>Device GUID</b>\n" + Axis.GUID;
                if (GUILayout.Button("Use another controller"))
                {
                    controller_index = 0;
                    Axis.GUID = Controller.DeviceList[controller_index];
                }
            }
            else
            {
                error = null;
                note = null;

                controller = Controller.Get(Axis.GUID);
                Axis.Axis %= controller.NumAxes;

                // Graph rect
                graphRect = new Rect(
                    GUI.skin.window.padding.left,
                    GUI.skin.window.padding.top + 36,
                    windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                    windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right);

                // Axis value
                GUI.Label(new Rect(graphRect.x, graphRect.y, graphRect.width, 20),
                        "  <color=#808080><b>"+ Axis.OutputValue.ToString("0.00")+"</b></color>",
                        new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft });

                // Draw drag controls
                if (Axis.OffsetX == 0 && Axis.OffsetY == 0)
                {
                    GUI.Label(new Rect(graphRect.x, graphRect.y + graphRect.height - 20, graphRect.width, 20),
                        "<color=#808080><b>DRAG TO SET OFFSET</b></color>",
                        new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleCenter });
                }
                else
                {
                    GUI.Label(new Rect(graphRect.x, graphRect.y + graphRect.height - 20, (graphRect.width - 16) / 2, 20),
                        "  <color=#808080><b>X: " + Axis.OffsetX.ToString("0.00") + "\tY:" + Axis.OffsetY.ToString("0.00") + "</b></color>",
                        new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft });
                    if (GUI.Button(new Rect(graphRect.x + (graphRect.width - 16) / 2, graphRect.y + graphRect.height - 20, (graphRect.width - 16) / 2, 20),
                            "<color=#808080><b>RESET OFFSET</b></color>",
                            new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleRight }))
                    {
                        Axis.OffsetX = 0;
                        Axis.OffsetY = 0;
                    }
                }

                // Draw graph
                DrawGraph();

                // Listen for drag
                var mousePos = UnityEngine.Input.mousePosition;
                mousePos.y = Screen.height - mousePos.y;
                var drag_handle = new Rect(windowRect.x + graphRect.x, windowRect.y + graphRect.y, graphRect.width, graphRect.height);
                var drag_range = (graphRect.width) / 2f;

                if (!dragging && UnityEngine.Input.GetMouseButtonDown(0) && drag_handle.Contains(mousePos))
                {
                    dragging = true;
                    click_position = mousePos;
                    click_position.x += Axis.OffsetX * drag_range;
                    click_position.y += Axis.OffsetY * drag_range;
                }

                if (dragging)
                {
                    Axis.OffsetX = Mathf.Clamp((click_position.x - mousePos.x) / drag_range, -1f, 1f);
                    Axis.OffsetY = Mathf.Clamp((click_position.y - mousePos.y) / drag_range, -1f, 1f);
                    if (UnityEngine.Input.GetMouseButtonUp(0))
                    {
                        dragging = false;
                    }
                }

                // Draw graph input and frame
                Util.DrawRect(graphRect, Color.gray);
                Util.FillRect(new Rect(
                        graphRect.x + graphRect.width / 2,
                        graphRect.y,
                        1,
                        graphRect.height),
                    Color.gray);
                Util.FillRect(new Rect(
                        graphRect.x,
                        graphRect.y + graphRect.height / 2,
                        graphRect.width,
                        1),
                    Color.gray);

                Util.FillRect(new Rect(
                                  graphRect.x + graphRect.width / 2 + graphRect.width / 2 * Axis.InputValue,
                                  graphRect.y,
                                  1,
                                  graphRect.height),
                         Color.yellow);

                // Draw controller selection
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("<", controller_index > 0 ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(30)) 
                    && controller_index > 0)
                {
                    controller_index--;
                    Axis.GUID = Controller.DeviceList[controller_index];
                }

                GUILayout.Label(controller != null ? controller.Name : "<color=#FF0000>Disconnected controller</color>",
                    new GUIStyle(Elements.InputFields.Default) { alignment = TextAnchor.MiddleCenter });

                if (GUILayout.Button(">", controller_index < Controller.NumDevices - 1 ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(30)) 
                    && controller_index < Controller.NumDevices - 1)
                {
                    controller_index++;
                    Axis.GUID = Controller.DeviceList[controller_index];
                }

                if (controller == null) return;

                GUILayout.EndHorizontal();

                // Draw axis selection
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("<", Axis.Axis > 0 ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(30)) 
                    && Axis.Axis > 0)
                    Axis.Axis--;

                GUILayout.Label(controller.AxisNames[Axis.Axis], new GUIStyle(Elements.InputFields.Default) { alignment = TextAnchor.MiddleCenter });

                if (GUILayout.Button(">", Axis.Axis < Controller.Get(Axis.GUID).NumAxes - 1 ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(30)) 
                    && Axis.Axis < Controller.Get(Axis.GUID).NumAxes - 1)
                    Axis.Axis++;

                GUILayout.EndHorizontal();

                // Draw Sensitivity slider
                Axis.Sensitivity = Util.DrawSlider("Sensitivity", Axis.Sensitivity, 0, 5, sens_string, out sens_string);

                // Draw Curvature slider
                Axis.Curvature = Util.DrawSlider("Curvaure", Axis.Curvature, 0, 3, curv_string, out curv_string);

                // Draw Deadzone slider
                Axis.Deadzone = Util.DrawSlider("Deadzone", Axis.Deadzone, 0, 0.5f, dead_string, out dead_string);

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

        public string GetHelpURL()
        {
            return "https://github.com/lench4991/AdvancedControlsMod/wiki/Controller-Axis";
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
