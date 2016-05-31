using System;
using System.Collections.Generic;
using AdvancedControls.Axes;
using AdvancedControls.Controls;
using UnityEngine;

namespace AdvancedControls
{
    public static class MachineData
    {

        public static void LoadData(MachineInfo machineInfo)
        {
            try
            {
                if (!machineInfo.MachineData.HasKey("ac-version")) return;
                var version = machineInfo.MachineData.ReadString("ac-version");

                var axis_names = machineInfo.MachineData.ReadStringArray("ac-axislist");
                foreach (string name in axis_names)
                {
                    InputAxis axis = null;
                    var type = machineInfo.MachineData.ReadString("ac-axis-" + name + "-type");
                    if (type == "controller")
                        axis = new ControllerAxis(name);
                    if (type == "custom")
                        axis = new CustomAxis(name);
                    if (type == "inertial" || type == "twokey" || type == "onekey")
                        axis = new InertialAxis(name);
                    if (type == "standard")
                        axis = new StandardAxis(name);
                    if (axis == null) continue;
                    axis.Load(machineInfo);
                    AxisManager.Put(name, axis);
                }

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
                            {
                                c.Load(blockInfo);
                                Debug.Log("loaded " + c.Name);
                                Debug.Log(c.Min + ", " + c.Center + ", " + c.Max);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error while loading machine's AdvancedControls:");
                Debug.LogException(e);
            }
        }

        public static void SaveData(MachineInfo machineInfo)
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
                        axes.Add(c.Axis);
                        control_names.Add(c.Name);
                        c.Save(blockInfo);
                    }
                    blockInfo.BlockData.Write("ac-controllist", control_names.ToArray());
                }
            }

            if (axes.Count == 0) return;
            foreach (string axis in axes)
            {
                AxisManager.Get(axis).Save(machineInfo);
            }
            machineInfo.MachineData.Write("ac-version", "v1.0.0");
            machineInfo.MachineData.Write("ac-axislist", axes.ToArray());
        }
    }
}
