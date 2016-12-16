using System;
using System.Reflection;
using spaar.ModLoader;
using Lench.AdvancedControls.UI;
using Lench.AdvancedControls.Input;
using Lench.AdvancedControls.Controls;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable CoVariantArrayConversion
// ReSharper disable UnusedMember.Local
// ReSharper disable DelegateSubtraction

namespace Lench.AdvancedControls
{
    /// <summary>
    /// Mod class to be loaded by spaar's mod loader.
    /// </summary>
    public class AdvancedControlsMod : Mod
    {

#pragma warning disable CS1591
        public override string Name { get; } = "AdvancedControlsMod";
        public override string DisplayName { get; } = "Advanced Controls Mod";
        public override string Author { get; } = "Lench";
        public override Version Version
        {
            get
            {
                var v =  Assembly.GetExecutingAssembly().GetName().Version;
                return new Version(v.Major, v.Minor, v.Build);
            }
        }
        
        public override string VersionExtra { get; } = "";
        public override string BesiegeVersion { get; } = "v0.4";
        public override bool CanBeUnloaded { get; } = true;
        public override bool Preload { get; } = false;
#pragma warning restore CS1591

        /// <summary>
        /// Creates main mod instance and subscribes to load and save events.
        /// </summary>
        public override void OnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(ACM.Instance);
            Game.OnSimulationToggle += SimulationToggle;
            BlockHandlerController.OnInitialisation += ACM.Instance.Initialise;
            XmlSaver.OnSave += MachineData.Save;
            XmlLoader.OnLoad += MachineData.Load;

            PythonEnvironment.LoadPythonAssembly();

            ImportPythonModules();
        }

        /// <summary>
        /// Unsubscribes from all events and destroys the mod.
        /// </summary>
        public override void OnUnload()
        {
            Game.OnSimulationToggle -= SimulationToggle;
            BlockHandlerController.OnInitialisation -= ACM.Instance.Initialise;
            XmlSaver.OnSave -= MachineData.Save;
            XmlLoader.OnLoad -= MachineData.Load;
            Configuration.Save();

            UnityEngine.Object.Destroy(ACM.Instance);
        }

        /// <summary>
        /// Checks if LenchScripterMod is present and adds initialisation statements
        /// that import AdvancedControls module.
        /// </summary>
        private static void ImportPythonModules()
        {
            try
            {
                var assembly = Assembly.LoadFrom(Application.dataPath + "/Mods/LenchScripterMod.dll");
                var type = assembly.GetType("Lench.Scripter.PythonEnvironment");
                var method = type.GetMethod("AddInitStatement", BindingFlags.Public | BindingFlags.Static);
                method.Invoke(null, new[] { "clr.AddReference(\"AdvancedControlsMod\")\n" +
                                            "from Lench.AdvancedControls import AdvancedControls\n" +
                                            "from Lench.AdvancedControls.Axes import AxisType\n" +
                                            "from Lench.AdvancedControls.Axes.ChainAxis import ChainMethod" });
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Destroys old block handlers on every simulation toggle,
        /// so they can be created again after block array is populated.
        /// </summary>
        private static void SimulationToggle(bool simulating)
        {
            if (simulating) Functions.ResetTimer();
            BlockHandlerController.DestroyBlockHandlers();
        }
    }

    /// <summary>
    /// Main mod script and entry point.
    /// Access the instance with `ACM.Instance`
    /// </summary>
    public class ACM : SingleInstance<ACM>
    {
        /// <summary>
        /// Name of the instance.
        /// </summary>
        public override string Name => "Advanced Controls";

        /// <summary>
        /// Is mod enabled in the settings menu.
        /// </summary>
        public bool ModEnabled = true;

        /// <summary>
        /// Is automatic game controller database updater enabled.
        /// Changed with `acm dbupdate enable/disable` command.
        /// </summary>
        public bool DbUpdaterEnabled;

        /// <summary>
        /// Is automatic mod update checker enabled.
        /// Changed with `acm modupdate enable/disable` command.
        /// </summary>
        public bool ModUpdaterEnabled;

        internal bool LoadedMachine;

        internal delegate void UpdateEventHandler();
        internal event UpdateEventHandler OnUpdate;

        internal delegate void InitialiseEventHandler();
        internal event InitialiseEventHandler OnInitialisation;

        private Guid _copySource;

        private void Awake()
        {
            gameObject.AddComponent<BlockHandlerController>();
            gameObject.AddComponent<ControlMapper>();
            gameObject.AddComponent<DeviceManager>();

            Commands.RegisterCommand("controller", ControllerCommand, Strings.Console_Controller_AllAvailable);
            Commands.RegisterCommand("acm", ConfigurationCommand, Strings.Console_Acm_AllAvailable);
            SettingsMenu.RegisterSettingsButton("ACM", EnableToggle, ModEnabled, 14);
        }

        private void Start()
        {
            Configuration.Load();

            if (ModUpdaterEnabled)
                CheckForModUpdate();

            if (DbUpdaterEnabled)
                CheckForDbUpdate();

            enabled = ModEnabled;

            DeviceManager.OnDeviceAdded += (e) => { Axes.AxisManager.ResolveMachineAxes(); };
        }

        private void OnDestroy()
        {
            OnUpdate = null;
            OnInitialisation = null;
            Destroy(BlockHandlerController.Instance);
            Destroy(ControlMapper.Instance);
            Destroy(DeviceManager.Instance);
            Destroy(GameObject.Find("Advanced Controls").transform.gameObject);
        }

        private void Update()
        {
            // Initialize block handlers
            if (Game.IsSimulating && !BlockHandlerController.Initialised)
                BlockHandlerController.InitializeBlockHandlers();

            // Open or hide ACM mapper
            if (BlockMapper.CurrentInstance != null)
            {
                if (BlockMapper.CurrentInstance.Block != null && BlockMapper.CurrentInstance.Block != ControlMapper.Instance.Block)
                    ControlMapper.Instance.ShowBlockControls(BlockMapper.CurrentInstance.Block);

                if (BlockMapper.CurrentInstance.Block != null)
                {
                    if (InputManager.CopyKeys())
                        _copySource = BlockMapper.CurrentInstance.Block.Guid;
                    if (InputManager.PasteKeys())
                        ControlManager.CopyBlockControls(_copySource, BlockMapper.CurrentInstance.Block.Guid);
                }
            }
            else
            {
                if (ControlMapper.Instance.Visible)
                    ControlMapper.Instance.Hide();
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

        private void EnableToggle(bool active)
        {
            ModEnabled = active;
            enabled = active;
            if (!active) ControlMapper.Instance.Hide();
        }

        private string ControllerCommand(string[] args, IDictionary<string, string> namedArgs)
        {
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "list":
                        string result = Strings.Console_Controller_ControllerList;
                        if (Controller.NumDevices > 0)
                            for (int i = 0; i < Controller.NumDevices; i++)
                            {
                                var controller = Controller.Get(i);
                                result += $"{i}: {controller.Name} ({(controller.IsGameController ? Strings.Console_Controller_ControllerTag : Strings.Console_Controller_JoystickTag)})\n\t"+
                                    Strings.Controller_GUID + " " + controller.GUID + "\n";
                            }
                        else
                            result = Strings.Console_Controller_NoDevicesConnected;
                        return result;

                    default:
                        return Strings.Console_Controller_InvalidCommand;
                }
            }
            else
            {
                return Strings.Console_Controller_AllAvailableList;
            }
        }

        private string ConfigurationCommand(string[] args, IDictionary<string, string> namedArgs)
        {
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "modupdate":
                        if (args.Length > 1)
                        {
                            switch (args[1].ToLower())
                            {
                                case "check":
                                    CheckForModUpdate(true);
                                    return Strings.Console_Acm_CheckingForModUpdates;
                                case "enable":
                                    ModUpdaterEnabled = true;
                                    return Strings.Console_Acm_ModUpdateCheckerEnabled;
                                case "disable":
                                    ModUpdaterEnabled = false;
                                    return Strings.Console_Acm_ModUpdateCheckerDisabled;
                                default:
                                    return Strings.Console_Acm_UpdateInvalidArgument;
                            }
                        }
                        else
                        {
                            return Strings.Console_Acm_UpdateMissingArgument;
                        }
                    case "dbupdate":
                        if (args.Length > 1)
                        {
                            switch (args[1].ToLower())
                            {
                                case "check":
                                    CheckForDbUpdate(true);
                                    return Strings.Console_Acm_CheckingForControllerDBUpdates;
                                case "enable":
                                    DbUpdaterEnabled = true;
                                    return Strings.Console_Acm_ControllerDBUpdateCheckerEnabled;
                                case "disable":
                                    DbUpdaterEnabled = false;
                                    return Strings.Console_Acm_ControllerDBUpdateCheckerDisabled;
                                default:
                                    return Strings.Console_Acm_UpdateInvalidArgument;
                            }
                        }
                        else
                        {
                            return Strings.Console_Acm_UpdateMissingArgument;
                        }
                    default:
                        return Strings.Console_Acm_InvalidCommand;
                }
            }
            else
            {
                return Strings.Console_Acm_AllAvailableList;
            }
        }

        private void CheckForModUpdate(bool verbose = false)
        {
            var updater = gameObject.AddComponent<Updater>();
            updater.Check(
                Strings.Updater_WindowTitle,
                "https://api.github.com/repos/lench4991/AdvancedControlsMod/releases/latest",
                Assembly.GetExecutingAssembly().GetName().Version,
                new List<Updater.Link>()
                    {
                            new Updater.Link() { DisplayName = Strings.Updater_SpiderlingForumLink, URL = "http://forum.spiderlinggames.co.uk/index.php?threads/3150/" },
                            new Updater.Link() { DisplayName = Strings.Updater_GithubReleasePageLink, URL = "https://github.com/lench4991/AdvancedControlsMod/releases/latest" }
                    },
                verbose);
        }

        private void CheckForDbUpdate(bool verbose = false)
        {
            StartCoroutine(DeviceManager.AssignMappings(true, verbose));
        }
    }
}
