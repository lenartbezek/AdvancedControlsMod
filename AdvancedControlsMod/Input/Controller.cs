using Lench.AdvancedControls.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lench.AdvancedControls.Input
{
    /// <summary>
    /// Controller class providing access to an SDL device with axis and button mapping.
    /// </summary>
    public class Controller : IDisposable, IEquatable<Controller>
    {
        internal IntPtr DevicePointer;
        internal IntPtr GameController;

        private readonly float[] _axisValuesRaw;
        private readonly float[] _axisValuesSmooth;
        private readonly float[,] _ballValuesRaw;
        private readonly float[,] _ballValuesSmooth;

#pragma warning disable CS1591

        public int NumAxes => SDL.SDL_JoystickNumAxes(DevicePointer);
        public int NumBalls => SDL.SDL_JoystickNumBalls(DevicePointer);
        public int NumHats => SDL.SDL_JoystickNumHats(DevicePointer);
        public int NumButtons => SDL.SDL_JoystickNumButtons(DevicePointer);

#pragma warning restore CS1591

        /// <summary>
        /// List of all controller buttons, including hats (one for every direction).
        /// Does not necessarily contain objects equal by reference to buttons bound at input axes.
        /// </summary>
        public readonly List<Button> Buttons;

        private List<string> _axisNames;
        private List<string> _ballNames;
        private List<string> _hatNames;
        private List<string> _buttonNames;

        /// <summary>
        /// Returns name of the axis at given index.
        /// Takes into account controller mappings.
        /// Returns 'Unknown axis' if the controller has no such axis.
        /// </summary>
        /// <param name="index">Index of the axis.</param>
        /// <returns>String name.</returns>
        public string GetAxisName(int index)
        {
            if (index >= 0 && index < NumAxes)
            {
                return _axisNames[index];
            }
            else
            {
                return Strings.Controller_AxisName_UnknownAxis;
            }
        }

        /// <summary>
        /// Returns name of the ball at given index.
        /// Takes into account controller mappings.
        /// Returns 'Unknown ball' if the controller has no such ball.
        /// </summary>
        /// <param name="index">Index of the ball.</param>
        /// <returns>String name.</returns>
        public string GetBallName(int index)
        {
            if (index >= 0 && index < NumBalls)
            {
                return _ballNames[index];
            }
            else
            {
                return Strings.Controller_BallName_UnknownBall;
            }
        }

        /// <summary>
        /// Returns name of the hat at given index.
        /// Takes into account controller mappings.
        /// Returns 'Unknown hat' if the controller has no such hat.
        /// </summary>
        /// <param name="index">Index of the hat.</param>
        /// <returns>String name.</returns>
        public string GetHatName(int index)
        {
            if (index >= 0 && index < NumHats)
            {
                return _hatNames[index];
            }
            else
            {
                return Strings.Controller_HatName_UnknownHat;
            }
        }

        /// <summary>
        /// Returns name of the button at given index.
        /// Takes into account controller mappings.
        /// Returns 'Unknown button' if the controller has no such button.
        /// </summary>
        /// <param name="index">Index of the button.</param>
        /// <returns>String name.</returns>
        public string GetButtonName(int index)
        {
            if (index >= 0 && index < NumButtons)
            {
                return _buttonNames[index];
            }
            else
            {
                return Strings.Controller_ButtonName_UnknownButton;
            }
        }

        /// <summary>
        /// Index of the device needed to access it through SDL.
        /// </summary>
        public int Index => SDL.SDL_JoystickInstanceID(DevicePointer);

        /// <summary>
        /// Name of the device.
        /// Returns name from GameControllerMappings.txt if found.
        /// </summary>
        public string Name => IsGameController ? SDL.SDL_GameControllerName(GameController) : SDL.SDL_JoystickName(DevicePointer);

        /// <summary>
        /// GUID of the controller.
        /// </summary>
        public Guid GUID => SDL.SDL_JoystickGetGUID(DevicePointer);

        /// <summary>
        /// Is device currently connected.
        /// </summary>
        public bool Connected => SDL.SDL_JoystickGetAttached(DevicePointer) == SDL.SDL_bool.SDL_TRUE;

        /// <summary>
        /// Was device mapping found and is thus recognized as game controller.
        /// </summary>
        public bool IsGameController { get; }

        internal static List<Controller> ControllerList = new List<Controller>();

        /// <summary>
        /// Number of currently connected devices.
        /// </summary>
        public static int NumDevices => ControllerList.Count;

        /// <summary>
        /// Adds device connected at given index.
        /// Does nothing if no device is connected at that index.
        /// Checks for disconnected devices first.
        /// Devices shouldn't be added twice.
        /// </summary>
        /// <param name="index"></param>
        public static Controller Add(int index)
        {
            RemoveDisconnected();
            if (index < NumDevices) return null;
            var controller = new Controller(NumDevices);
            ControllerList.Add(controller);
            return controller;
        }

        /// <summary>
        /// Removes and disposes all disconnected devices.
        /// </summary>
        public static void RemoveDisconnected()
        {
            foreach (var c in ControllerList)
                if (!c.Connected) c.Dispose();
            ControllerList.RemoveAll(c => !c.Connected);
        }

        /// <summary>
        /// Returns a Controller object for a device with given guid.
        /// If such device is not found, returns null.
        /// </summary>
        /// <param name="guid">GUID of the device.</param>
        public static Controller Get(Guid guid)
        {
            try
            {
                return ControllerList.First(c => c.GUID == guid);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a Controller object for a device at given index.
        /// If such device is not found, returns null.
        /// </summary>
        /// <param name="id">Index of the device.</param>
        public static Controller Get(int id)
        {
            try
            {
                return ControllerList[id];
            }
            catch
            {
                return null;
            }
        }

        private Controller(int index)
        {
            if (index > SDL.SDL_NumJoysticks())
                throw new InvalidOperationException($"Cannot open controller {index} when only {SDL.SDL_NumJoysticks()} are connected.");

            IsGameController = SDL.SDL_IsGameController(index) == SDL.SDL_bool.SDL_TRUE;

            if (IsGameController)
            {
                GameController = SDL.SDL_GameControllerOpen(index);
                DevicePointer = SDL.SDL_GameControllerGetJoystick(GameController);
            }
            else
            {
                DevicePointer = SDL.SDL_JoystickOpen(index);
            }

            UpdateMappings();
            Buttons = new List<Button>();

            _axisValuesRaw = new float[SDL.SDL_JoystickNumAxes(DevicePointer)];
            _axisValuesSmooth = new float[SDL.SDL_JoystickNumAxes(DevicePointer)];

            _ballValuesRaw = new float[SDL.SDL_JoystickNumBalls(DevicePointer), 2];
            _ballValuesSmooth = new float[SDL.SDL_JoystickNumBalls(DevicePointer), 2];

            for (var i = 0; i < SDL.SDL_JoystickNumHats(DevicePointer); i++)
            {
                Buttons.Add(new HatButton(this, i, SDL.SDL_HAT_UP));
                Buttons.Add(new HatButton(this, i, SDL.SDL_HAT_DOWN));
                Buttons.Add(new HatButton(this, i, SDL.SDL_HAT_LEFT));
                Buttons.Add(new HatButton(this, i, SDL.SDL_HAT_RIGHT));
            }

            for (var i = 0; i < SDL.SDL_JoystickNumButtons(DevicePointer); i++)
            {
                Buttons.Add(new JoystickButton(this, i));
            }

            DeviceManager.OnDeviceRemapped += UpdateMappings;
            ACM.Instance.OnUpdate += Update;

            // Debug
            Debug.Log(IsGameController
                ? $"[ACM]: {Strings.Controller_GameControllerConnected} {Name}"
                : $"[ACM]: {Strings.Controller_JoystickConnected} {Name}");
            Debug.Log("\t"+Strings.Controller_GUID+" " + GUID);
        }

        private void Update()
        {
            var d = Mathf.Clamp(Time.deltaTime * 12, 0, 1);
            for (var i = 0; i < NumAxes; i++)
            {
                var axis = i;
                if (IsGameController)
                    axis = SDL.SDL_GameControllerGetBindForAxis(GameController, (SDL.SDL_GameControllerAxis)i).axis;
                _axisValuesRaw[axis] = SDL.SDL_JoystickGetAxis(DevicePointer, axis) / 32767.0f;
                _axisValuesSmooth[axis] = _axisValuesSmooth[axis] * (1 - d) + _axisValuesRaw[axis] * d;
            }
            for (var i = 0; i < NumBalls; i++)
            {
                int x, y;
                SDL.SDL_JoystickGetBall(DevicePointer, i, out x, out y);
                _ballValuesRaw[i, 0] = x / 32767.0f;
                _ballValuesRaw[i, 1] = y / 32767.0f;
                _ballValuesSmooth[i, 0] = _ballValuesSmooth[i, 0] * (1 - d) + _ballValuesRaw[i, 0] * d;
                _ballValuesSmooth[i, 1] = _ballValuesSmooth[i, 1] * (1 - d) + _ballValuesRaw[i, 1] * d;
            }
        }

        private void UpdateMappings(SDL.SDL_Event e)
        {
            if (!Connected) return;
            if (e.cdevice.which == Index ||
                e.jdevice.which == Index)
                UpdateMappings();
        }

        private static string GetAxisNameFromEnum(SDL.SDL_GameControllerAxis i)
        {
            switch (i)
            {
                case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX:
                    return Strings.Controller_AxisName_LeftX;
                case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY:
                    return Strings.Controller_AxisName_LeftY;
                case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX:
                    return Strings.Controller_AxisName_RightX;
                case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY:
                    return Strings.Controller_AxisName_RightY;
                case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT:
                    return Strings.Controller_AxisName_LeftTrigger;
                case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT:
                    return Strings.Controller_AxisName_RightTrigger;
                default:
                    return string.Format(Strings.Controller_AxisName_Default, (int)i + 1);
            }
        }

        private static string GetButtonNameFromEnum(SDL.SDL_GameControllerButton i)
        {
            switch (i)
            {
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A:
                    return Strings.Controller_ButtonName_AButton;
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B:
                    return Strings.Controller_ButtonName_BButton;
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X:
                    return Strings.Controller_ButtonName_XButton;
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y:
                    return Strings.Controller_ButtonName_YButton;
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK:
                    return Strings.Controller_ButtonName_BackButton;
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_GUIDE:
                    return Strings.Controller_ButtonName_GuideButton;
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START:
                    return Strings.Controller_ButtonName_StartButton;
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSTICK:
                    return Strings.Controller_ButtonName_LeftStick;
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSTICK:
                    return Strings.Controller_ButtonName_RightStick;
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER:
                    return Strings.Controller_ButtonName_LeftShoulder;
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER:
                    return Strings.Controller_ButtonName_RightShoulder;
                default:
                    return string.Format(Strings.Controller_ButtonName_Default, (int)i + 1);
            }
        }

        private void UpdateMappings()
        {
            if (!Connected) return;

            _axisNames = new List<string>();
            for (var i = 0; i < SDL.SDL_JoystickNumAxes(DevicePointer); i++)
            {
                string name;
                if (IsGameController)
                {
                    name = GetAxisNameFromEnum((SDL.SDL_GameControllerAxis)i);
                } 
                else
                {
                    switch (i)
                    {
                        case 0:
                            name = Strings.Controller_AxisName_XAxis;
                            break;
                        case 1:
                            name = Strings.Controller_AxisName_YAxis;
                            break;
                        default:
                            name = string.Format(Strings.Controller_AxisName_Default, i + 1);
                            break;
                    }
                }
                _axisNames.Add(name);
            }

            _ballNames = new List<string>();
            for (var i = 0; i < SDL.SDL_JoystickNumBalls(DevicePointer); i++)
            {
                _ballNames.Add(string.Format(Strings.Controller_BallName_Default, i + 1));
            }

            _hatNames = new List<string>();
            for (var i = 0; i < SDL.SDL_JoystickNumHats(DevicePointer); i++)
            {
                _hatNames.Add(IsGameController && i == 0
                    ? Strings.Controller_HatName_DPAD
                    : string.Format(Strings.Controller_HatName_Default, i + 1));
            }

            _buttonNames = new List<string>();
            for (var i = 0; i < SDL.SDL_JoystickNumButtons(DevicePointer); i++)
            {
                var name = IsGameController 
                    ? GetButtonNameFromEnum((SDL.SDL_GameControllerButton)i) 
                    : string.Format(Strings.Controller_ButtonName_Default, i + 1);
                _buttonNames.Add(name);
            }
        }

        /// <summary>
        /// Returns raw value of an axis.
        /// </summary>
        /// <param name="index">Index of the axis.</param>
        /// <param name="smooth">Axis smoothing.</param>
        /// <returns>value in range [-1, 1]</returns>
        public float GetAxis(int index, bool smooth = false)
        {
            return smooth ? _axisValuesSmooth[index] : _axisValuesRaw[index];
        }

        /// <summary>
        /// Closes the device and unsubscribes it from poll events.
        /// Is called on disconnected devices.
        /// </summary>
        public void Dispose()
        {
            if (IsGameController)
                SDL.SDL_GameControllerClose(GameController);
            else
                SDL.SDL_JoystickClose(DevicePointer);

            DeviceManager.OnDeviceRemapped -= UpdateMappings;
            ACM.Instance.OnUpdate -= Update;
        }

        /// <summary>
        /// Does this object represent the same device as other.
        /// </summary>
        public bool Equals(Controller other)
        {
            return other != null && DevicePointer == other.DevicePointer;
        }
    }
}
