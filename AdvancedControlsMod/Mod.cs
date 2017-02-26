using System;
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
        
        public override void OnLoad()
        {

        }

        public override void OnUnload()
        {

        }
        
#pragma warning disable CS1591
        public override string Name { get; } = "AdvancedControlsMod";
        public override string DisplayName { get; } = "Advanced Controls Mod";
        public override string Author { get; } = "Lench";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        public override string VersionExtra { get; } = "";
        public override string BesiegeVersion { get; } = "v0.42";
        public override bool CanBeUnloaded { get; } = true;
        public override bool Preload { get; } = false;
#pragma warning restore CS1591

        public static event Action OnUpdate;

        public static GameObject Controller;
    }
}