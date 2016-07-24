using System;
using System.Collections.Generic;
using Lench.AdvancedControls.Controls;
using Lench.AdvancedControls.Axes;
using UnityEngine;
using System.Reflection;

namespace Lench.AdvancedControls
{
    internal static class MachineData
    {
        internal static void Load(MachineInfo machineInfo)
        {
            try
            {
                AxisManager.MachineAxes.Clear();

                // read mod version
                if (!machineInfo.MachineData.HasKey("ac-version")) return;
                var version = new Version(machineInfo.MachineData.ReadString("ac-version").TrimStart('v'));

                // version alert
                if (version < Assembly.GetExecutingAssembly().GetName().Version)
                    Debug.Log("[ACM]: " + machineInfo.Name + " was saved with mod version " + version + ".\n\tIt may not support some newer features.");

                // return if no input axes are present
                if (!machineInfo.MachineData.HasKey("ac-axislist")) return;
                var axes = machineInfo.MachineData.ReadStringArray("ac-axislist");

                // load all axes
                foreach (var name in axes)
                {
                    InputAxis axis = null;
                    if (!machineInfo.MachineData.HasKey("axis-" + name + "-type"))
                        continue;
                    var type = machineInfo.MachineData.ReadString("axis-" + name + "-type");
                    if (type == AxisType.Chain.ToString())
                        axis = new ChainAxis(name);
                    if (type == AxisType.Controller.ToString())
                        axis = new ControllerAxis(name);
                    if (type == AxisType.Custom.ToString())
                        axis = new CustomAxis(name);
                    if (type == AxisType.Inertial.ToString())
                        axis = new InertialAxis(name);
                    if (type == AxisType.Standard.ToString())
                        axis = new StandardAxis(name);
                    if (type == AxisType.Mouse.ToString())
                        axis = new MouseAxis(name);
                    if (axis != null)
                    {
                        axis?.Load(machineInfo);
                        AxisManager.AddMachineAxis(axis);
                    }
                }

                // refresh chain axis links
                foreach (var entry in AxisManager.MachineAxes)
                {
                    if (entry.Value.Type == AxisType.Chain)
                        (entry.Value as ChainAxis).RefreshLinks();
                }

                // resolve from foreign controllers
                AxisManager.ResolveMachineAxes();

                // load all controls
                foreach (BlockInfo blockInfo in machineInfo.Blocks)
                {
                    if (!blockInfo.BlockData.HasKey("ac-controllist")) continue;
                    var control_list = ControlManager.GetBlockControls(blockInfo.ID, blockInfo.Guid);
                    var control_names = blockInfo.BlockData.ReadStringArray("ac-controllist");
                    foreach (string name in control_names)
                    {
                        foreach (Control c in control_list)
                        {
                            if (name == c.Name)
                                c.Load(blockInfo);
                        }
                    }
                }

                ACM.Instance.LoadedMachine = true;
            }
            catch (Exception e)
            {
                Debug.Log("[ACM]: Error loading machine's controls:");
                Debug.LogException(e);
            }
        }

        internal static void Save(MachineInfo machineInfo)
        {
            try
            {
                var axes = new List<string>();

                foreach (BlockInfo blockInfo in machineInfo.Blocks)
                {
                    if (ControlManager.Blocks.ContainsKey(blockInfo.Guid))
                    {
                        var controls = ControlManager.GetActiveBlockControls(blockInfo.Guid);
                        if (controls.Count == 0) continue;
                        var control_names = new List<string>();
                        foreach (Control c in controls)
                        {
                            if (!axes.Contains(c.Axis))
                                axes.Add(c.Axis);
                            control_names.Add(c.Name);
                            c.Save(blockInfo);
                        }
                        blockInfo.BlockData.Write("ac-controllist", control_names.ToArray());
                    }
                }

                if (axes.Count != 0)
                {
                    machineInfo.MachineData.Write("ac-version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    machineInfo.MachineData.Write("ac-axislist", axes.ToArray());
                }

                foreach (var axis in axes)
                {
                    var a = AxisManager.Get(axis);
                    a.Save(machineInfo);
                    AxisManager.AddMachineAxis(a);
                }
            }
            catch (Exception e)
            {
                Debug.Log("[ACM]: Error saving machine's controls.");
                Debug.LogException(e);
            }
        }
    }
}
