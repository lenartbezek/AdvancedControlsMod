using System;
using System.Collections.Generic;
using System.Reflection;
using Lench.AdvancedControls.Axes;
using Lench.AdvancedControls.Controls;
using Lench.AdvancedControls.Input;
using Lench.AdvancedControls.Resources;
using Lench.AdvancedControls.UI;
using spaar.ModLoader;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable UnusedMember.Local

namespace Lench.AdvancedControls
{
    /// <summary>
    ///     Mod class to be loaded by spaar's mod loader.
    /// </summary>
    public class Mod : spaar.ModLoader.Mod
    {
        /// <summary>
        ///     Is mod enabled in the settings menu.
        /// </summary>
        public static bool ModEnabled = true;

        /// <summary>
        ///     Is automatic game controller database updater enabled.
        ///     Changed with `acm dbupdate enable/disable` command.
        /// </summary>
        public static bool DbUpdaterEnabled;

        /// <summary>
        ///     Is automatic mod update checker enabled.
        ///     Changed with `acm modupdate enable/disable` command.
        /// </summary>
        public static bool ModUpdaterEnabled;

        internal static bool LoadedMachine;

        /// <summary>
        ///     Parent GameObject of all mod components.
        /// </summary>
        public static GameObject Controller { get; private set; }

        internal static ControlMapper ControlMapper { get; private set; }

        internal static event Action OnUpdate;

        /// <summary>
        ///     Creates main mod instance and subscribes to load and save events.
        /// </summary>
        public override void OnLoad()
        {
            Game.OnSimulationToggle += SimulationToggle;
            Game.OnSimulationToggle += Block.HandleSimulationToggle;
            XmlSaver.OnSave += MachineData.Save;
            XmlLoader.OnLoad += MachineData.Load;

            PythonEnvironment.LoadPythonAssembly();

            ImportPythonModules();

            Controller = new GameObject("AdvancedControlsMod");
            Controller.AddComponent<ModController>();
            DeviceManager.InitSdl();
            ControlMapper = Controller.AddComponent<ControlMapper>();
            Object.DontDestroyOnLoad(Controller);

            Commands.RegisterCommand("controller", ControllerCommand, Strings.Console_Controller_AllAvailable);
            Commands.RegisterCommand("acm", ConfigurationCommand, Strings.Console_Acm_AllAvailable);

            new SettingsButton
            {
                Text = "ACM",
                FontSize = 18,
                OnToggle = EnableToggle,
                Value = ModEnabled
            }.Create();
        }

        /// <summary>
        ///     Unsubscribes from all events and destroys the mod.
        /// </summary>
        public override void OnUnload()
        {
            Game.OnSimulationToggle -= SimulationToggle;
            XmlSaver.OnSave -= MachineData.Save;
            XmlLoader.OnLoad -= MachineData.Load;
            Configuration.Save();

            Object.Destroy(Controller);
        }

        /// <summary>
        ///     Checks if LenchScripterMod is present and adds initialisation statements
        ///     that import AdvancedControls module.
        /// </summary>
        private static void ImportPythonModules()
        {
            try
            {
                var assembly = Assembly.LoadFrom(Application.dataPath + "/Mods/LenchScripterMod.dll");
                var type = assembly.GetType("Lench.Scripter.PythonEnvironment");
                var method = type.GetMethod("AddInitStatement", BindingFlags.Public | BindingFlags.Static);
                method.Invoke(null, new object[]
                {
                    "clr.AddReference(\"AdvancedControlsMod\")\n" +
                    "from Lench.AdvancedControls import AdvancedControls\n" +
                    "from Lench.AdvancedControls.Axes import AxisType\n" +
                    "from Lench.AdvancedControls.Axes.ChainAxis import ChainMethod"
                });
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        ///     Destroys old block handlers on every simulation toggle,
        ///     so they can be created again after block array is populated.
        /// </summary>
        private static void SimulationToggle(bool simulating)
        {
            if (simulating) Functions.ResetTimer();
        }

        private static void EnableToggle(bool active)
        {
            ModEnabled = active;
            if (!active) ControlMapper.Hide();
        }

        private static string ControllerCommand(string[] args, IDictionary<string, string> namedArgs)
        {
            if (args.Length <= 0) return Strings.Console_Controller_AllAvailableList;

            switch (args[0].ToLower())
            {
                case "list":
                    var result = Strings.Console_Controller_ControllerList;
                    if (Input.Controller.NumDevices > 0)
                        for (var i = 0; i < Input.Controller.NumDevices; i++)
                        {
                            var controller = Input.Controller.Get(i);
                            result +=
                                $"{i}: {controller.Name} ({(controller.IsGameController ? Strings.Console_Controller_ControllerTag : Strings.Console_Controller_JoystickTag)})\n\t" +
                                Strings.Controller_GUID + " " + controller.GUID + "\n";
                        }
                    else
                        result = Strings.Console_Controller_NoDevicesConnected;
                    return result;

                default:
                    return Strings.Console_Controller_InvalidCommand;
            }
        }

        private static string ConfigurationCommand(string[] args, IDictionary<string, string> namedArgs)
        {
            if (args.Length <= 0) return Strings.Console_Acm_AllAvailableList;

            switch (args[0].ToLower())
            {
                case "modupdate":
                    if (args.Length > 1)
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
                    return Strings.Console_Acm_UpdateMissingArgument;
                case "dbupdate":
                    if (args.Length > 1)
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
                    return Strings.Console_Acm_UpdateMissingArgument;
                default:
                    return Strings.Console_Acm_InvalidCommand;
            }
        }

        /// <summary>
        ///     Checks for mod update from GitHub release API.
        ///     Displays notification if new version available.
        /// </summary>
        /// <param name="verbose">Log status to console.</param>
        public static void CheckForModUpdate(bool verbose = false)
        {
            var updater = Controller.AddComponent<Updater>();
            updater.Check(
                Strings.Updater_WindowTitle,
                "https://api.github.com/repos/lench4991/AdvancedControlsMod/releases/latest",
                Assembly.GetExecutingAssembly().GetName().Version,
                new List<Updater.Link>
                {
                    new Updater.Link
                    {
                        DisplayName = Strings.Updater_SpiderlingForumLink,
                        URL = "http://forum.spiderlinggames.co.uk/index.php?threads/3150/"
                    },
                    new Updater.Link
                    {
                        DisplayName = Strings.Updater_GithubReleasePageLink,
                        URL = "https://github.com/lench4991/AdvancedControlsMod/releases/latest"
                    }
                },
                verbose);
        }

        /// <summary>
        ///     Updates controller database from GitHub.
        ///     On successful update it refreshes controller mappings.
        /// </summary>
        /// <param name="verbose"></param>
        public static void CheckForDbUpdate(bool verbose = false)
        {
            DeviceManager.AssignMappings(true, verbose);
        }

        /// <summary>
        ///     Mod parent game object.
        /// </summary>
        // ReSharper disable once ClassNeverInstantiated.Local
        private class ModController : MonoBehaviour
        {
            private Guid _copySource;

            private void Awake()
            {
                gameObject.AddComponent<ControlMapper>();
            }

            private void Start()
            {
                Configuration.Load();

                if (ModUpdaterEnabled)
                    CheckForModUpdate();

                if (DbUpdaterEnabled)
                    CheckForDbUpdate();

                enabled = ModEnabled;

                DeviceManager.OnDeviceAdded += e => { AxisManager.ResolveMachineAxes(); };
            }

            private void OnDestroy()
            {
                OnUpdate = null;
                Destroy(Controller);
                Destroy(GameObject.Find("Advanced Controls").transform.gameObject);
            }

            private void Update()
            {
                // Initialize block handlers
                if (Game.IsSimulating && !Block.Initialised)
                    Block.Initialize();

                // Open or hide ACM mapper
                if (BlockMapper.CurrentInstance != null)
                {
                    if (BlockMapper.CurrentInstance.Block != null &&
                        BlockMapper.CurrentInstance.Block != ControlMapper.Block)
                        ControlMapper.ShowBlockControls(BlockMapper.CurrentInstance.Block);

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
        }

#pragma warning disable CS1591
        public override string Name { get; } = "AdvancedControlsMod";
        public override string DisplayName { get; } = "Advanced Controls Mod";
        public override string Author { get; } = "Lench";

        public override Version Version
        {
            get
            {
                var v = Assembly.GetExecutingAssembly().GetName().Version;
                return new Version(v.Major, v.Minor, v.Build);
            }
        }

        public override string VersionExtra { get; } = "";
        public override string BesiegeVersion { get; } = "v0.42";
        public override bool CanBeUnloaded { get; } = true;
        public override bool Preload { get; } = false;
#pragma warning restore CS1591
    }
}