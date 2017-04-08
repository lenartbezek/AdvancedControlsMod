using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Lench.AdvancedControls.Input.SDL;

// ReSharper disable UnusedMember.Local

namespace Lench.AdvancedControls.Input
{
    /// <summary>
    ///     Controller class providing access to an SDL device with axis and button mapping.
    /// </summary>
    public class Controller
    {
        private static readonly List<Controller> ControllerList = new List<Controller>();

        private readonly float[] _axisValuesRaw;
        private readonly float[] _axisValuesSmooth;
        private readonly float[,] _ballValuesRaw;
        private readonly float[,] _ballValuesSmooth;

        /// <summary>
        ///     List of all controller buttons, including hats (one for every direction).
        ///     Does not necessarily contain objects equal by reference to buttons bound at input axes.
        /// </summary>
        public readonly List<Button> Buttons;

        internal IntPtr DevicePointer;
        internal IntPtr GameController;

        private Controller(int index)
        {
            if (index > SDL_NumJoysticks())
                throw new InvalidOperationException(
                    $"Cannot open controller {index} when only {SDL_NumJoysticks()} are connected.");

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
                Buttons.Add(new JoystickButton(this, i));

            // TODO: Hook Update and mapping
        }

        /// <summary>
        ///     Index of the device needed to access it through SDL.
        /// </summary>
        public int Index => SDL_JoystickInstanceID(DevicePointer);

        /// <summary>
        ///     Name of the device.
        ///     Returns name from GameControllerMappings.txt if found.
        /// </summary>
        public string Name => IsGameController
            ? SDL_GameControllerName(GameController)
            : SDL_JoystickName(DevicePointer);

        /// <summary>
        ///     GUID of the controller.
        /// </summary>
        public Guid GUID => SDL_JoystickGetGUID(DevicePointer);

        /// <summary>
        ///     Is device currently connected.
        /// </summary>
        public bool Connected => SDL_JoystickGetAttached(DevicePointer) == SDL_bool.SDL_TRUE;

        /// <summary>
        ///     Was device mapping found and is recognized as game controller.
        /// </summary>
        public bool IsGameController { get; }

        /// <summary>
        ///     Number of currently connected devices.
        /// </summary>
        public static int NumDevices => ControllerList.Count;

        /// <summary>
        ///     Adds device connected at given index.
        ///     Does nothing if no device is connected at that index.
        ///     Checks for disconnected devices first.
        ///     Devices shouldn't be added twice.
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
        ///     Removes and disposes all disconnected devices.
        /// </summary>
        /// <returns>Returns the number of removed devices.</returns>
        public static int RemoveDisconnected()
        {
            return ControllerList.RemoveAll(c => !c.Connected);
        }

        /// <summary>
        ///     Returns a Controller object for a device with given guid.
        ///     If such device is not found, returns null.
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
        ///     Returns a Controller object for a device at given index.
        ///     If such device is not found, returns null.
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

        /// <summary>
        ///     Closes the device when garbage collected.
        /// </summary>
        ~Controller()
        {
            if (IsGameController)
                SDL_GameControllerClose(GameController);
            else
                SDL_JoystickClose(DevicePointer);
        }

        private void Update()
        {
            var d = Mathf.Clamp(Time.deltaTime * 12, 0, 1);
            for (var i = 0; i < NumAxes; i++)
            {
                _axisValuesRaw[i] = IsGameController
                    ? SDL_GameControllerGetAxis(GameController, (SDL_GameControllerAxis) i) / short.MaxValue
                    : SDL_JoystickGetAxis(DevicePointer, i) / short.MaxValue;
                _axisValuesSmooth[i] = _axisValuesSmooth[i] * (1 - d) + _axisValuesRaw[i] * d;
            }
            for (var i = 0; i < NumBalls; i++)
            {
                SDL_JoystickGetBall(DevicePointer, i, out int x, out int y);
                _ballValuesRaw[i, 0] = x / (float) short.MaxValue;
                _ballValuesRaw[i, 1] = y / (float) short.MaxValue;
                _ballValuesSmooth[i, 0] = _ballValuesSmooth[i, 0] * (1 - d) + _ballValuesRaw[i, 0] * d;
                _ballValuesSmooth[i, 1] = _ballValuesSmooth[i, 1] * (1 - d) + _ballValuesRaw[i, 1] * d;
            }
        }

        /// <summary>
        ///     Returns raw value of an axis.
        /// </summary>
        /// <param name="index">Index of the axis.</param>
        /// <param name="smooth">Axis smoothing.</param>
        /// <returns>value in range [-1, 1]</returns>
        public float GetAxis(int index, bool smooth = false)
        {
            // TODO: Resolve mapping
            return smooth ? _axisValuesSmooth[index] : _axisValuesRaw[index];
        }

        /// <summary>
        ///     Does this object represent the same device as other.
        /// </summary>
        public bool Equals(Controller other)
        {
            return other != null && DevicePointer == other.DevicePointer;
        }

#pragma warning disable CS1591

        public int NumAxes => SDL_JoystickNumAxes(DevicePointer);
        public int NumBalls => SDL_JoystickNumBalls(DevicePointer);
        public int NumHats => SDL_JoystickNumHats(DevicePointer);
        public int NumButtons => SDL_JoystickNumButtons(DevicePointer);

#pragma warning restore CS1591
    }
}