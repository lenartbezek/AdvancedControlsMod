using AdvancedControls.Axes;
using AdvancedControls.Input;
using System;
using System.Collections.Generic;

namespace AdvancedControls
{
    public static class AdvancedControls
    {
        public static int GetNumControllers()
        {
            return Controller.NumDevices;
        }

        public static Controller GetController(int i)
        {
            return Controller.Get(i);
        }

        public static Controller GetController(string guid)
        {
            return Controller.Get(new Guid(guid));
        }

        public static int GetNumAxes()
        {
            return AxisManager.Axes.Count;
        }

        public static InputAxis GetAxis(int i)
        {
            var names = new List<string>(AxisManager.Axes.Keys);
            names.Sort();
            return GetAxis(names[i]);
        }

        public static InputAxis GetAxis(string name)
        {
            return AxisManager.Get(name);
        }

        public static Button CreateButtonFromKeycode(UnityEngine.KeyCode key)
        {
            return new Key(key);
        }
    }
}
