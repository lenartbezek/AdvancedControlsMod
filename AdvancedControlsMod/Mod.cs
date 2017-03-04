using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using spaar.ModLoader;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable UnusedMember.Local

namespace Lench.AdvancedControls
{
    /// <summary>
    ///     Mod class to be loaded by spaar's mod loader.
    /// </summary>
    public class Mod : spaar.ModLoader.Mod
    {

#pragma warning disable CS1591

        public override void OnLoad()
        {
            Controller = new GameObject("AdvancedControls");
            Object.DontDestroyOnLoad(Controller);

            Game.OnSimulationToggle += Block.HandleSimulationToggle;

            spaar.Commands.RegisterCommand("py", PythonCommand);
        }

        public override void OnUnload()
        {
            Object.Destroy(Controller);

            Game.OnSimulationToggle -= Block.HandleSimulationToggle;
        }

        public override string Name { get; } = "AdvancedControlsMod";
        public override string DisplayName { get; } = "Advanced Controls Mod";
        public override string Author { get; } = "Lench";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        public override string VersionExtra { get; } = "";
        public override string BesiegeVersion { get; } = "v0.42";
        public override bool CanBeUnloaded { get; } = true;
        public override bool Preload { get; } = false;

#pragma warning restore CS1591

        /// <summary>
        ///     Mod main game object, where all other components attach to.
        /// </summary>
        public static GameObject Controller;

        /// <summary>
        ///     Called on python console command.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="namedArgs"></param>
        /// <returns></returns>
        public static string PythonCommand(string[] args, IDictionary<string, string> namedArgs)
        {
            if (args.Length == 0)
                return "Executes a Python expression.";

            var expression = args.Aggregate("", (current, t) => current + (t + " "));

            try
            {
                var result = Script.Global.Execute(expression);
                return result?.ToString() ?? "";
            }
            catch (Exception e)
            {
                if (e.InnerException != null) e = e.InnerException;
                Debug.Log("<b><color=#FF0000>Python error: " + e.Message + "</color></b>\n" +
                          Script.FormatException(e));
                return "";
            }
        }
    }
}