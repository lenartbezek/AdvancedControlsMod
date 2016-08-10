using System;
using System.Reflection;
using spaar.ModLoader;
using Lench.AdvancedControls.UI;
using Lench.AdvancedControls.Input;
using Lench.AdvancedControls.Controls;
using System.Collections.Generic;
using UnityEngine;

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
        
        public override string VersionExtra { get; } = "test";
        public override string BesiegeVersion { get; } = "v0.32";
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
            BlockHandlers.OnInitialisation += ACM.Instance.Initialise;
            XmlSaver.OnSave += MachineData.Save;
            XmlLoader.OnLoad += MachineData.Load;

            PythonEnvironment.LoadPythonAssembly();
            Blocks.Block.LoadBlockLoaderAssembly();

            ImportPythonModules();
        }

        /// <summary>
        /// Unsubscribes from all events and destroys the mod.
        /// </summary>
        public override void OnUnload()
        {
            Game.OnSimulationToggle -= SimulationToggle;
            BlockHandlers.OnInitialisation -= ACM.Instance.Initialise;
            XmlSaver.OnSave -= MachineData.Save;
            XmlLoader.OnLoad -= MachineData.Load;
            Configuration.Save();

            UnityEngine.Object.Destroy(ACM.Instance);
        }

        /// <summary>
        /// Checks if LenchScripterMod is present and adds initialisation statements
        /// that import AdvancedControls module.
        /// </summary>
        private void ImportPythonModules()
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
                return;
            }
        }

        /// <summary>
        /// Destroys old block handlers on every simulation toggle,
        /// so they can be created again after block array is populated.
        /// </summary>
        private void SimulationToggle(bool simulating)
        {
            if (simulating) Functions.ResetTimer();
            BlockHandlers.DestroyBlockHandlers();
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
        public override string Name { get { return "Advanced Controls"; } }

        /// <summary>
        /// Is mod enabled in the settings menu.
        /// </summary>
        public bool ModEnabled = true;

        /// <summary>
        /// Is automatic game controller database updater enabled.
        /// Changed with `acm dbupdate enable/disable` command.
        /// </summary>
        public bool DBUpdaterEnabled = false;

        /// <summary>
        /// Is automatic mod update checker enabled.
        /// Changed with `acm modupdate enable/disable` command.
        /// </summary>
        public bool ModUpdaterEnabled = false;

        internal bool LoadedMachine = false;

        internal delegate void UpdateEventHandler();
        internal event UpdateEventHandler OnUpdate;

        internal delegate void InitialiseEventHandler();
        internal event InitialiseEventHandler OnInitialisation;

        private Guid copy_source;

        private void Awake()
        {
            gameObject.AddComponent<BlockHandlers>();
            gameObject.AddComponent<ControlMapper>();
            gameObject.AddComponent<DeviceManager>();

            Commands.RegisterCommand("controller", ControllerCommand, "Enter 'controller' for all available controller commands.");
            Commands.RegisterCommand("acm", ConfigurationCommand, "Enter 'acm' for all available configuration commands.");
            SettingsMenu.RegisterSettingsButton("ACM", EnableToggle, ModEnabled, 14);
        }

        private void Start()
        {
            Configuration.Load();

            if (ModUpdaterEnabled)
                CheckForModUpdate();

            if (DBUpdaterEnabled)
                CheckForDBUpdate();

            enabled = ModEnabled;

            DeviceManager.OnDeviceAdded += (SDL.SDL_Event e) => { Axes.AxisManager.ResolveMachineAxes(); };
        }

        private void OnDestroy()
        {
            OnUpdate = null;
            OnInitialisation = null;
            Destroy(BlockHandlers.Instance);
            Destroy(ControlMapper.Instance);
            Destroy(DeviceManager.Instance);
            Destroy(GameObject.Find("Advanced Controls").transform.gameObject);
        }

        private void Update()
        {
            // Initialize block handlers
            if (Game.IsSimulating && !BlockHandlers.Initialised)
                BlockHandlers.InitializeBlockHandlers();

            // Open or hide ACM mapper
            if (BlockMapper.CurrentInstance != null)
            {
                if (BlockMapper.CurrentInstance.Block != null && BlockMapper.CurrentInstance.Block != ControlMapper.Instance.Block)
                    ControlMapper.Instance.ShowBlockControls(BlockMapper.CurrentInstance.Block);

                if (BlockMapper.CurrentInstance.Block != null)
                {
                    if (InputManager.CopyKeys())
                        copy_source = BlockMapper.CurrentInstance.Block.Guid;
                    if (InputManager.PasteKeys())
                        ControlManager.CopyBlockControls(copy_source, BlockMapper.CurrentInstance.Block.Guid);
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
                        string result = "Controller list:\n";
                        if (Controller.NumDevices > 0)
                            for (int i = 0; i < Controller.NumDevices; i++)
                            {
                                var controller = Controller.Get(i);
                                result += i+": "+controller.Name+" ("+(controller.IsGameController ? "Controller" : "Joystick")+")\n"+ "\tGuid: " + controller.GUID+"\n";
                            }
                        else
                            result = "No devices connected.";
                        return result;
                    case "info":

                    default:
                        return "Invalid command. Enter 'controller' for all available commands.";
                }
            }
            else
            {
                return "Available commands:\n" +
                    "  controller list             \t List all connected devices.\n" +
                    "  controller info [index]     \t Show info of a device at index.\n";
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
                                    return "Checking for mod updates ...";
                                case "enable":
                                    ModUpdaterEnabled = true;
                                    return "Mod update checker enabled.";
                                case "disable":
                                    ModUpdaterEnabled = false;
                                    return "Mod update checker disabled.";
                                default:
                                    return "Invalid argument [check/enable/disable]. Enter 'acm' for all available commands.";
                            }
                        }
                        else
                        {
                            return "Missing argument [check/enable/disable]. Enter 'acm' for all available commands.";
                        }
                    case "dbupdate":
                        if (args.Length > 1)
                        {
                            switch (args[1].ToLower())
                            {
                                case "check":
                                    CheckForDBUpdate(true);
                                    return "Checking for controller DB updates ...";
                                case "enable":
                                    DBUpdaterEnabled = true;
                                    return "Controller DB update checker enabled.";
                                case "disable":
                                    DBUpdaterEnabled = false;
                                    return "Controller DB update checker disabled.";
                                default:
                                    return "Invalid argument [check/enable/disable]. Enter 'acm' for all available commands.";
                            }
                        }
                        else
                        {
                            return "Missing argument [check/enable/disable]. Enter 'acm' for all available commands.";
                        }
                    default:
                        return "Invalid command. Enter 'acm' for all available commands.";
                }
            }
            else
            {
                return "Available commands:\n" +
                    "  acm modupdate check  \t Checks for mod update.\n" +
                    "  acm modupdate enable \t Enables update checker.\n" +
                    "  acm modupdate disable\t Disables update checker.\n" +
                    "  acm dbupdate check   \t Checks for controller database update.\n" +
                    "  acm dbupdate enable  \t Enables automatic controller database updates.\n" +
                    "  acm dbupdate disable \t Disables automatic controller database updates.\n";
            }
        }

        private void CheckForModUpdate(bool verbose = false)
        {
            var updater = gameObject.AddComponent<Updater.Updater>();
            updater.Check(
                "Advanced Controls Mod",
                "https://api.github.com/repos/lench4991/AdvancedControlsMod/releases",
                Assembly.GetExecutingAssembly().GetName().Version,
                new List<Updater.Updater.Link>()
                    {
                            new Updater.Updater.Link() { DisplayName = "Spiderling forum page", URL = "http://forum.spiderlinggames.co.uk/index.php?threads/3150/" },
                            new Updater.Updater.Link() { DisplayName = "GitHub release page", URL = "https://github.com/lench4991/AdvancedControlsMod/releases/latest" }
                    },
                verbose);
        }

        private void CheckForDBUpdate(bool verbose = false)
        {
            StartCoroutine(DeviceManager.AssignMappings(true, verbose));
        }
    }
}
