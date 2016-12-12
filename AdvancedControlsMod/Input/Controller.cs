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
                return "Unknown axis";
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
                return "Unknown ball";
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
                return "Unknown hat";
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
                return "Unknown button";
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
        public bool IsGameController { get; private set; }

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
        public static void AddDevice(int index)
        {
            RemoveDisconnected();
            if (index < NumDevices) return;
            ControllerList.Add(new Controller(NumDevices));
        }

        /// <summary>
        /// Removes and disposes all disconnected devices.
        /// </summary>
        public static void RemoveDisconnected()
        {
            ControllerList.RemoveAll((c) => !c.Connected);
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
                return ControllerList.First((c) => c.GUID == guid);
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
                return ControllerList[0];
            }
            catch
            {
                return null;
            }
        }

        private Controller(int index)
        {
            if (index > SDL.SDL_NumJoysticks())
                throw new InvalidOperationException("Cannot open controller " + index + " when only " + SDL.SDL_NumJoysticks()+" are connected.");

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

            for (int i = 0; i < SDL.SDL_JoystickNumHats(DevicePointer); i++)
            {
                Buttons.Add(new HatButton(this, i, SDL.SDL_HAT_UP));
                Buttons.Add(new HatButton(this, i, SDL.SDL_HAT_DOWN));
                Buttons.Add(new HatButton(this, i, SDL.SDL_HAT_LEFT));
                Buttons.Add(new HatButton(this, i, SDL.SDL_HAT_RIGHT));
            }

            for (int i = 0; i < SDL.SDL_JoystickNumButtons(DevicePointer); i++)
            {
                Buttons.Add(new JoystickButton(this, i));
            }

            DeviceManager.OnDeviceRemapped += UpdateMappings;
            ACM.Instance.OnUpdate += Update;

            // Debug
            if (IsGameController)
                Debug.Log("[ACM]: Game controller connected: " + Name);
            else
                Debug.Log("[ACM]: Joystick connected: " + Name);
            Debug.Log("\tGuid: " + GUID);
        }

        private void Update()
        {
            var d = Mathf.Clamp(Time.deltaTime * 12, 0, 1);
            for (int i = 0; i < NumAxes; i++)
            {
                int axis = i;
                if (IsGameController)
                    axis = SDL.SDL_GameControllerGetBindForAxis(GameController, (SDL.SDL_GameControllerAxis)i).axis;
                _axisValuesRaw[axis] = SDL.SDL_JoystickGetAxis(DevicePointer, axis) / 32767.0f;
                _axisValuesSmooth[axis] = _axisValuesSmooth[axis] * (1 - d) + _axisValuesRaw[axis] * d;
            }
            for (int i = 0; i < NumBalls; i++)
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
                    return "Left X";
                case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY:
                    return "Left Y";
                case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX:
                    return "Right X";
                case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY:
                    return "Right Y";
                case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT:
                    return "Left Trigger";
                case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT:
                    return "Right Trigger";
                default:
                    return "Axis " + ((int)i + 1);
            }
        }

        private static string GetButtonNameFromEnum(SDL.SDL_GameControllerButton i)
        {
            switch (i)
            {
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A:
                    return "A Button";
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B:
                    return "B Button";
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X:
                    return "X Button";
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y:
                    return "Y Button";
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK:
                    return "Back Button";
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_GUIDE:
                    return "Guide Button";
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START:
                    return "Start Button";
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSTICK:
                    return "Left Stick";
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSTICK:
                    return "Right Stick";
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER:
                    return "Left Shoulder";
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER:
                    return "Right Shoulder";
                default:
                    return "Button " + ((int)i + 1);
            }
        }

        private void UpdateMappings()
        {
            if (!Connected) return;

            _axisNames = new List<string>();
            for (int i = 0; i < SDL.SDL_JoystickNumAxes(DevicePointer); i++)
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
                            name = "X Axis";
                            break;
                        case 1:
                            name = "Y Axis";
                            break;
                        default:
                            name = "Axis " + (i + 1);
                            break;
                    }
                }
                _axisNames.Add(name);
            }

            _ballNames = new List<string>();
            for (int i = 0; i < SDL.SDL_JoystickNumBalls(DevicePointer); i++)
                _ballNames.Add("Ball " + (i + 1));

            _hatNames = new List<string>();
            for (int i = 0; i < SDL.SDL_JoystickNumHats(DevicePointer); i++)
                if (IsGameController)
                    _hatNames.Add("DPAD");
                else
                    _hatNames.Add("Hat " + (i + 1));

            _buttonNames = new List<string>();
            for (int i = 0; i < SDL.SDL_JoystickNumButtons(DevicePointer); i++)
            {
                string name;
                if (IsGameController)
                {
                    name = GetButtonNameFromEnum((SDL.SDL_GameControllerButton)i);
                }
                else
                {
                    name = "Button " + (i + 1);
                }
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
