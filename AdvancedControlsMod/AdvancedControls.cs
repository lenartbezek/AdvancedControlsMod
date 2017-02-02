using Lench.AdvancedControls.Axes;
using Lench.AdvancedControls.Input;
using System;
using System.Collections.Generic;

namespace Lench.AdvancedControls
{
    /// <summary>
    /// Function container exposed to Python script.
    /// </summary>
    public static class AdvancedControls
    {
        /// <summary>
        /// Returns number of currently connected devices.
        /// SDL must be initialized to use this.
        /// </summary>
        public static int GetNumControllers()
        {
            return Controller.NumDevices;
        }

        /// <summary>
        /// Returns a reference to a controller at a given index.
        /// Returns null (None) if there is no controller at that index.
        /// SDL must be initialized to use this.
        /// </summary>
        /// <param name="i">Index of the controller.</param>
        public static Controller GetController(int i)
        {
            return Controller.Get(i);
        }

        /// <summary>
        /// Returns a reference to a controller with given GUID.
        /// Returns null (None) if there is no such controller.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static Controller GetController(string guid)
        {
            return Controller.Get(new Guid(guid));
        }

        /// <summary>
        /// Returns the number of locally saved axes.
        /// </summary>
        public static int GetNumAxes()
        {
            return AxisManager.LocalAxes.Count;
        }

        /// <summary>
        /// Returns a locally saved axis at a given index in an array, sorted by axis names.
        /// </summary>
        /// <param name="i">Axis index</param>
        public static InputAxis GetAxis(int i)
        {
            var names = new List<string>(AxisManager.LocalAxes.Keys);
            names.Sort();
            return GetAxis(names[i]);
        }

        /// <summary>
        /// Returns an axis with given name.
        /// Looks in machine axes first, then in locally saved axes.
        /// This is the preferred method.
        /// </summary>
        /// <param name="name">Name of the axis.</param>
        /// <returns></returns>
        public static InputAxis GetAxis(string name)
        {
            return AxisManager.Get(name);
        }

        /// <summary>
        /// Creates and returns a button, mapped to the specified key code.
        /// Useful for mapping it in input axes.
        /// </summary>
        /// <param name="key">UnityEngine.KeyCode</param>
        /// <returns>New Button object.</returns>
        public static Button CreateButtonFromKeycode(UnityEngine.KeyCode key)
        {
            return new Key(key);
        }
    }
}
