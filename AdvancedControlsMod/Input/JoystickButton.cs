using System;

namespace AdvancedControls.Input
{
    /// <summary>
    /// Joystick button.
    /// </summary>
    public class JoystickButton : Button
    {
        private Controller controller;
        private int index;
        private Guid guid;

        private bool down = false;
        private bool pressed = false;
        private bool released = false;

        public string ID { get { return "joy:" + index + ":" + guid; } }
        public bool IsDown { get { return down; } }
        public bool Pressed { get { return pressed; } }
        public bool Released { get { return released; } }
        public float Value { get { return down ? 1 : 0; } }
        public string Name { get { return controller != null ? controller.ButtonNames[index] : "<color=#FF0000>Unknown button</color>"; } }
        public bool Connected { get { return controller != null && controller.Connected; } }

        public JoystickButton(Controller controller, int index)
        {
            this.controller = controller;
            this.index = index;
            this.guid = controller.GUID;

            ACM.Instance.EventManager.OnButton += HandleEvent;
            ACM.Instance.EventManager.OnDeviceAdded += UpdateDevice;
        }

        public JoystickButton(string id)
        {
            var args = id.Split(':');
            if (args[0].Equals("joy"))
            {
                index = int.Parse(args[1]);
                guid = new Guid(args[2]);
                controller = Controller.Get(guid);
            }
            else
                throw new FormatException("Specified ID does not represent a joystick button.");

            ACM.Instance.EventManager.OnButton += HandleEvent;
            ACM.Instance.EventManager.OnDeviceAdded += UpdateDevice;
        }

        private void HandleEvent(SDL.SDL_Event e, bool down)
        {
            if (controller == null) return;
            if (e.cdevice.which != controller.Index &&
                e.jdevice.which != controller.Index)
                return;
            if (controller.IsGameController)
            {
                if (e.cbutton.button == index)
                {
                    pressed = this.down != down && down;
                    released = this.down != down && !down;
                    this.down = down;
                }
            }
            else
            {
                if (e.jbutton.button == index)
                {
                    pressed = this.down != down && down;
                    released = this.down != down && !down;
                    this.down = down;
                }
            }
        }

        private void UpdateDevice(SDL.SDL_Event e)
        {
            controller = Controller.Get(guid);
        }
    }
}
