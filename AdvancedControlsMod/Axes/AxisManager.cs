using System.Collections.Generic;
using AdvancedControls.Controls;

namespace AdvancedControls.Axes
{
    public static class AxisManager
    {
        internal static Dictionary<string, InputAxis> Axes = new Dictionary<string, InputAxis>();

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

        internal static void Put(string name, InputAxis axis)
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

        internal static void Remove(string name)
        {
            Axes[name].Delete();
            Axes.Remove(name);
        }

        internal static Dictionary<string, InputAxis> GetActiveAxes(List<Control> controls)
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
