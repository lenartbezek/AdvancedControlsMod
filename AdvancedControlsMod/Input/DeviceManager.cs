using System;
using System.IO;
using System.Collections;
using System.Net;
using UnityEngine;
using Lench.AdvancedControls.UI;
// ReSharper disable UnusedMember.Local

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

        public static bool SdlInitialized;
        public static bool SdlInstalled;

        public override string Name => "ACM: Device Manager";

        private void Start()
        {
            InitSdl();
        }

        internal static void InitSdl()
        {
            try
            {
                SDL.SDL_SetMainReady();
                SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER | SDL.SDL_INIT_JOYSTICK);
                SdlInitialized = true;
            }
            catch (DllNotFoundException)
            {
                SdlInitialized = false;
            }

            if (SdlInitialized)
                Instance.StartCoroutine(AssignMappings(false));
        }

        internal static void InstallSdl()
        {
            ControllerAxisEditor.DownloadingInProgress = true;
            ControllerAxisEditor.DownloadButtonText = "0.0 %";
            if (File.Exists(Application.dataPath + "/Mods/Resources/AdvancedControls/lib/SDL2.dll"))
                File.Delete(Application.dataPath + "/Mods/Resources/AdvancedControls/lib/SDL2.dll");
            if (!Directory.Exists(Application.dataPath + "/Mods/Resources/AdvancedControls/lib/"))
                Directory.CreateDirectory(Application.dataPath + "/Mods/Resources/AdvancedControls/lib/");
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadProgressChanged += (sender, e) =>
                    {
                        ControllerAxisEditor.DownloadButtonText = (Convert.ToSingle(e.BytesReceived) / Convert.ToSingle(e.TotalBytesToReceive) * 100).ToString("0.0") + " %";
                    };
                    client.DownloadFileCompleted += (sender, e) =>
                    {
                        ControllerAxisEditor.DownloadingInProgress = false;
                        if (e.Error != null)
                        {
                            ControllerAxisEditor.DownloadButtonText = "Error";
                            spaar.ModLoader.ModConsole.AddMessage(LogType.Log, "[ACM]: Error downloading file: SDL2.dll", e.Error.Message);
                        }
                        else
                        {
                            spaar.ModLoader.ModConsole.AddMessage(LogType.Log, "[ACM]: File downloaded: SDL2.dll");
                            ControllerAxisEditor.DownloadButtonText = "Please restart Besiege";
                            SdlInstalled = true;
                            InitSdl();
                        } 
                    };
                    client.DownloadFileAsync(
                        new Uri("http://lench4991.github.io/AdvancedControlsMod/files/SDL2.dll"),
                        Application.dataPath + "/Mods/Resources/AdvancedControls/lib/SDL2.dll");
                }
                catch (Exception e)
                {
                    ControllerAxisEditor.DownloadingInProgress = false;
                    ControllerAxisEditor.DownloadButtonText = "Error";
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
                var currentMappings = File.ReadAllText(dir + file);
                if (update && mappings != null)
                {
                    if (mappings == currentMappings)
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
                    mappings = currentMappings;
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

            if (!SdlInitialized) yield break;

            if (mappings != null)
                SDL.SDL_GameControllerAddMapping(mappings);

            try
            {
                var envVar = Environment.GetEnvironmentVariable("SDL_GAMECONTROLLERCONFIG", EnvironmentVariableTarget.User);
                if (string.IsNullOrEmpty(envVar))
                {
                    if (verbose) Debug.Log("=> SDL_GAMECONTROLLERCONFIG environment variable not set.");
                }
                else
                {
                    if (verbose) Debug.Log("=> Successfully read SDL_GAMECONTROLLERCONFIG environment variable.");
                    SDL.SDL_GameControllerAddMapping(envVar);
                }
            }
            catch
            {
                if (verbose) Debug.Log("=> SDL_GAMECONTROLLERCONFIG environment variable not retrieved.");
            }
        }

        private void OnDestroy()
        {
            if (SdlInitialized)
                SDL.SDL_Quit();
        }

        private void Update()
        {
            if (SdlInstalled && !SdlInitialized) InitSdl();
            if (!SdlInitialized) return;

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
                }
            }
        }
    }
}
