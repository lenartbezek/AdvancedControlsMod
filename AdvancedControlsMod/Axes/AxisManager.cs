using System.Collections.Generic;
using Lench.AdvancedControls.Controls;

namespace Lench.AdvancedControls.Axes
{
    /// <summary>
    /// Axis manager manages references to all locally saved and machine saved axes.
    /// </summary>
    public static class AxisManager
    {
        internal static Dictionary<string, InputAxis> LocalAxes = new Dictionary<string, InputAxis>();
        internal static Dictionary<string, InputAxis> MachineAxes = new Dictionary<string, InputAxis>();

        /// <summary>
        /// Returns an axis with given name.
        /// Returns null if such axis is not found.
        /// If the axis with the same name is present in machine and locally, it returns the machine instance.
        /// </summary>
        /// <param name="name">Name of the axis.</param>
        /// <returns>InputAxis object.</returns>
        public static InputAxis Get(string name)
        {
            if (name == null) return null;
            if (MachineAxes.ContainsKey(name))
                return MachineAxes[name];
            if (LocalAxes.ContainsKey(name))
                return LocalAxes[name];
            return null;
        }

        /// <summary>
        /// Saves the axis locally.
        /// Overwrites axis if saved under an existing name.
        /// </summary>
        /// <param name="axis">InputAxis object.</param>
        public static void Save(InputAxis axis)
        {
            if (LocalAxes.ContainsKey(axis.Name))
            {
                LocalAxes[axis.Name].Delete();
                LocalAxes[axis.Name] = axis;
                axis.Local = true;
            }
            else
            {
                LocalAxes.Add(axis.Name, axis);
            }
        }

        /// <summary>
        /// Adds the axis to the machine.
        /// Intended to be called on loading the axes from the machine.
        /// </summary>
        /// <param name="axis">InputAxis object.</param>
        public static void Add(InputAxis axis)
        {
            if (MachineAxes.ContainsKey(axis.Name))
            {
                MachineAxes[axis.Name] = axis;
                axis.Local = false;
            }
            else
            {
                MachineAxes.Add(axis.Name, axis);
            }
        }

        /// <summary>
        /// Deletes a locally saved axis.
        /// </summary>
        /// <param name="name">Name of the axis.</param>
        public static void Delete(string name)
        {
            LocalAxes[name].Delete();
            LocalAxes.Remove(name);
        }

        /// <summary>
        /// Returns all active axes from a list of controls.
        /// Does not include duplicates.
        /// </summary>
        /// <param name="controls">List of controls.</param>
        /// <returns>Returns a dictionary of axes with their names as the keys.</returns>
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
