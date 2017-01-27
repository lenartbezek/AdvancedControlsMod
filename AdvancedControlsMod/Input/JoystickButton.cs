using System;
using Lench.AdvancedControls.Resources;
using Lench.AdvancedControls.UI;

namespace Lench.AdvancedControls.Input
{
    /// <summary>
    /// Joystick button for mapping in input axes.
    /// </summary>
    public class JoystickButton : Button
    {
        private Controller _controller;
        private Guid _guid;

        private bool _down;
        private bool _pressed;
        private bool _released;

        /// <summary>
        /// Joystick button identifying string of the following format:
        /// joy:[index]:[device_guid]
        /// </summary>
        public override string ID => $"joy:{Index}:{_guid}";

        /// <summary>
        /// Guid of the associated controller.
        /// Changing it updates the controller.
        /// </summary>
        public Guid GUID
        {
            get { return _guid; }
            set
            {
                _guid = value;
                _controller = Controller.Get(_guid);
            }
        }

        /// <summary>
        /// Index of the button on a device.
        /// </summary>
        public int Index { get; }

#pragma warning disable CS1591
        public override bool IsDown => _down;
        public override bool Pressed => _pressed;
        public override bool Released => _released;
        public override float Value => _down ? 1 : 0;
        public override string Name => _controller?.GetButtonName(Index) ?? Strings.Controller_ButtonName_UnknownButton;
        public override bool Connected => _controller != null && _controller.Connected;
#pragma warning restore CS1591

        /// <summary>
        /// Creates a joystick button for a given controller.
        /// </summary>
        /// <param name="controller">Controller object.</param>
        /// <param name="index">Index of the button.</param>
        public JoystickButton(Controller controller, int index)
        {
            Index = index;
            _controller = controller;
            _guid = controller.GUID;

            DeviceManager.OnButton += HandleEvent;
            DeviceManager.OnDeviceAdded += UpdateDevice;
        }

        /// <summary>
        /// Creates a joystick button from an identifier string.
        /// </summary>
        /// <param name="id"></param>
        public JoystickButton(string id)
        {
            var args = id.Split(':');
            if (args[0].Equals("joy"))
            {
                Index = int.Parse(args[1]);
                _guid = new Guid(args[2]);
                _controller = Controller.Get(_guid);
            }
            else
                throw new FormatException("Specified ID does not represent a joystick button.");

            DeviceManager.OnButton += HandleEvent;
            DeviceManager.OnDeviceAdded += UpdateDevice;
        }

        private void HandleEvent(SDL.SDL_Event e, bool down)
        {
            if (_controller == null) return;
            if (e.cdevice.which != _controller.Index &&
                e.jdevice.which != _controller.Index)
                return;
            if (_controller.IsGameController)
            {
                var en = (SDL.SDL_GameControllerButton) Index;
                if (e.cbutton.button != _controller.GetIndexForButton(en)) return;

                _pressed = _down != down && down;
                _released = _down != down && !down;
                _down = down;
            }
            else
            {
                if (e.jbutton.button != Index) return;

                _pressed = _down != down && down;
                _released = _down != down && !down;
                _down = down;
            }
        }

        private void UpdateDevice(SDL.SDL_Event e)
        {
            _controller = Controller.Get(_guid);
        }
    }
}
