using System;
using spaar.ModLoader;
using LenchScripter;
using AdvancedControls.UI;
using UnityEngine;

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

        internal static ControlMapperWindow ControlMapper;
        internal static AxisEditorWindow AxisEditor;

        public override void OnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(ADVControls.Instance);
            BlockHandlers.OnInitialisation += ADVControls.Instance.Initialise;
            Game.OnSimulationToggle += ADVControls.Instance.OnSimulationToggle;
            XmlSaver.OnSave += MachineData.SaveData;
            XmlLoader.OnLoad += MachineData.LoadData;

            AxisEditor = ADVControls.Instance.gameObject.AddComponent<AxisEditorWindow>();
            ControlMapper = ADVControls.Instance.gameObject.AddComponent<ControlMapperWindow>();
        }

        public override void OnUnload()
        {
            BlockHandlers.OnInitialisation -= ADVControls.Instance.Initialise;
            Game.OnSimulationToggle -= ADVControls.Instance.OnSimulationToggle;
            XmlSaver.OnSave -= MachineData.SaveData;
            XmlLoader.OnLoad -= MachineData.LoadData;

            UnityEngine.Object.Destroy(ADVControls.Instance);
        }
    }

    public class ADVControls : SingleInstance<ADVControls>
    {
        public override string Name { get { return "Advanced Controls"; } }

        public bool IsSimulating { get { return isSimulating; } }
        private bool isSimulating = false;

        public delegate void UpdateEventHandler();
        public event UpdateEventHandler OnUpdate;

        public delegate void InitialiseEventHandler();
        public event InitialiseEventHandler OnInitialisation;

        private BlockMapper blockMapper;

        internal void Update()
        {
            if (blockMapper == null)
                blockMapper = FindObjectOfType<BlockMapper>();
            if (blockMapper != null)
            {
                var hoveredBlock = blockMapper.Block.GetComponent<GenericBlock>();
                if (hoveredBlock != null && hoveredBlock != AdvancedControlsMod.ControlMapper.Block)
                    AdvancedControlsMod.ControlMapper.ShowBlockControls(hoveredBlock);
            }
            else
            {
                if (AdvancedControlsMod.ControlMapper.Visible)
                    AdvancedControlsMod.ControlMapper.Hide();
            }

            OnUpdate?.Invoke();
        }

        internal void Initialise()
        {
            OnInitialisation?.Invoke();
        }

        internal void OnSimulationToggle(bool s)
        {
            isSimulating = s;
        }
    }
}
