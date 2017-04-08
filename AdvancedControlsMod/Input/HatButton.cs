using System;

namespace Lench.AdvancedControls.Input
{
    /// <summary>
    ///     Translates a joystick hat position into a button.
    /// </summary>
    public class HatButton : Button
    {
        private readonly byte _downState;
        private Controller _controller;

        private bool _down;
        private Guid _guid;
        private bool _pressed;
        private bool _released;

        /// <summary>
        ///     Creates a hat button for given controller.
        /// </summary>
        /// <param name="controller">Controller class.</param>
        /// <param name="index">Index of the hat button.</param>
        /// <param name="downState">Down state byte. For example SDL.SDL_HAT_UP</param>
        public HatButton(Controller controller, int index, byte downState)
        {
            _controller = controller;
            Index = index;
            _guid = controller.GUID;
            _downState = downState;
            SdlManager.OnDeviceAdded += UpdateDevice;
        }

        /// <summary>
        ///     Creates a hat button from an identifier string.
        ///     Intended for loading buttons from xml.
        ///     Throws FormatException.
        /// </summary>
        /// <param name="id">Hat button identifier string.</param>
        public HatButton(string id)
        {
            var args = id.Split(':');
            if (args[0].Equals("hat"))
            {
                Index = int.Parse(args[1]);
                _downState = byte.Parse(args[2]);
                _guid = new Guid(args[3]);
                _controller = Controller.Get(_guid);
            }
            else
            {
                throw new FormatException("Specified ID does not represent a hat button.");
            }

            SdlManager.OnHatMotion += HandleEvent;
            SdlManager.OnDeviceAdded += UpdateDevice;
        }

        /// <summary>
        ///     Hat button identifying string of the following format:
        ///     hat:[index]:[down_state_byte]:[device_guid]
        /// </summary>
        public override string ID => "hat:" + Index + ":" + _downState + ":" + _guid;

        /// <summary>
        ///     Guid of the associated controller.
        ///     Changing it updates the controller.
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
        ///     Index of the button on a device.
        /// </summary>
        public int Index { get; }

        private void HandleEvent(SDL.SDL_Event e)
        {
            if (_controller == null) return;
            if (e.jhat.which != _controller.Index &&
                e.jhat.which != _controller.Index)
                return;
            if (e.jhat.hat != Index) return;

            var down = (e.jhat.hatValue & _downState) > 0;
            _pressed = _down != down && down;
            _released = _down != down && !down;
            _down = down;
        }

        private void UpdateDevice(SDL.SDL_Event e)
        {
            _controller = Controller.Get(_guid);
        }

#pragma warning disable CS1591
        public override bool IsDown => _down;
        public override bool Pressed => _pressed;
        public override bool Released => _released;
        public override float Value => _down ? 1 : 0;
        public override string Name => ID;
        public override bool Connected => _controller != null && _controller.Connected && Index < _controller.NumHats;
#pragma warning restore CS1591
    }
}