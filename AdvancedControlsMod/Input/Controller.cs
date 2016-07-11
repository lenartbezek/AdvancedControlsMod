using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lench.AdvancedControls.Input
{
    public class Controller : IDisposable, IEquatable<Controller>
    {
        internal IntPtr device_pointer;
        internal IntPtr game_controller;

        private float[] axis_values_raw;
        private float[] axis_values_smooth;
        private float[,] ball_values_raw;
        private float[,] ball_values_smooth;

        public List<string> AxisNames;
        public List<string> BallNames;
        public List<string> HatNames;
        public List<string> ButtonNames;

        public readonly List<Button> Buttons;

        public int Index { get { return SDL.SDL_JoystickInstanceID(device_pointer); } }
        public string Name { get { return IsGameController ? SDL.SDL_GameControllerName(game_controller) : SDL.SDL_JoystickName(device_pointer); } }
        public Guid GUID { get { return SDL.SDL_JoystickGetGUID(device_pointer); } }
        public bool Connected { get { return SDL.SDL_JoystickGetAttached(device_pointer) == SDL.SDL_bool.SDL_TRUE; } }
        public bool IsGameController { get; private set; }

        public int NumAxes { get { return SDL.SDL_JoystickNumAxes(device_pointer); } }
        public int NumBalls { get { return SDL.SDL_JoystickNumBalls(device_pointer); } }
        public int NumHats { get { return SDL.SDL_JoystickNumHats(device_pointer); } }
        public int NumButtons { get { return SDL.SDL_JoystickNumButtons(device_pointer); } }

        internal static List<Guid> DeviceList = new List<Guid>();
        internal static Dictionary<Guid, Controller> Devices = new Dictionary<Guid, Controller>();

        public static int NumDevices { get { return Devices.Count;}}

        internal static void AddDevice(int index)
        {
            RemoveDisconnected();
            if (index < NumDevices) return;
            var controller = new Controller(NumDevices);
            DeviceList.Add(controller.GUID);
            Devices.Add(controller.GUID, controller);
        }

        internal static void RemoveDisconnected()
        {
            var remove = new List<Guid>();

            foreach (KeyValuePair<Guid, Controller> entry in Devices)
                if (!entry.Value.Connected) remove.Add(entry.Key);

            foreach (Guid c in remove)
            {
                Devices[c].Dispose();
                Devices.Remove(c);
                DeviceList.Remove(c);
            }
        }

        public static Controller Get(Guid guid)
        {
            if (Devices.ContainsKey(guid))
                return Devices[guid];
            else
                return null;
        }

        public static Controller Get(int id)
        {
            try
            {
                return Devices[DeviceList[id]];
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
                game_controller = SDL.SDL_GameControllerOpen(index);
                device_pointer = SDL.SDL_GameControllerGetJoystick(game_controller);
            }
            else
            {
                device_pointer = SDL.SDL_JoystickOpen(index);
            }

            UpdateMappings();
            Buttons = new List<Button>();

            axis_values_raw = new float[SDL.SDL_JoystickNumAxes(device_pointer)];
            axis_values_smooth = new float[SDL.SDL_JoystickNumAxes(device_pointer)];

            ball_values_raw = new float[SDL.SDL_JoystickNumBalls(device_pointer), 2];
            ball_values_smooth = new float[SDL.SDL_JoystickNumBalls(device_pointer), 2];

            for (int i = 0; i < SDL.SDL_JoystickNumHats(device_pointer); i++)
            {
                Buttons.Add(new HatButton(this, i, SDL.SDL_HAT_UP));
                Buttons.Add(new HatButton(this, i, SDL.SDL_HAT_DOWN));
                Buttons.Add(new HatButton(this, i, SDL.SDL_HAT_LEFT));
                Buttons.Add(new HatButton(this, i, SDL.SDL_HAT_RIGHT));
            }

            for (int i = 0; i < SDL.SDL_JoystickNumButtons(device_pointer); i++)
            {
                Buttons.Add(new JoystickButton(this, i));
            }

            ACM.Instance.EventManager.OnDeviceRemapped += UpdateMappings;
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
                    axis = SDL.SDL_GameControllerGetBindForAxis(game_controller, (SDL.SDL_GameControllerAxis)i).axis;
                axis_values_raw[axis] = SDL.SDL_JoystickGetAxis(device_pointer, axis) / 32767.0f;
                axis_values_smooth[axis] = axis_values_smooth[axis] * (1 - d) + axis_values_raw[axis] * d;
            }
            for (int i = 0; i < NumBalls; i++)
            {
                int x, y;
                SDL.SDL_JoystickGetBall(device_pointer, i, out x, out y);
                ball_values_raw[i, 0] = x / 32767.0f;
                ball_values_raw[i, 1] = y / 32767.0f;
                ball_values_smooth[i, 0] = ball_values_smooth[i, 0] * (1 - d) + ball_values_raw[i, 0] * d;
                ball_values_smooth[i, 1] = ball_values_smooth[i, 1] * (1 - d) + ball_values_raw[i, 1] * d;
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
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER:
                    return "Left Shoulder";
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER:
                    return "Right Shoulder";
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK:
                    return "Back Button";
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START:
                    return "Start Button";
                case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_GUIDE:
                    return "Guide Button";
                default:
                    return "Button " + ((int)i + 1);
            }
        }

        private void UpdateMappings()
        {
            if (!Connected) return;

            AxisNames = new List<string>();
            for (int i = 0; i < SDL.SDL_JoystickNumAxes(device_pointer); i++)
            {
                string name = null;
                if (IsGameController)
                {
                    name = GetAxisNameFromEnum((SDL.SDL_GameControllerAxis)i);
                } 
                else
                {
                    if (i == 0) name = "X Axis";
                    else if (i == 1) name = "Y Axis";
                    else name = "Axis " + (i + 1);
                }
                AxisNames.Add(name);
            }

            BallNames = new List<string>();
            for (int i = 0; i < SDL.SDL_JoystickNumBalls(device_pointer); i++)
                BallNames.Add("Ball " + (i + 1));

            HatNames = new List<string>();
            for (int i = 0; i < SDL.SDL_JoystickNumHats(device_pointer); i++)
                if (IsGameController)
                    HatNames.Add("DPAD");
                else
                    HatNames.Add("Hat " + (i + 1));

            ButtonNames = new List<string>();
            for (int i = 0; i < SDL.SDL_JoystickNumButtons(device_pointer); i++)
            {
                string name = null;
                if (IsGameController)
                {
                    name = GetButtonNameFromEnum((SDL.SDL_GameControllerButton)i);
                }
                else
                {
                    name = "Button " + (i + 1);
                }
                ButtonNames.Add(name);
            }
        }

        public float GetAxis(int index, bool smooth = false)
        {
            if (smooth)
                return axis_values_smooth[index];
            else
                return axis_values_raw[index];
        }

        public void Dispose()
        {
            if (IsGameController)
                SDL.SDL_GameControllerClose(game_controller);
            else
                SDL.SDL_JoystickClose(device_pointer);

            ACM.Instance.EventManager.OnDeviceRemapped -= UpdateMappings;
            ACM.Instance.OnUpdate -= Update;
        }

        public bool Equals(Controller other)
        {
            return device_pointer == other.device_pointer;
        }
    }
}
