using System;
using spaar.ModLoader;
using UnityEngine;
using LenchScripter;

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
        internal static ControlMapper ControlMapper;

        public override void OnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(ADVControls.Instance);
            Game.OnSimulationToggle += ADVControls.Instance.OnSimulationToggle;
            BlockHandlers.OnInitialisation += ADVControls.Instance.Initialise;

            ControllerAxisEdit = ADVControls.Instance.gameObject.AddComponent<ControllerAxisEdit>();
            OneKeyAxisEdit = ADVControls.Instance.gameObject.AddComponent<OneKeyAxisEdit>();
            TwoKeyAxisEdit = ADVControls.Instance.gameObject.AddComponent<TwoKeyAxisEdit>();
            CustomAxisEdit = ADVControls.Instance.gameObject.AddComponent<CustomAxisEdit>();
            AxisList = ADVControls.Instance.gameObject.AddComponent<AxisList>();
            ControlMapper = ADVControls.Instance.gameObject.AddComponent<ControlMapper>();

            Keybindings.AddKeybinding("Control Mapper", new Key(KeyCode.LeftShift, KeyCode.M));
        }

        public override void OnUnload()
        {
            Game.OnSimulationToggle -= ADVControls.Instance.OnSimulationToggle;
            BlockHandlers.OnInitialisation -= ADVControls.Instance.Initialise;

            ADVControls.Instance.OnSimulationToggle(false);
            UnityEngine.Object.Destroy(ADVControls.Instance);
        }
    }

    public class ADVControls : SingleInstance<ADVControls>
    {
        public override string Name { get { return "Advanced Controls"; } }

        private bool isSimulating;
        public bool IsSimulating { get { return isSimulating; } }

        public delegate void UpdateEventHandler();
        public event UpdateEventHandler OnUpdate;

        public delegate void InitialiseEventHandler();
        public event InitialiseEventHandler OnInitialisation;

        private void Update()
        {

            if (Keybindings.Get("Control Mapper").IsDown())
            {
                var hoveredBlock = Game.AddPiece.HoveredBlock;
                if (hoveredBlock != null) GetComponent<ControlMapper>().ShowBlockControls(hoveredBlock);
            }

            OnUpdate?.Invoke();
        }

        internal void Initialise()
        {
            OnInitialisation?.Invoke();
        }

        private void OnSimulationStart()
        {
            
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
