using Lench.AdvancedControls.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Lench.AdvancedControls.Input.SDL;

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

        public int NumAxes => SDL_JoystickNumAxes(DevicePointer);
        public int NumBalls => SDL_JoystickNumBalls(DevicePointer);
        public int NumHats => SDL_JoystickNumHats(DevicePointer);
        public int NumButtons => SDL_JoystickNumButtons(DevicePointer);

#pragma warning restore CS1591

        /// <summary>
        /// List of all controller buttons, including hats (one for every direction).
        /// Does not necessarily contain objects equal by reference to buttons bound at input axes.
        /// </summary>
        public readonly List<Button> Buttons;

        /// <summary>
        /// Returns the mapping string of the game controller. 
        /// String format: https://wiki.libsdl.org/SDL_GameControllerAddMapping
        /// </summary>
        public string Mapping
        {
            get { return _mapping; }
            private set
            {
                FormMappingDicts(value);
                _mapping = value;
            }
        }

        private List<string> _axisNames;
        private List<string> _ballNames;
        private List<string> _hatNames;
        private List<string> _buttonNames;

        private string _mapping;
        private readonly Dictionary<SDL_GameControllerAxis, int> _axisMappingDict = new Dictionary<SDL_GameControllerAxis, int>();
        private readonly Dictionary<SDL_GameControllerButton, int> _buttonMappingDict = new Dictionary<SDL_GameControllerButton, int>();

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
        public int Index => SDL_JoystickInstanceID(DevicePointer);

        /// <summary>
        /// Name of the device.
        /// Returns name from GameControllerMappings.txt if found.
        /// </summary>
        public string Name => IsGameController ? SDL_GameControllerName(GameController) : SDL_JoystickName(DevicePointer);

        /// <summary>
        /// GUID of the controller.
        /// </summary>
        public Guid GUID => SDL_JoystickGetGUID(DevicePointer);

        /// <summary>
        /// Is device currently connected.
        /// </summary>
        public bool Connected => SDL_JoystickGetAttached(DevicePointer) == SDL_bool.SDL_TRUE;

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
#if DEBUG
            Debug.Log("Attempting to remove disconnected devices.");
#endif
            foreach (var c in ControllerList)
                if (!c.Connected) c.Dispose();

            // ReSharper disable once UnusedVariable
            var count = ControllerList.RemoveAll(c => !c.Connected);
#if DEBUG
            Debug.Log($"Removed {count} devices.");
#endif
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
#if DEBUG
            Debug.Log($"Attempting to open controller at index {index}.");
#endif

            if (index > SDL_NumJoysticks())
                throw new InvalidOperationException($"Cannot open controller {index} when only {SDL_NumJoysticks()} are connected.");

            IsGameController = SDL_IsGameController(index) == SDL_bool.SDL_TRUE;

            if (IsGameController)
            {
                GameController = SDL_GameControllerOpen(index);
                DevicePointer = SDL_GameControllerGetJoystick(GameController);
            }
            else
            {
                DevicePointer = SDL_JoystickOpen(index);
            }

            UpdateNames();
            Buttons = new List<Button>();

            _axisValuesRaw = new float[SDL_JoystickNumAxes(DevicePointer)];
            _axisValuesSmooth = new float[SDL_JoystickNumAxes(DevicePointer)];

            _ballValuesRaw = new float[SDL_JoystickNumBalls(DevicePointer), 2];
            _ballValuesSmooth = new float[SDL_JoystickNumBalls(DevicePointer), 2];

            for (var i = 0; i < SDL_JoystickNumHats(DevicePointer); i++)
            {
                Buttons.Add(new HatButton(this, i, SDL_HAT_UP));
                Buttons.Add(new HatButton(this, i, SDL_HAT_DOWN));
                Buttons.Add(new HatButton(this, i, SDL_HAT_LEFT));
                Buttons.Add(new HatButton(this, i, SDL_HAT_RIGHT));
            }

            for (var i = 0; i < SDL_JoystickNumButtons(DevicePointer); i++)
            {
                Buttons.Add(new JoystickButton(this, i));
            }

            DeviceManager.OnDeviceRemapped += UpdateMapping;
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
                _axisValuesRaw[i] = IsGameController 
                    ? SDL_GameControllerGetAxis(GameController, (SDL_GameControllerAxis)i) / 32767.0f
                    : SDL_JoystickGetAxis(DevicePointer, i) / 32767.0f;
                _axisValuesSmooth[i] = _axisValuesSmooth[i] * (1 - d) + _axisValuesRaw[i] * d;
            }
            for (var i = 0; i < NumBalls; i++)
            {
                int x, y;
                SDL_JoystickGetBall(DevicePointer, i, out x, out y);
                _ballValuesRaw[i, 0] = x / 32767.0f;
                _ballValuesRaw[i, 1] = y / 32767.0f;
                _ballValuesSmooth[i, 0] = _ballValuesSmooth[i, 0] * (1 - d) + _ballValuesRaw[i, 0] * d;
                _ballValuesSmooth[i, 1] = _ballValuesSmooth[i, 1] * (1 - d) + _ballValuesRaw[i, 1] * d;
            }
        }

        private static string GetAxisNameFromEnum(SDL_GameControllerAxis i)
        {
            switch (i)
            {
                case SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX:
                    return Strings.Controller_AxisName_LeftX;
                case SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY:
                    return Strings.Controller_AxisName_LeftY;
                case SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX:
                    return Strings.Controller_AxisName_RightX;
                case SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY:
                    return Strings.Controller_AxisName_RightY;
                case SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT:
                    return Strings.Controller_AxisName_LeftTrigger;
                case SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT:
                    return Strings.Controller_AxisName_RightTrigger;
                default:
                    return string.Format(Strings.Controller_AxisName_Default, (int)i + 1);
            }
        }

        private static string GetButtonNameFromEnum(SDL_GameControllerButton i)
        {
            switch (i)
            {
                case SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A:
                    return Strings.Controller_ButtonName_AButton;
                case SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B:
                    return Strings.Controller_ButtonName_BButton;
                case SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X:
                    return Strings.Controller_ButtonName_XButton;
                case SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y:
                    return Strings.Controller_ButtonName_YButton;
                case SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK:
                    return Strings.Controller_ButtonName_BackButton;
                case SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_GUIDE:
                    return Strings.Controller_ButtonName_GuideButton;
                case SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START:
                    return Strings.Controller_ButtonName_StartButton;
                case SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSTICK:
                    return Strings.Controller_ButtonName_LeftStick;
                case SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSTICK:
                    return Strings.Controller_ButtonName_RightStick;
                case SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER:
                    return Strings.Controller_ButtonName_LeftShoulder;
                case SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER:
                    return Strings.Controller_ButtonName_RightShoulder;
                default:
                    return string.Format(Strings.Controller_ButtonName_Default, (int)i + 1);
            }
        }

        private void UpdateMapping(SDL_Event e)
        {
            if (!Connected) return;
            if (e.cdevice.which == Index ||
                e.jdevice.which == Index)
            {
                Mapping = SDL_GameControllerMapping(GameController);
            }
        }

        /* Example mapping:
         * c05000000000000c405000000000000,PS4 Controller,
         * a:b1,b:b2,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b12,
         * leftshoulder:b4,leftstick:b10,lefttrigger:a3,leftx:a0,lefty:a1,rightshoulder:b5,
         * rightstick:b11,righttrigger:a4,rightx:a2,righty:a5,start:b9,x:b0,y:b3,platform:Mac OS X,
         */
        private void FormMappingDicts(string mapping)
        {
            var list = mapping.Split(',');
            _axisMappingDict.Clear();
            _buttonMappingDict.Clear();

            foreach (var m in list)
            {
                var ms = m.Split(':');
                if (ms.Length < 2) continue;
                var name = ms[0];
                var bind = ms[1];
                int index;
                try
                {
                    index = int.Parse(bind.Substring(1));
                }
                catch
                {
                    continue;
                }

                switch (name)
                {
                    // Buttons
                    case "a":
                        _buttonMappingDict[SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A] = index;
                        break;
                    case "b":
                        _buttonMappingDict[SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B] = index;
                        break;
                    case "x":
                        _buttonMappingDict[SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X] = index;
                        break;
                    case "y":
                        _buttonMappingDict[SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y] = index;
                        break;
                    case "back":
                        _buttonMappingDict[SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK] = index;
                        break;
                    case "guide":
                        _buttonMappingDict[SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_GUIDE] = index;
                        break;
                    case "start":
                        _buttonMappingDict[SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START] = index;
                        break;
                    case "leftstick":
                        _buttonMappingDict[SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSTICK] = index;
                        break;
                    case "rightstick":
                        _buttonMappingDict[SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSTICK] = index;
                        break;
                    case "leftshoulder":
                        _buttonMappingDict[SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER] = index;
                        break;
                    case "rightshoulder":
                        _buttonMappingDict[SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER] = index;
                        break;

                    // Axes
                    case "leftx":
                        _axisMappingDict[SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX] = index;
                        break;
                    case "righttrigger":
                        _axisMappingDict[SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT] = index;
                        break;
                    case "rightx":
                        _axisMappingDict[SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX] = index;
                        break;
                    case "righty":
                        _axisMappingDict[SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY] = index;
                        break;
                    case "lefttrigger":
                        _axisMappingDict[SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT] = index;
                        break;
                    case "lefty":
                        _axisMappingDict[SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY] = index;
                        break;

                    default:
                        continue;
                }
            }
        }

        private void UpdateNames()
        {
            if (!Connected) return;

#if DEBUG
            Debug.Log($"Attempting to update mappings for controller {Name}");
#endif
            _axisNames = new List<string>();
            for (var i = 0; i < SDL_JoystickNumAxes(DevicePointer); i++)
            {
                string name;
                if (IsGameController)
                {
                    name = GetAxisNameFromEnum((SDL_GameControllerAxis)i);
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
            for (var i = 0; i < SDL_JoystickNumBalls(DevicePointer); i++)
            {
                _ballNames.Add(string.Format(Strings.Controller_BallName_Default, i + 1));
            }

            _hatNames = new List<string>();
            for (var i = 0; i < SDL_JoystickNumHats(DevicePointer); i++)
            {
                _hatNames.Add(IsGameController && i == 0
                    ? Strings.Controller_HatName_DPAD
                    : string.Format(Strings.Controller_HatName_Default, i + 1));
            }

            _buttonNames = new List<string>();
            for (var i = 0; i < SDL_JoystickNumButtons(DevicePointer); i++)
            {
                var name = IsGameController 
                    ? GetButtonNameFromEnum((SDL_GameControllerButton)i) 
                    : string.Format(Strings.Controller_ButtonName_Default, i + 1);
                _buttonNames.Add(name);
            }
#if DEBUG
            Debug.Log("Successfully updated mappings.");
#endif
        }

        /// <summary>
        /// Returns raw value of an axis.
        /// </summary>
        /// <param name="index">Index of the axis.</param>
        /// <param name="smooth">Axis smoothing.</param>
        /// <returns>value in range [-1, 1]</returns>
        public float GetAxis(int index, bool smooth = false)
        {
            if (IsGameController) index = GetIndexForAxis((SDL_GameControllerAxis)index);
            return smooth ? _axisValuesSmooth[index] : _axisValuesRaw[index];
        }

        /// <summary>
        /// Takes integer specifying the type of the axis
        /// and returns the index of the axis on the controller.
        /// </summary>
        internal int GetIndexForAxis(SDL_GameControllerAxis i)
        {
            return _axisMappingDict.ContainsKey(i) ? _axisMappingDict[i] : (int)i;
        }

        /// <summary>
        /// Takes integer specifying the type of the button
        /// and returns the index of the button on the controller.
        /// </summary>
        internal int GetIndexForButton(SDL_GameControllerButton i)
        {
            return _buttonMappingDict.ContainsKey(i) ? _buttonMappingDict[i] : (int)i;
        }

        /// <summary>
        /// Closes the device and unsubscribes it from poll events.
        /// Is called on disconnected devices.
        /// </summary>
        public void Dispose()
        {
#if DEBUG
            Debug.Log($"Attempting to dispose controller. IsGameController: {IsGameController}");
#endif
            if (IsGameController)
                SDL_GameControllerClose(GameController);
            else
                SDL_JoystickClose(DevicePointer);

            DeviceManager.OnDeviceRemapped -= UpdateMapping;
            ACM.Instance.OnUpdate -= Update;
#if DEBUG
            Debug.Log("Successfully disposed controller.");
#endif
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
