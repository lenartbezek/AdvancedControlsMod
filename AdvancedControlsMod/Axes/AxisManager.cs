using System.Collections.Generic;
using AdvancedControls.Controls;

namespace AdvancedControls.Axes
{
    public static class AxisManager
    {
        public static Dictionary<string, InputAxis> Axes = new Dictionary<string, InputAxis>();

        public static InputAxis Get(string name)
        {
            if (name == null) return null;
            try
            {
                return Axes[name];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public static void Put(string name, InputAxis axis)
        {
            if (Axes.ContainsKey(name))
            {
                Axes[name].Delete();
                Axes[name] = axis;
            }
            else
            {
                Axes.Add(name, axis);
            }
        }


        public static void Remove(string name)
        {
            Axes[name].Delete();
            Axes.Remove(name);
        }

        public static Dictionary<string, InputAxis> GetActiveAxes(List<Control> controls)
        {
            var dict = new Dictionary<string, InputAxis>();
            foreach (Control c in controls)
            {
                if (c.Enabled && Get(c.Axis) != null)
                    dict[c.Axis] = Get(c.Axis);
            }
            return dict;
        }
    }
}
