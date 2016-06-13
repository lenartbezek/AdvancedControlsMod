namespace AdvancedControls.Input
{
    /// <summary>
    /// Translates a joystick hat position into a button.
    /// </summary>
    public class HatButton : Button
    {
        private Controller controller;
        private int index;
        private byte down_state;
        private string direction;

        private bool down = false;
        private bool pressed = false;
        private bool released = false;

        public bool IsDown { get { return down; } }
        public bool Pressed { get { return pressed; } }
        public bool Released { get { return released; } }
        public float Value { get { return down ? 1 : 0; } }
        public string Name { get { return controller.HatNames[index] + " - " + direction; } }
        public bool Connected { get { return controller != null && controller.Connected; } }

        public HatButton(Controller controller, int index, byte down_state)
        {
            this.controller = controller;
            this.index = index;
            this.down_state = down_state;
            if ((down_state & SDL.SDL_HAT_UP) > 0)
                direction = "UP";
            else if ((down_state & SDL.SDL_HAT_DOWN) > 0)
                direction = "DOWN";
            else if ((down_state & SDL.SDL_HAT_LEFT) > 0)
                direction = "LEFT";
            else if ((down_state & SDL.SDL_HAT_RIGHT) > 0)
                direction = "RIGHT";

            AdvancedControlsMod.EventManager.OnHatMotion += HandleEvent;
        }

        private void HandleEvent(SDL.SDL_Event e)
        {
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
    }
}
