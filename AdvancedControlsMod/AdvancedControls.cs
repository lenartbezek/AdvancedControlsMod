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

        public static Controller GetController(int id)
        {
            return Controller.Get(id);
        }

        public static Controller GetController(Guid guid)
        {
            return Controller.Get(guid);
        }

        public static int GetNumAxes()
        {
            return AxisManager.Axes.Count;
        }

        public static InputAxis GetAxis(int id)
        {
            var names = new List<string>(AxisManager.Axes.Keys);
            names.Sort();
            return GetAxis(names[id]);
        }

        public static InputAxis GetAxis(string name)
        {
            return AxisManager.Get(name);
        }
    }
}
