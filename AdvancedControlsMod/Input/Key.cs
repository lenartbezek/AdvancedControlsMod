using System;
using UnityEngine;

namespace AdvancedControls.Input
{
    public class Key : Button
    {
        private KeyCode keycode;

        public string ID { get { return "key-" + keycode.ToString(); } }
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

        public Key(string id)
        {
            var args = id.Split('-');
            if (args[0].Equals("key"))
                keycode = (KeyCode)Enum.Parse(typeof(KeyCode), args[0]);
            else
                throw new FormatException("Specified id does not represent a key.");
        }
    }
}
