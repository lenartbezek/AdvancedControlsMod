using System;
using spaar.ModLoader;
using UnityEngine;
using LenchScripter;
using LenchScripter.Blocks;

using AdvancedControls.UI;

namespace AdvancedControls
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

        internal static ControllerAxisEdit ControllerAxisEdit;
        internal static OneKeyAxisEdit OneKeyAxisEdit;
        internal static TwoKeyAxisEdit TwoKeyAxisEdit;
        internal static CustomAxisEdit CustomAxisEdit;
        internal static AxisList AxisList;

        public override void OnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(ADVControls.Instance);
            Game.OnSimulationToggle += ADVControls.Instance.OnSimulationToggle;

            ControllerAxisEdit = ADVControls.Instance.gameObject.AddComponent<ControllerAxisEdit>();
            OneKeyAxisEdit = ADVControls.Instance.gameObject.AddComponent<OneKeyAxisEdit>();
            TwoKeyAxisEdit = ADVControls.Instance.gameObject.AddComponent<TwoKeyAxisEdit>();
            CustomAxisEdit = ADVControls.Instance.gameObject.AddComponent<CustomAxisEdit>();
            AxisList = ADVControls.Instance.gameObject.AddComponent<AxisList>();
        }

        public override void OnUnload()
        {
            Game.OnSimulationToggle -= ADVControls.Instance.OnSimulationToggle;
            ADVControls.Instance.OnSimulationToggle(false);
            UnityEngine.Object.Destroy(ADVControls.Instance);
        }
    }

    public class ADVControls : SingleInstance<ADVControls>
    {
        public override string Name { get { return "Advanced Controls"; } }

        private bool isSimulating;

        public delegate void UpdateEventHandler();
        public event UpdateEventHandler OnUpdate;

        public delegate void ResetEventHandler();
        public event ResetEventHandler OnReset;

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
