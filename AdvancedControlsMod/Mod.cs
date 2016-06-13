using System;
using spaar.ModLoader;
using LenchScripter;
using AdvancedControls.UI;
using AdvancedControls.Input;

namespace AdvancedControls
{

    public class AdvancedControlsMod : Mod
    {
        public override string Name { get; } = "Advanced Controls Mod";
        public override string DisplayName { get; } = "Advanced Controls Mod";
        public override string Author { get; } = "Lench";
        public override Version Version { get; } = new Version(1, 2, 0);
        
        public override string VersionExtra { get; } = "alpha";
        public override string BesiegeVersion { get; } = "v0.3";
        public override bool CanBeUnloaded { get; } = true;
        public override bool Preload { get; } = false;

        internal static ControlMapperWindow ControlMapper;
        internal static EventManager EventManager;

        public override void OnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(ACM.Instance);
            BlockHandlers.OnInitialisation += ACM.Instance.Initialise;
            Game.OnSimulationToggle += ACM.Instance.OnSimulationToggle;
            XmlSaver.OnSave += MachineData.Save;
            XmlLoader.OnLoad += MachineData.Load;

            ControlMapper = ACM.Instance.gameObject.AddComponent<ControlMapperWindow>();
            EventManager = ACM.Instance.gameObject.AddComponent<EventManager>();
        }

        public override void OnUnload()
        {
            BlockHandlers.OnInitialisation -= ACM.Instance.Initialise;
            Game.OnSimulationToggle -= ACM.Instance.OnSimulationToggle;
            XmlSaver.OnSave -= MachineData.Save;
            XmlLoader.OnLoad -= MachineData.Load;
            Configuration.Save();

            UnityEngine.Object.Destroy(ACM.Instance);
        }
    }

    public class ACM : SingleInstance<ACM>
    {
        public override string Name { get { return "Advanced Controls"; } }

        internal bool LoadedMachine = false;
        public bool IsSimulating { get { return isSimulating; } }
        private bool isSimulating = false;

        public delegate void UpdateEventHandler();
        public event UpdateEventHandler OnUpdate;

        public delegate void InitialiseEventHandler();
        public event InitialiseEventHandler OnInitialisation;

        private BlockMapper blockMapper;

        private void Start()
        {
            Configuration.Load();
        }

        private void Update()
        {

            if (blockMapper == null)
            {
                blockMapper = FindObjectOfType<BlockMapper>();
            }

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

            if (LoadedMachine)
            {
                foreach (AssignAxesWindow g in FindObjectsOfType<AssignAxesWindow>())
                    Destroy(g);

                LoadedMachine = false;
                AssignAxesWindow.Open();
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
