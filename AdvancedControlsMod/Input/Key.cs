using System;
using UnityEngine;

namespace Lench.AdvancedControls.Input
{
    /// <summary>
    ///     Translates Unity Input to ACM button for mapping in input axes.
    /// </summary>
    public class Key : Button
    {
        private readonly KeyCode _keycode;

        /// <summary>
        ///     Creates a key button from a keycode.
        /// </summary>
        /// <param name="keycode">UnityEngine.KeyCode</param>
        public Key(KeyCode keycode)
        {
            _keycode = keycode;
        }

        /// <summary>
        ///     Creates a key button from an identifier string.
        /// </summary>
        /// <param name="id"></param>
        public Key(string id)
        {
            var args = id.Split(':');
            if (args[0].Equals("key"))
                _keycode = (KeyCode) Enum.Parse(typeof(KeyCode), args[1]);
            else
                throw new FormatException("Specified ID does not represent a key.");
        }

        /// <summary>
        ///     Keyboard button identifying string of the following format:
        ///     key:[UnityEngine.KeyCode]
        /// </summary>
        public override string ID => "key:" + _keycode;

#pragma warning disable CS1591
        public override bool IsDown => UnityEngine.Input.GetKey(_keycode);
        public override bool Pressed => UnityEngine.Input.GetKeyDown(_keycode);
        public override bool Released => UnityEngine.Input.GetKeyUp(_keycode);
        public override float Value => IsDown ? 1 : 0;
        public override string Name => _keycode.ToString();
        public override bool Connected { get; } = true;
#pragma warning restore CS1591
    }
}