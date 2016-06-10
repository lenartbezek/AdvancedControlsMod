namespace AdvancedControls.Input
{
    /// <summary>
    /// Joystick button.
    /// </summary>
    public class JoystickButton : Button
    {
        private Controller controller;
        private int index;

        private bool down = false;
        private bool pressed = false;
        private bool released = false;

        public bool IsDown { get { return down; } }
        public bool Pressed { get { return pressed; } }
        public bool Released { get { return released; } }
        public float Value { get { return down ? 1 : 0; } }
        public string Name { get { return controller.ButtonNames[index]; } }

        public JoystickButton(Controller controller, int index)
        {
            this.controller = controller;
            this.index = index;

            AdvancedControlsMod.EventManager.OnButton += HandleEvent;
        }

        private void HandleEvent(SDL.SDL_Event e, bool down)
        {
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
    }
}
