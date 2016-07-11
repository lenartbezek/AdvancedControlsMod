using System;
using System.Reflection;
using spaar.ModLoader;
using Lench.Scripter;
using Lench.AdvancedControls.UI;
using Lench.AdvancedControls.Input;
using Lench.AdvancedControls.Controls;

namespace Lench.AdvancedControls
{
    public class AdvancedControlsMod : Mod
    {
        public override string Name { get; } = "AdvancedControlsMod";
        public override string DisplayName { get; } = "Advanced Controls Mod";
        public override string Author { get; } = "Lench";
        public override Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }
        
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

        internal ControlMapper ControlMapper;
        internal EventManager EventManager;
        internal Updater UpdateChecker;

        internal delegate void UpdateEventHandler();
        internal event UpdateEventHandler OnUpdate;

        internal delegate void InitialiseEventHandler();
        internal event InitialiseEventHandler OnInitialisation;

        private Guid copy_source;

        private void Start()
        {
            ControlMapper = gameObject.AddComponent<ControlMapper>();
            EventManager = gameObject.AddComponent<EventManager>();
            UpdateChecker = gameObject.AddComponent<ACMUpdater>();

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
            if (BlockMapper.CurrentInstance != null)
            {
                if (BlockMapper.CurrentInstance.Block != null && BlockMapper.CurrentInstance.Block != ControlMapper.Block)
                    ControlMapper.ShowBlockControls(BlockMapper.CurrentInstance.Block);

                if (BlockMapper.CurrentInstance.Block != null &&
                    UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftControl) ||
                    UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftCommand))
                {
                    if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.C))
                        copy_source = BlockMapper.CurrentInstance.Block.Guid;
                    if (copy_source != null && UnityEngine.Input.GetKey(UnityEngine.KeyCode.V))
                        ControlManager.CopyBlockControls(copy_source, BlockMapper.CurrentInstance.Block.Guid);
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
                ControlOverview.Open(true);
            }

            OnUpdate?.Invoke();
        }

        internal void Initialise()
        {
            OnInitialisation?.Invoke();
        }
    }
}
