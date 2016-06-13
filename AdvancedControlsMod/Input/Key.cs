using UnityEngine;

namespace AdvancedControls.Input
{
    public class Key : Button
    {
        KeyCode keycode;

        public bool IsDown { get { return UnityEngine.Input.GetKey(keycode); } }
        public bool Pressed { get { return UnityEngine.Input.GetKeyDown(keycode); } }
        public bool Released { get { return UnityEngine.Input.GetKeyUp(keycode); } }
        public float Value { get { return IsDown ? 1 : 0; } }
        public string Name { get { return keycode.ToString(); } }
        public bool Connected { get; } = true;

        public Key(KeyCode keycode)
        {
            this.keycode = keycode;
        }
    }
}
