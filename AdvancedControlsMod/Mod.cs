using System;
using spaar.ModLoader;
using UnityEngine;
using LenchScripter;
using LenchScripter.Blocks;

namespace AdvancedControlsMod
{

    public class AdvancedControlsMod : Mod
    {
        public override string Name { get; } = "Advanced Controls Mod";
        public override string DisplayName { get; } = "Advanced Controls Mod";
        public override string Author { get; } = "Lench";
        public override Version Version { get; } = new Version(0, 1, 0);
        
        public override string VersionExtra { get; } = "alpha";
        public override string BesiegeVersion { get; } = "v0.27";
        public override bool CanBeUnloaded { get; } = true;
        public override bool Preload { get; } = false;

        public override void OnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(AdvancedControls.Instance);
            Game.OnSimulationToggle += AdvancedControls.Instance.OnSimulationToggle;
            AdvancedControls.Instance.gameObject.AddComponent<ControllerAxisEdit>();
        }

        public override void OnUnload()
        {
            Game.OnSimulationToggle -= AdvancedControls.Instance.OnSimulationToggle;
            AdvancedControls.Instance.OnSimulationToggle(false);
            UnityEngine.Object.Destroy(AdvancedControls.Instance);
        }
    }

    public class AdvancedControls : SingleInstance<AdvancedControls>
    {
        public override string Name { get { return "Advanced Controls"; } }

        private bool isSimulating;

        public delegate void UpdateEventHandler();
        public event UpdateEventHandler OnUpdate;

        public delegate void ResetEventHandler();
        public event ResetEventHandler OnReset;
        
        public void SaveAxis(Axis axis, string name)
        {

        } 

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void OnSimulationStart()
        {
            OnReset?.Invoke();
        }

        private void OnSimulationStop()
        {
            
        }
        
        internal void OnSimulationToggle(bool isSimulating)
        {
            this.isSimulating = isSimulating;
            if (isSimulating)
                OnSimulationStart();
            else
                OnSimulationStop();
        }
    }
}
