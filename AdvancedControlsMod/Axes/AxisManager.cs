﻿using System.Collections.Generic;
using Lench.AdvancedControls.Input;
// ReSharper disable PossibleNullReferenceException

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
        public static void AddLocalAxis(InputAxis axis)
        {
            // overwrite existing axis or create new
            if (LocalAxes.ContainsKey(axis.Name))
            {
                LocalAxes[axis.Name].Delete();
                LocalAxes[axis.Name] = axis;
            }
            else
            {
                LocalAxes.Add(axis.Name, axis);
            }
            // remove axis in machine embedded axis list
            if (MachineAxes.ContainsKey(axis.Name))
                RemoveMachineAxis(axis.Name);
        }

        /// <summary>
        /// Adds the axis to the machine.
        /// Intended to be called on loading the axes from the machine.
        /// If there is an identical axis already locally saved, it does nothing.
        /// </summary>
        /// <param name="axis">InputAxis object.</param>
        public static void AddMachineAxis(InputAxis axis)
        {
            if (LocalAxes.ContainsKey(axis.Name) && axis.Equals(LocalAxes[axis.Name]))
                return;
            MachineAxes[axis.Name] = axis;
        }

        /// <summary>
        /// Removes a machine bound axis.
        /// </summary>
        /// <param name="name">Name of the axis.</param>
        public static void RemoveMachineAxis(string name)
        {
            MachineAxes.Remove(name);
        }

        /// <summary>
        /// Deletes a locally saved axis.
        /// </summary>
        /// <param name="name">Name of the axis.</param>
        public static void RemoveLocalAxis(string name)
        {
            if (LocalAxes.ContainsKey(name))
            {
                LocalAxes[name].Delete();
                LocalAxes.Remove(name);
            }
        }

        /// <summary>
        /// For every axis embedded in machine save, checks if associated controller is connected.
        /// If not, it attempts to bind it to another controller.
        /// </summary>
        public static void ResolveMachineAxes()
        {
            foreach (var entry in MachineAxes)
            {
                var axis = entry.Value;

                switch (axis.Type)
                {
                    case AxisType.Key:
                        ResolveButton((axis as KeyAxis).PositiveBind);
                        ResolveButton((axis as KeyAxis).NegativeBind);
                        continue;
                    case AxisType.Controller:
                        ResolveControllerAxis(axis as ControllerAxis);
                        continue;
                }
            }
        }

        /// <summary>
        /// If the button is a controller button and the associated controller is not found, it remaps it to the first connected device.
        /// </summary>
        public static void ResolveButton(Button button)
        {
            if (button == null) return;
            if (button.GetType() == typeof(JoystickButton))
            {
                var joybutton = button as JoystickButton;
                var controller = Controller.Get(joybutton.GUID);
                if (controller == null && Controller.NumDevices > 0)
                {
                    controller = Controller.Get(0);
                    joybutton.GUID = controller.GUID;
                }
            }
            if (button.GetType() == typeof(HatButton))
            {
                var hatbutton = button as HatButton;
                var controller = Controller.Get(hatbutton.GUID);
                if (controller == null && Controller.NumDevices > 0)
                {
                    controller = Controller.Get(0);
                    hatbutton.GUID = controller.GUID;
                }
            }
        }

        /// <summary>
        /// If the associated controller is not found, it remaps it to the first connected device.
        /// </summary>
        public static void ResolveControllerAxis(ControllerAxis axis)
        {
            var controller = Controller.Get(axis.GUID);
            if (controller == null && Controller.NumDevices > 0)
            {
                controller = Controller.Get(0);
                axis.GUID = controller.GUID;
            }
                
        }
    }
}
