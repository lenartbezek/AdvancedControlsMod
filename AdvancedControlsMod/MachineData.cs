using System;
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
                    Axes.Axis axis = null;
                    var type = machineInfo.MachineData.ReadString("AC-Axis-" + name + "-Type");
                    if (type == "Controller")
                        axis = new ControllerAxis() { Name = name };
                    if (type == "Custom")
                        axis = new CustomAxis() { Name = name };
                    if (type == "OneKey")
                        axis = new OneKeyAxis() { Name = name };
                    if (type == "TwoKey")
                        axis = new TwoKeyAxis() { Name = name };
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
                            if (name == c.Name || c as ControlGroup != null)
                                c.Load(blockInfo);
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
            machineInfo.MachineData.EraseCustomData();
            machineInfo.MachineData.Write("AdvancedControls-Version", "v1.0.0");

            var axes = AxisManager.GetActiveAxes(ControlManager.GetActiveControls());
            var axis_names = new string[axes.Count];
            int i = 0;
            foreach (Axes.Axis axis in axes)
            {
                axis_names[i] = axis.Name; i++;
                axis.Save(machineInfo);
            }
            machineInfo.MachineData.Write("AC-AxisList", axis_names);

            foreach (BlockInfo blockInfo in machineInfo.Blocks)
            {
                if (ControlManager.Blocks.ContainsKey(blockInfo.Guid))
                {
                    var controls = ControlManager.GetActiveBlockControls(blockInfo.Guid);
                    var control_names = new string[controls.Count];
                    int j = 0;
                    foreach (Control c in controls)
                    {
                        control_names[j] = c.Name; j++;
                        c.Save(blockInfo);
                    }
                    blockInfo.BlockData.Write("AC-ControlList", control_names);
                }
            }
        }
    }
}
