using System;
using System.IO;
using System.Collections;
using System.Net;
using System.ComponentModel;
using UnityEngine;
using Lench.AdvancedControls.UI;

namespace Lench.AdvancedControls.Input
{
    internal class DeviceManager : SingleInstance<DeviceManager>
    {
        internal delegate void AxisMotionEventHandler(SDL.SDL_Event e);
        internal static event AxisMotionEventHandler OnAxisMotion;

        internal delegate void BallMotionEventHandler(SDL.SDL_Event e);
        internal static event BallMotionEventHandler OnBallMotion;

        internal delegate void HatMotionEventHandler(SDL.SDL_Event e);
        internal static event HatMotionEventHandler OnHatMotion;

        internal delegate void ButtonEventHandler(SDL.SDL_Event e, bool down);
        internal static event ButtonEventHandler OnButton;

        internal delegate void KeyEventHandler(SDL.SDL_Event e, bool down);
        internal static event KeyEventHandler OnKey;

        internal delegate void DeviceAddedEventHandler(SDL.SDL_Event e);
        internal static event DeviceAddedEventHandler OnDeviceAdded;

        internal delegate void DeviceRemovedEventHandler(SDL.SDL_Event e);
        internal static event DeviceRemovedEventHandler OnDeviceRemoved;

        internal delegate void DeviceRemappedEventHandler(SDL.SDL_Event e);
        internal static event DeviceRemappedEventHandler OnDeviceRemapped;

        public static bool SDL_Initialized = false;
        public static bool SDL_Installed = false;

        public override string Name { get { return "ACM: Device Manager"; } }

        private void Start()
        {
            InitSDL();
        }

        internal static void InitSDL()
        {
            try
            {
                SDL.SDL_SetMainReady();
                SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER | SDL.SDL_INIT_JOYSTICK);
                SDL_Initialized = true;
            }
            catch (DllNotFoundException)
            {
                SDL_Initialized = false;
            }

            if (SDL_Initialized)
                Instance.StartCoroutine(AssignMappings(false));
        }

        internal static void InstallSDL()
        {
            ControllerAxisEditor.downloading_in_progress = true;
            ControllerAxisEditor.download_button_text = "0.0 %";
            if (File.Exists(Application.dataPath + "/Mods/Resources/AdvancedControls/lib/SDL2.dll"))
                File.Delete(Application.dataPath + "/Mods/Resources/AdvancedControls/lib/SDL2.dll");
            if (!Directory.Exists(Application.dataPath + "/Mods/Resources/AdvancedControls/lib/"))
                Directory.CreateDirectory(Application.dataPath + "/Mods/Resources/AdvancedControls/lib/");
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
                    {
                        ControllerAxisEditor.download_button_text = (Convert.ToSingle(e.BytesReceived) / Convert.ToSingle(e.TotalBytesToReceive) * 100).ToString("0.0") + " %";
                    };
                    client.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                    {
                        ControllerAxisEditor.downloading_in_progress = false;
                        if (e.Error != null)
                        {
                            ControllerAxisEditor.download_button_text = "Error";
                            spaar.ModLoader.ModConsole.AddMessage(LogType.Log, "[ACM]: Error downloading file: SDL2.dll", e.Error.Message);
                        }
                        else
                        {
                            spaar.ModLoader.ModConsole.AddMessage(LogType.Log, "[ACM]: File downloaded: SDL2.dll");
                            ControllerAxisEditor.download_button_text = "Please restart Besiege";
                            SDL_Installed = true;
                            InitSDL();
                        } 
                    };
                    client.DownloadFileAsync(
                        new Uri("http://lench4991.github.io/AdvancedControlsMod/files/SDL2.dll"),
                        Application.dataPath + "/Mods/Resources/AdvancedControls/lib/SDL2.dll");
                }
                catch (Exception e)
                {
                    ControllerAxisEditor.downloading_in_progress = false;
                    ControllerAxisEditor.download_button_text = "Error";
                    spaar.ModLoader.ModConsole.AddMessage(LogType.Error, "[ACM]: Error downloading file: SDL2.dll", e.Message);
                }
            }
        }

        internal static IEnumerator AssignMappings(bool update, bool verbose = false)
        {
            string mappings = null;

            if (update)
            {
                var www = new WWW("http://lench4991.github.io/AdvancedControlsMod/db/gamecontrollerdb.txt");
                yield return www;

                if (www.isDone && string.IsNullOrEmpty(www.error))
                    mappings = www.text;
                else
                    if (verbose) Debug.Log("=> Unable to connect.");
            }

            var dir = Application.dataPath + "/Mods/Resources/AdvancedControls/";
            var file = "GameControllerMappings.txt";

            if (File.Exists(dir + file))
            {
                var current_mappings = File.ReadAllText(dir + file);
                if (update && mappings != null)
                {
                    if (mappings == current_mappings)
                    {
                        if (verbose) Debug.Log("=> Game Controller DB is up to date.");
                    }
                    else
                    {
                        File.WriteAllText(dir + file, mappings);
                        if (verbose) Debug.Log("=> Game Controller DB update successfull.");
                    }
                }
                else
                {
                    mappings = current_mappings;
                }
            }
            else
            {
                if (update && mappings != null)
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    File.WriteAllText(dir + file, mappings);
                    if (verbose) Debug.Log("=> Game Controller DB downloaded.");
                }
            }

            if (!SDL_Initialized) yield break;

            if (mappings != null)
                SDL.SDL_GameControllerAddMapping(mappings);

            try
            {
                var env_var = Environment.GetEnvironmentVariable("SDL_GAMECONTROLLERCONFIG", EnvironmentVariableTarget.User);
                if (string.IsNullOrEmpty(env_var))
                {
                    if (verbose) Debug.Log("=> SDL_GAMECONTROLLERCONFIG environment variable not set.");
                }
                else
                {
                    if (verbose) Debug.Log("=> Successfully read SDL_GAMECONTROLLERCONFIG environment variable.");
                    SDL.SDL_GameControllerAddMapping(env_var);
                }
            }
            catch
            {
                if (verbose) Debug.Log("=> SDL_GAMECONTROLLERCONFIG environment variable not retrieved.");
            }
        }

        private void OnDestroy()
        {
            if (SDL_Initialized)
                SDL.SDL_Quit();
        }

        private void Update()
        {
            if (SDL_Installed && !SDL_Initialized) InitSDL();
            if (!SDL_Initialized) return;

            SDL.SDL_Event e;

            while(SDL.SDL_PollEvent(out e) > 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        OnKey?.Invoke(e, true);
                        break;
                    case SDL.SDL_EventType.SDL_KEYUP:
                        OnKey?.Invoke(e, false);
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERAXISMOTION:
                        OnAxisMotion?.Invoke(e);
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                        OnButton?.Invoke(e, true);
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERBUTTONUP:
                        OnButton?.Invoke(e, false);
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                        Controller.AddDevice(e.cdevice.which);
                        OnDeviceAdded?.Invoke(e);
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
                        Controller.RemoveDisconnected();
                        OnDeviceRemoved?.Invoke(e);
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMAPPED:
                        OnDeviceRemapped?.Invoke(e);
                        break;
                    case SDL.SDL_EventType.SDL_JOYAXISMOTION:
                        OnAxisMotion?.Invoke(e);
                        break;
                    case SDL.SDL_EventType.SDL_JOYBALLMOTION:
                        OnBallMotion?.Invoke(e);
                        break;
                    case SDL.SDL_EventType.SDL_JOYHATMOTION:
                        OnHatMotion?.Invoke(e);
                        break;
                    case SDL.SDL_EventType.SDL_JOYBUTTONDOWN:
                        OnButton?.Invoke(e, true);
                        break;
                    case SDL.SDL_EventType.SDL_JOYBUTTONUP:
                        OnButton?.Invoke(e, false);
                        break;
                    case SDL.SDL_EventType.SDL_JOYDEVICEADDED:
                        Controller.AddDevice(e.jdevice.which);
                        OnDeviceAdded?.Invoke(e);
                        break;
                    case SDL.SDL_EventType.SDL_JOYDEVICEREMOVED:
                        Controller.RemoveDisconnected();
                        OnDeviceRemoved?.Invoke(e);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
