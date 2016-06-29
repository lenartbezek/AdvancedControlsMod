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
        public override Version Version { get; } = new Version(1, 2, 3);
        
        public override string VersionExtra { get; } = "";
        public override string BesiegeVersion { get; } = "v0.3";
        public override bool CanBeUnloaded { get; } = true;
        public override bool Preload { get; } = false;

        public override void OnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(ACM.Instance);
            BlockHandlers.OnInitialisation += ACM.Instance.Initialise;
            XmlSaver.OnSave += MachineData.Save;
            XmlLoader.OnLoad += MachineData.Load;
        }

        public override void OnUnload()
        {
            BlockHandlers.OnInitialisation -= ACM.Instance.Initialise;
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

        internal ControlMapperWindow ControlMapper;
        internal EventManager EventManager;

        internal delegate void UpdateEventHandler();
        internal event UpdateEventHandler OnUpdate;

        internal delegate void InitialiseEventHandler();
        internal event InitialiseEventHandler OnInitialisation;

        private BlockMapper blockMapper;
        private Guid copy_source;

        private void Start()
        {
            ControlMapper = gameObject.AddComponent<ControlMapperWindow>();
            EventManager = gameObject.AddComponent<EventManager>();

            if (PythonEnvironment.Loaded)
            {
                PythonEnvironment.AddInitStatement("clr.AddReference(\"AdvancedControlsMod\")");
                PythonEnvironment.AddInitStatement("from AdvancedControls import AdvancedControls");
                PythonEnvironment.AddInitStatement("from AdvancedControls.Axes import AxisType");
                PythonEnvironment.AddInitStatement("from AdvancedControls.Axes.ChainAxis import ChainMethod");
            }

            Configuration.Load();
        }

        private void OnDestroy()
        {
            OnUpdate = null;
            OnInitialisation = null;
            Destroy(ControlMapper);
            Destroy(EventManager);
            Destroy(UnityEngine.GameObject.Find("Advanced Controls").transform.gameObject);
        }

        private void Update()
        {
            if (blockMapper == null)
            {
                blockMapper = BlockMapper.CurrentInstance;
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
                LoadedMachine = false;
                AssignAxesWindow.Open(true);
            }

            OnUpdate?.Invoke();
        }

        internal void Initialise()
        {
            OnInitialisation?.Invoke();
        }
    }
}
