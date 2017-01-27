using Lench.AdvancedControls.Axes;
using System;
using System.Collections.Generic;
using Lench.AdvancedControls.Resources;
using Lench.AdvancedControls.UI;
using UnityEngine;
// ReSharper disable PossibleNullReferenceException

namespace Lench.AdvancedControls
{
    internal static class Configuration
    {
        internal static void Load()
        {
            try
            {
                // load mod configuration
                Mod.ModEnabled = spaar.ModLoader.Configuration.GetBool("acm-enabled", true);
                Mod.ModUpdaterEnabled = spaar.ModLoader.Configuration.GetBool("mod-updater-enabled", true);
                Mod.DbUpdaterEnabled = spaar.ModLoader.Configuration.GetBool("db-updater-enabled", true);

                // read input axes
                int count = spaar.ModLoader.Configuration.GetInt("number-of-axes", 0);
                for (int i = 0; i < count; i++)
                {
                    string name = spaar.ModLoader.Configuration.GetString("axis-" + i + "-name", null);
                    InputAxis axis = null;
                    if (name != null)
                    {
                        var type = spaar.ModLoader.Configuration.GetString("axis-" + name + "-type", null);
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
                    }
                    if (axis != null)
                    {
                        axis.Load();
                        AxisManager.AddLocalAxis(axis);
                    }
                }

                // refresh chain axis links
                foreach (var entry in AxisManager.LocalAxes)
                {
                    if (entry.Value.Type == AxisType.Chain)
                        (entry.Value as ChainAxis).RefreshLinks();
                }
            }
            catch (Exception e)
            {
                Debug.Log("[ACM]: "+Strings.Log_AxisLoadingError);
                Debug.LogException(e);
            }
        }

        internal static void Save()
        {
            string log = "";
            try
            {
                spaar.ModLoader.Configuration.SetBool("acm-enabled", Mod.ModEnabled);
                spaar.ModLoader.Configuration.SetBool("mod-updater-enabled", Mod.ModUpdaterEnabled);
                spaar.ModLoader.Configuration.SetBool("db-updater-enabled", Mod.DbUpdaterEnabled);

                int count = spaar.ModLoader.Configuration.GetInt("number-of-axes", 0);
                log += "Attempting to clear " + count + " existing axes.\n";
                for (int i = 0; i < count; i++)
                    spaar.ModLoader.Configuration.RemoveKey("axis-" + i + "-name");

                log += "\tExisting axis list removed.\n\n";

                var axisNames = new List<string>();

                foreach (var entry in AxisManager.LocalAxes)
                {
                    log += "Attempting to save axis '"+entry.Key+ "'.\n";
                    axisNames.Add(entry.Key);
                    entry.Value.Save();
                    log += "\tSuccessfully saved axis '" + entry.Key + "'.\n";
                }

                spaar.ModLoader.Configuration.SetInt("number-of-axes", AxisManager.LocalAxes.Count);
                log += "\nWrote new number of axes: " + AxisManager.LocalAxes.Count + ".\n";
                for (int i = 0; i < axisNames.Count; i++)
                    spaar.ModLoader.Configuration.SetString("axis-" + i + "-name", axisNames[i]);
                log += "Successfully wrote axis list.\n";

                spaar.ModLoader.Configuration.Save();
                log += "Successfully saved configuration.";
            }
            catch (Exception e)
            {
                Debug.Log("[ACM]: "+Strings.Log_AxisSavingError);
                Debug.LogException(e);
                log += "\nException thrown:\n";
                log += e.Message + "\n";
                log += e.StackTrace;
            }
            System.IO.File.WriteAllText(Application.dataPath + "/Mods/Debug/ACM_Log.txt", log);
        }
    }
}
