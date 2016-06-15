using System;
using spaar.ModLoader;
using LenchScripter;
using AdvancedControls.UI;
using AdvancedControls.Input;
using AdvancedControls.Controls;

namespace AdvancedControls
{

    public class AdvancedControlsMod : Mod
    {
        public override string Name { get; } = "Advanced Controls Mod";
        public override string DisplayName { get; } = "Advanced Controls Mod";
        public override string Author { get; } = "Lench";
        public override Version Version { get; } = new Version(1, 2, 0, 0);
        
        public override string VersionExtra { get; } = "";
        public override string BesiegeVersion { get; } = "v0.3";
        public override bool CanBeUnloaded { get; } = true;
        public override bool Preload { get; } = false;

        public override void OnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(ACM.Instance);
            BlockHandlers.OnInitialisation += ACM.Instance.Initialise;
            Game.OnSimulationToggle += ACM.Instance.OnSimulationToggle;
            XmlSaver.OnSave += MachineData.Save;
            XmlLoader.OnLoad += MachineData.Load;
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

        internal ControlMapperWindow ControlMapper;
        internal EventManager EventManager;

        public delegate void UpdateEventHandler();
        public event UpdateEventHandler OnUpdate;

        public delegate void InitialiseEventHandler();
        public event InitialiseEventHandler OnInitialisation;

        private BlockMapper blockMapper;
        private Guid copy_source;

        private void Start()
        {
            ControlMapper = gameObject.AddComponent<ControlMapperWindow>();
            EventManager = gameObject.AddComponent<EventManager>();

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
                if (hoveredBlock != null && hoveredBlock != ControlMapper.Block)
                    ControlMapper.ShowBlockControls(hoveredBlock);

                if (hoveredBlock != null && UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftControl) ||
                                            UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftCommand))
                {
                    if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.C))
                        copy_source = hoveredBlock.Guid;
                    if (copy_source != null && UnityEngine.Input.GetKey(UnityEngine.KeyCode.V))
                        ControlManager.CopyBlockControls(copy_source, hoveredBlock.Guid);
                }
            }
            else
            {
                if (ControlMapper.Visible)
                    ControlMapper.Hide();
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
