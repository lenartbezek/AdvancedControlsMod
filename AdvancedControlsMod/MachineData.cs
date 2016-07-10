using System;
using System.Collections.Generic;
using AdvancedControls.Controls;
using UnityEngine;

namespace AdvancedControls
{
    internal static class MachineData
    {
        internal static void Load(MachineInfo machineInfo)
        {
            try
            {
                if (!machineInfo.MachineData.HasKey("ac-version")) return;
                var version = machineInfo.MachineData.ReadString("ac-version");

                if (!machineInfo.MachineData.HasKey("ac-axislist")) return;
                var axis_names = machineInfo.MachineData.ReadStringArray("ac-axislist");

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
                            if (!axes.Contains(c.Axis)) axes.Add(c.Axis);
                            control_names.Add(c.Name);
                            c.Save(blockInfo);
                        }
                        blockInfo.BlockData.Write("ac-controllist", control_names.ToArray());
                    }
                }

                if (axes.Count != 0)
                {
                    machineInfo.MachineData.Write("ac-version", "v1.2.4");
                    machineInfo.MachineData.Write("ac-axislist", axes.ToArray());
                }
            }
            catch (Exception e)
            {
                Debug.Log("[ACM]: Error saving machine's controls:");
                Debug.LogException(e);
            }
        }
    }
}
