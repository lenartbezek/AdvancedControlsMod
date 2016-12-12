using System;
using System.Collections.Generic;
using Lench.AdvancedControls.Controls;
using Lench.AdvancedControls.Axes;
using UnityEngine;
using System.Reflection;
// ReSharper disable PossibleNullReferenceException

namespace Lench.AdvancedControls
{
    internal static class MachineData
    {
        internal static void Load(MachineInfo machineInfo)
        {
            try
            {
                AxisManager.MachineAxes.Clear();
                ControlManager.Blocks.Clear();

                // read mod version
                if (!machineInfo.MachineData.HasKey("ac-version")) return;
                var version = new Version(machineInfo.MachineData.ReadString("ac-version").TrimStart('v'));

                // version alert
                if (version < Assembly.GetExecutingAssembly().GetName().Version)
                    Debug.Log("[ACM]: " + machineInfo.Name + " was saved with mod version " + version + ".\n\tIt may not be compatible with some newer features.");

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
                    if (type == AxisType.Standard.ToString() || // backwards compatibility
                        type == AxisType.Inertial.ToString() || // backwards compatibility
                        type == AxisType.Key.ToString())
                        axis = new KeyAxis(name);
                    if (type == AxisType.Mouse.ToString())
                        axis = new MouseAxis(name);

                    if (axis != null)
                    {
                        axis.Load(machineInfo);
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
                    var controlList = ControlManager.GetBlockControls(blockInfo.ID, blockInfo.Guid);
                    var controlNames = blockInfo.BlockData.ReadStringArray("ac-controllist");
                    foreach (string name in controlNames)
                    {
                        foreach (Control c in controlList)
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
                var axisStack = new Stack<string>();
                var axisList = new List<string>();

                // save all controls and collect all bound axes
                foreach (BlockInfo blockInfo in machineInfo.Blocks)
                {
                    if (ControlManager.Blocks.ContainsKey(blockInfo.Guid))
                    {
                        var controls = ControlManager.GetActiveBlockControls(blockInfo.Guid);
                        if (controls.Count == 0) continue;
                        var controlNames = new List<string>();
                        foreach (Control c in controls)
                        {
                            if (!axisStack.Contains(c.Axis))
                                axisStack.Push(c.Axis);
                            controlNames.Add(c.Name);
                            c.Save(blockInfo);
                        }
                        blockInfo.BlockData.Write("ac-controllist", controlNames.ToArray());
                    }
                }

                // go through stack and save all axes and chained axes
                while (axisStack.Count > 0)
                {
                    var a = AxisManager.Get(axisStack.Pop());
                    if (a == null || axisList.Contains(a.Name)) continue;
                    axisList.Add(a.Name);
                    if (a.Type == AxisType.Chain)
                    {
                        var chain = a as ChainAxis;
                        axisStack.Push(chain.SubAxis1);
                        axisStack.Push(chain.SubAxis2);
                    }
                    if (a.Type == AxisType.Custom)
                    {
                        var custom = a as CustomAxis;
                        foreach (var linked in custom.LinkedAxes)
                            axisStack.Push(linked);
                    }
                    a.Save(machineInfo);
                }

                // save axis list and metadata
                if (axisList.Count != 0)
                {
                    machineInfo.MachineData.Write("ac-version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    machineInfo.MachineData.Write("ac-axislist", axisList.ToArray());
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
