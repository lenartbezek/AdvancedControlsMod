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
                Axes[name] = axis;
            }
            else
            {
                Axes.Add(name, axis);
            }
        }


        public static void Remove(string name)
        {
            Axes.Remove(name);
        }

        public static List<InputAxis> GetActiveAxes(List<Control> controls)
        {
            var list = new List<InputAxis>();
            foreach (Control c in controls)
            {
                if (c.Enabled && Get(c.Axis) != null)
                {
                    if (!list.Contains(Get(c.Axis))) list.Add(Get(c.Axis));
                }
            }
            return list;
        }
    }
}
