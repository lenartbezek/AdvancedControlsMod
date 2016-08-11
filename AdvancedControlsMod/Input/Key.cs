using System;
using UnityEngine;

namespace Lench.AdvancedControls.Input
{
    /// <summary>
    /// Translates Unity Input to ACM button for mapping in input axes.
    /// </summary>
    public class Key : Button
    {
        private KeyCode keycode;

        /// <summary>
        /// Keyboard button identifying string of the following format:
        /// key:[UnityEngine.KeyCode]
        /// </summary>
        public override string ID { get { return "key:" + keycode.ToString(); } }

#pragma warning disable CS1591
        public override bool IsDown { get { return UnityEngine.Input.GetKey(keycode); } }
        public override bool Pressed { get { return UnityEngine.Input.GetKeyDown(keycode); } }
        public override bool Released { get { return UnityEngine.Input.GetKeyUp(keycode); } }
        public override float Value { get { return IsDown ? 1 : 0; } }
        public override string Name { get { return keycode.ToString(); } }
        public override bool Connected { get; } = true;
#pragma warning restore CS1591

        /// <summary>
        /// Creates a key button from a keycode.
        /// </summary>
        /// <param name="keycode">UnityEngine.KeyCode</param>
        public Key(KeyCode keycode)
        {
            this.keycode = keycode;
        }

        /// <summary>
        /// Creates a key button from an identifier string.
        /// </summary>
        /// <param name="id"></param>
        public Key(string id)
        {
            var args = id.Split(':');
            if (args[0].Equals("key"))
                keycode = (KeyCode)Enum.Parse(typeof(KeyCode), args[1]);
            else
                throw new FormatException("Specified ID does not represent a key.");
        }
    }
}
