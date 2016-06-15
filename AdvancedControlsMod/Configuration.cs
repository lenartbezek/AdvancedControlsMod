using AdvancedControls.Axes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedControls
{
    public static class Configuration
    {
        public static void Load()
        {
            try
            {
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
                        if (type == AxisType.Inertial.ToString())
                            axis = new InertialAxis(name);
                        if (type == AxisType.Standard.ToString())
                            axis = new StandardAxis(name);
                        axis?.Load();
                    }
                    if (axis != null)
                    {
                        AxisManager.Put(name, axis);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("[ACM]: Error loading saved axes:");
                Debug.LogException(e);
            }
        }

        public static void Save()
        {
            string log = "";
            try
            {
                int count = spaar.ModLoader.Configuration.GetInt("number-of-axes", 0);
                log += "Attempting to clear " + count + " existing axes.\n";
                for (int i = 0; i < count; i++)
                    spaar.ModLoader.Configuration.RemoveKey("axis-" + i + "-name");

                log += "\tExisting axis list removed.\n\n";

                List<string> axis_names = new List<string>();

                foreach (KeyValuePair<string, InputAxis> entry in AxisManager.Axes)
                {
                    log += "Attempting to save axis '"+entry.Key+ "'.\n";
                    axis_names.Add(entry.Key);
                    entry.Value.Save();
                    log += "\tSuccessfully saved axis '" + entry.Key + "'.\n";
                }

                spaar.ModLoader.Configuration.SetInt("number-of-axes", AxisManager.Axes.Count);
                log += "\nWrote new number of axes: " + AxisManager.Axes.Count + ".\n";
                for (int i = 0; i < axis_names.Count; i++)
                    spaar.ModLoader.Configuration.SetString("axis-" + i + "-name", axis_names[i]);
                log += "Successfully wrote axis list.\n";

                spaar.ModLoader.Configuration.Save();
                log += "Successfully saved configuration.";
            }
            catch (Exception e)
            {
                Debug.Log("[ACM]: Error saving axes:");
                Debug.LogException(e);
                log += "\nException thrown:\n";
                log += e.Message + "\n";
                log += e.StackTrace;
            }
            System.IO.File.WriteAllText(Application.dataPath + "/Mods/Debug/ACM_Log.txt", log);
        }
    }
}
