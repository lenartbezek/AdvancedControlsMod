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
                if (!machineInfo.MachineData.HasKey("AdvancedControls-Version")) return;
                var version = machineInfo.MachineData.ReadString("AdvancedControls-Version");

                var axis_names = machineInfo.MachineData.ReadStringArray("AC-AxisList");
                foreach (string name in axis_names)
                {
                    InputAxis axis = null;
                    var type = machineInfo.MachineData.ReadString("AC-Axis-" + name + "-Type");
                    if (type == "Controller")
                        axis = new ControllerAxis(name);
                    if (type == "Custom")
                        axis = new CustomAxis(name);
                    if (type == "OneKey")
                        axis = new OneKeyAxis(name);
                    if (type == "TwoKey")
                        axis = new TwoKeyAxis(name);
                    if (axis == null) continue;
                    axis.Load(machineInfo);
                    AxisManager.Put(name, axis);
                }

                foreach (BlockInfo blockInfo in machineInfo.Blocks)
                {
                    if (!blockInfo.BlockData.HasKey("AC-ControlList")) continue;
                    var control_list = ControlManager.GetBlockControls(blockInfo.ID, blockInfo.Guid);
                    var control_names = blockInfo.BlockData.ReadStringArray("AC-ControlList");
                    foreach (string name in control_names)
                    {
                        foreach (Control c in control_list)
                        {
                            if (name == c.Name)
                                c.Load(blockInfo);
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
            var all_controls = new List<Control>();

            foreach (BlockInfo blockInfo in machineInfo.Blocks)
            {
                if (ControlManager.Blocks.ContainsKey(blockInfo.Guid))
                {
                    var controls = ControlManager.GetActiveBlockControls(blockInfo.Guid);
                    if (controls.Count == 0) continue;
                    var control_names = new List<string>();
                    foreach (Control c in controls)
                    {
                        Debug.Log("Saving control: " + c.Name);
                        all_controls.Add(c);
                        control_names.Add(c.Name);
                        c.Save(blockInfo);
                    }
                    blockInfo.BlockData.Write("AC-ControlList", control_names.ToArray());
                }
            }

            machineInfo.MachineData.Write("AdvancedControls-Version", "v1.0.0");

            var axes = AxisManager.GetActiveAxes(all_controls);
            if (axes.Count == 0) return;
            var axis_names = new List<string>();
            foreach (InputAxis axis in axes)
            {
                Debug.Log("Saving axis: " + axis.Name);
                axis_names.Add(axis.Name);
                axis.Save(machineInfo);
            }
            machineInfo.MachineData.Write("AC-AxisList", axis_names.ToArray());
        }
    }
}
