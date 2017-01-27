using UnityEngine;
using spaar.ModLoader.UI;
using Lench.AdvancedControls.Axes;
using Lench.AdvancedControls.Input;
using Lench.AdvancedControls.Resources;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Lench.AdvancedControls.UI
{
    internal class ControllerAxisEditor : IAxisEditor
    {

        internal static bool DownloadingInProgress = false;
        internal static string DownloadButtonText = Strings.DownloadButtonText_Download;

        public ControllerAxisEditor(InputAxis axis)
        {
            _axis = axis as ControllerAxis;

            FindIndex();
            DeviceManager.OnDeviceAdded += (e) => FindIndex();
            DeviceManager.OnDeviceRemoved += (e) => FindIndex();
        }

        private readonly ControllerAxis _axis;
        private Controller _controller;

        private Rect _graphRect;
        private Rect _lastGraphRect;
        private Texture2D _graphTex;
        private Color[] _resetTex;

        private int _controllerIndex = -1;

        internal string Note;
        internal string Error;
         
        private Vector2 _clickPosition;
        private bool _dragging;

        private string _sensString;
        private string _curvString;
        private string _deadString;

        private void FindIndex()
        {
            if (Controller.ControllerList.Exists((c) => c.GUID == _axis.GUID))
                _controllerIndex = Controller.ControllerList.FindIndex((c) => c.GUID == _axis.GUID);
            else
                _controllerIndex = -1;
        }

        private void DrawGraph()
        {
            if (_graphTex == null || _graphRect != _lastGraphRect)
            {
                _graphTex = new Texture2D((int)_graphRect.width, (int)_graphRect.height);
                for (int i = 0; i < _graphTex.width; i++)
                    for (int j = 0; j < _graphTex.height; j++)
                        _graphTex.SetPixel(i, j, Color.clear);
                _resetTex = _graphTex.GetPixels();
            }
            if (_axis.Changed || _graphRect != _lastGraphRect)
            {
                _graphTex.SetPixels(_resetTex);
                float step = 0.5f / _graphTex.width;
                for (float xValue = -1; xValue < 1; xValue += step)
                {
                    float yValue = _axis.Process(xValue);
                    if (yValue <= -1f || yValue >= 1f) continue;
                    float xPixel = (xValue + 1) * _graphTex.width / 2;
                    float yPixel = (yValue + 1) * _graphTex.height / 2;
                    _graphTex.SetPixel(Mathf.RoundToInt(xPixel), Mathf.RoundToInt(yPixel), Color.white);
                }
                _graphTex.Apply();
            }
            _lastGraphRect = _graphRect;
            GUILayout.Box(_graphTex);
        }

        public void Open()
        {
            _sensString = _axis.Sensitivity.ToString("0.00");
            _curvString = _axis.Curvature.ToString("0.00");
            _deadString = _axis.Deadzone.ToString("0.00");
        }

        public void Close() { }

        public void DrawAxis(Rect windowRect)
        {
            if (!DeviceManager.SdlInitialized)
            {
#if windows
                GUILayout.Label($"<b>{Strings.ControllerAxisEditor_Message_AdditionalLibraryNeeded}</b>\n" +
                                Strings.ControllerAxisEditor_DrawAxis_RequireSDL2Windows+
                                $"<b>{Strings.ControllerAxisEditor_Message_Platform}</b>\n" +
                                Strings.ControllerAxisEditor_Message_PlatformWindows+
                                Strings.ControllerAxisEditor_Message_PlatformCorrect);
                if (GUILayout.Button(DownloadButtonText) && !DownloadingInProgress && !DeviceManager.SdlInstalled)
                    DeviceManager.InstallSdl();
#elif linux
                GUILayout.Label($"<b>{Strings.ControllerAxisEditor_Message_AdditionalLibraryNeeded}</b>\n" +
                                Strings.ControllerAxisEditor_Message_RequireSDL2Linux +
                                $"<b>{Strings.ControllerAxisEditor_Message_Platform}</b>\n" +
                                Strings.ControllerAxisEditor_Message_PlatformLinux +
                                Strings.ControllerAxisEditor_Message_PlatformCorrect);
                GUILayout.TextField("sudo apt-get install libsdl2-2.0-0");
#elif osx
                GUILayout.Label($"<b>{Strings.ControllerAxisEditor_Message_AdditionalLibraryNeeded}</b>\n" +
                                Strings.ControllerAxisEditor_Message_RequireSDL2OSX+
                                $"<b>{Strings.ControllerAxisEditor_Message_Platform}</b>\n" +
                                Strings.ControllerAxisEditor_Message_PlatformOSX +
                                Strings.ControllerAxisEditor_Message_PlatformCorrect);
                if (GUILayout.Button("www.libsdl.org/download-2.0.php"))
                    Application.OpenURL("www.libsdl.org/download-2.0.php");
#endif
            }
            else if (Controller.NumDevices == 0)
            {
                Note =  $"<color=#FFFF00><b>{Strings.ControllerAxisEditor_Message_NoControllersConnected}</b></color>\n"+
                        Strings.ControllerAxisEditor_Message_NoControllersConnectedDetail;
            }
            else if (_controllerIndex < 0)
            {
                Note = $"<color=#FFFF00><b>{Strings.ControllerAxisEditor_Message_AssociatedControllerNotConnected}</b></color>\n" +
                        Strings.ControllerAxisEditor_Message_AssociatedControllerNotConnectedDetail + _axis.GUID;
                if (GUILayout.Button(Strings.ControllerAxisEditor_ButtonText_UseAnotherController))
                {
                    _controllerIndex = 0;
                    _axis.GUID = Controller.ControllerList[_controllerIndex].GUID;
                }
            }
            else
            {
                Error = null;
                Note = null;

                _controller = Controller.Get(_axis.GUID);

                // Graph rect
                _graphRect = new Rect(
                    GUI.skin.window.padding.left,
                    GUI.skin.window.padding.top + 36,
                    windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                    windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right);

                // Axis value
                GUI.Label(new Rect(_graphRect.x, _graphRect.y, _graphRect.width, 20),
                        $"  <color=#808080><b>{_axis.OutputValue:0.00}</b></color>",
                        new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft });

                // Draw drag controls
                if (_axis.OffsetX == 0 && _axis.OffsetY == 0)
                {
                    GUI.Label(new Rect(_graphRect.x, _graphRect.y + _graphRect.height - 20, _graphRect.width, 20),
                        $"<color=#808080><b>{Strings.ControllerAxisEditor_Graph_DragToSetOffset}</b></color>",
                        new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleCenter });
                }
                else
                {
                    GUI.Label(new Rect(_graphRect.x, _graphRect.y + _graphRect.height - 20, (_graphRect.width - 16) / 2, 20),
                        $"  <color=#808080><b>X: {_axis.OffsetX:0.00}\tY:{_axis.OffsetY:0.00}</b></color>",
                        new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft });
                    if (GUI.Button(new Rect(_graphRect.x + (_graphRect.width - 16) / 2, _graphRect.y + _graphRect.height - 20, (_graphRect.width - 16) / 2, 20),
                            $"<color=#808080><b>{Strings.ControllerAxisEditor_Graph_ResetOffset}</b></color>",
                            new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleRight }))
                    {
                        _axis.OffsetX = 0;
                        _axis.OffsetY = 0;
                    }
                }

                // Draw graph
                DrawGraph();

                // Listen for drag
                var mousePos = UnityEngine.Input.mousePosition;
                mousePos.y = Screen.height - mousePos.y;
                var dragHandle = new Rect(windowRect.x + _graphRect.x, windowRect.y + _graphRect.y, _graphRect.width, _graphRect.height);
                var dragRange = (_graphRect.width) / 2f;

                if (!_dragging && UnityEngine.Input.GetMouseButtonDown(0) && dragHandle.Contains(mousePos))
                {
                    _dragging = true;
                    _clickPosition = mousePos;
                    _clickPosition.x += _axis.OffsetX * dragRange;
                    _clickPosition.y += _axis.OffsetY * dragRange;
                }

                if (_dragging)
                {
                    _axis.OffsetX = Mathf.Clamp((_clickPosition.x - mousePos.x) / dragRange, -1f, 1f);
                    _axis.OffsetY = Mathf.Clamp((_clickPosition.y - mousePos.y) / dragRange, -1f, 1f);
                    if (UnityEngine.Input.GetMouseButtonUp(0))
                    {
                        _dragging = false;
                    }
                }

                // Draw graph input and frame
                Util.DrawRect(_graphRect, Color.gray);
                Util.FillRect(new Rect(
                        _graphRect.x + _graphRect.width / 2,
                        _graphRect.y,
                        1,
                        _graphRect.height),
                    Color.gray);
                Util.FillRect(new Rect(
                        _graphRect.x,
                        _graphRect.y + _graphRect.height / 2,
                        _graphRect.width,
                        1),
                    Color.gray);

                if (_axis.Status == AxisStatus.OK)
                Util.FillRect(new Rect(
                                  _graphRect.x + _graphRect.width / 2 + _graphRect.width / 2 * _axis.InputValue,
                                  _graphRect.y,
                                  1,
                                  _graphRect.height),
                         Color.yellow);

                // Draw controller selection
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Strings.ButtonText_ArrowPrevious, _controllerIndex > 0 ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(30)) 
                    && _controllerIndex > 0)
                {
                    _controllerIndex--;
                    _axis.GUID = Controller.ControllerList[_controllerIndex].GUID;
                }

                GUILayout.Label(_controller.Name, new GUIStyle(Elements.InputFields.Default) { alignment = TextAnchor.MiddleCenter });

                if (GUILayout.Button(Strings.ButtonText_ArrowNext, _controllerIndex < Controller.NumDevices - 1 ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(30)) 
                    && _controllerIndex < Controller.NumDevices - 1)
                {
                    _controllerIndex++;
                    _axis.GUID = Controller.ControllerList[_controllerIndex].GUID;
                }

                if (_controller == null) return;

                GUILayout.EndHorizontal();

                // Draw axis selection
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Strings.ButtonText_ArrowPrevious, _axis.Axis > 0 ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(30)) 
                    && _axis.Axis > 0)
                    _axis.Axis--;

                GUILayout.Label(_controller.GetAxisName(_axis.Axis), new GUIStyle(Elements.InputFields.Default) { alignment = TextAnchor.MiddleCenter });

                if (GUILayout.Button(Strings.ButtonText_ArrowNext, _axis.Axis < Controller.Get(_axis.GUID).NumAxes - 1 ? Elements.Buttons.Default : Elements.Buttons.Disabled, GUILayout.Width(30)) 
                    && _axis.Axis < Controller.Get(_axis.GUID).NumAxes - 1)
                    _axis.Axis++;

                GUILayout.EndHorizontal();

                // Draw Sensitivity slider
                _axis.Sensitivity = Util.DrawSlider(Strings.Label_Sensitivity, _axis.Sensitivity, 0, 5, _sensString, out _sensString);

                // Draw Curvature slider
                _axis.Curvature = Util.DrawSlider(Strings.Label_Curvaure, _axis.Curvature, 0, 3, _curvString, out _curvString);

                // Draw Deadzone slider
                _axis.Deadzone = Util.DrawSlider(Strings.Label_Deadzone, _axis.Deadzone, 0, 0.5f, _deadString, out _deadString);

                GUILayout.BeginHorizontal();

                // Draw Invert toggle
                _axis.Invert = GUILayout.Toggle(_axis.Invert, string.Empty,
                    Util.ToggleStyle,
                    GUILayout.Width(20),
                    GUILayout.Height(20));

                GUILayout.Label(Strings.Label_Invert,
                    new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });

                // Draw Raw toggle
                _axis.Smooth = GUILayout.Toggle(_axis.Smooth, string.Empty,
                    Util.ToggleStyle,
                    GUILayout.Width(20),
                    GUILayout.Height(20));

                GUILayout.Label(Strings.Label_Smooth,
                    new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });

                GUILayout.EndHorizontal();
            }
        }

        public string GetHelpURL()
        {
            return Strings.ControllerAxisEditor_HelpURL;
        }

        public string GetNote()
        {
            return Note;
        }

        public string GetError()
        {
            return Error;
        }
    }
}
