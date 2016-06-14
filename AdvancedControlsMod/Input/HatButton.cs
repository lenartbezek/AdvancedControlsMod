using System;

namespace AdvancedControls.Input
{
    /// <summary>
    /// Translates a joystick hat position into a button.
    /// </summary>
    public class HatButton : Button
    {
        private Controller controller;
        private int index;
        private Guid guid;
        private byte down_state;
        private string direction;

        private bool down = false;
        private bool pressed = false;
        private bool released = false;

        public string ID { get { return "hat:" + index + ":" + down_state + ":" + guid; } }
        public bool IsDown { get { return down; } }
        public bool Pressed { get { return pressed; } }
        public bool Released { get { return released; } }
        public float Value { get { return down ? 1 : 0; } }
        public string Name { get { return controller != null ? controller.HatNames[index] + " - " + direction : "<color=#FF0000>Unknown hat</color>"; } }
        public bool Connected { get { return controller != null && controller.Connected; } }

        public HatButton(Controller controller, int index, byte down_state)
        {
            this.controller = controller;
            this.index = index;
            this.guid = controller.GUID;
            this.down_state = down_state;
            if ((down_state & SDL.SDL_HAT_UP) > 0)
                direction = "UP";
            else if ((down_state & SDL.SDL_HAT_DOWN) > 0)
                direction = "DOWN";
            else if ((down_state & SDL.SDL_HAT_LEFT) > 0)
                direction = "LEFT";
            else if ((down_state & SDL.SDL_HAT_RIGHT) > 0)
                direction = "RIGHT";

            ACM.Instance.EventManager.OnHatMotion += HandleEvent;
            ACM.Instance.EventManager.OnDeviceAdded += UpdateDevice;
        }

        public HatButton(string id)
        {
            var args = id.Split(':');
            if (args[0].Equals("hat"))
            {
                index = int.Parse(args[1]);
                down_state = byte.Parse(args[2]);
                guid = new Guid(args[3]);
                controller = Controller.Get(guid);
            }
            else
                throw new FormatException("Specified ID does not represent a hat button.");

            if ((down_state & SDL.SDL_HAT_UP) > 0)
                direction = "UP";
            else if ((down_state & SDL.SDL_HAT_DOWN) > 0)
                direction = "DOWN";
            else if ((down_state & SDL.SDL_HAT_LEFT) > 0)
                direction = "LEFT";
            else if ((down_state & SDL.SDL_HAT_RIGHT) > 0)
                direction = "RIGHT";

            ACM.Instance.EventManager.OnHatMotion += HandleEvent;
            ACM.Instance.EventManager.OnDeviceAdded += UpdateDevice;
        }

        private void HandleEvent(SDL.SDL_Event e)
        {
            if (controller == null) return;
            if (e.jhat.which != controller.Index &&
                e.jhat.which != controller.Index)
                return;
            if (e.jhat.hat == index)
            {
                bool down = (e.jhat.hatValue & down_state) > 0;
                pressed = this.down != down && down;
                released = this.down != down && !down;
                this.down = down;
            }
        }

        private void UpdateDevice(SDL.SDL_Event e)
        {
            controller = Controller.Get(guid);
        }
    }
}
