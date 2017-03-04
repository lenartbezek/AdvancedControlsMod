using System;
using System.Collections;
using System.IO;
using UnityEngine;

// ReSharper disable UnusedMember.Local

namespace Lench.AdvancedControls.Input
{
    internal class DeviceManager
    {
        public static bool SdlInitialized;
        public static bool SdlInstalled;

        private static DeviceManagerController _controller;
        internal static event AxisMotionEventHandler OnAxisMotion;
        internal static event BallMotionEventHandler OnBallMotion;
        internal static event HatMotionEventHandler OnHatMotion;
        internal static event ButtonEventHandler OnButton;
        internal static event KeyEventHandler OnKey;
        internal static event DeviceAddedEventHandler OnDeviceAdded;
        internal static event DeviceRemovedEventHandler OnDeviceRemoved;
        internal static event DeviceRemappedEventHandler OnDeviceRemapped;

        public static void InitSdl()
        {
#if DEBUG
            Debug.Log("Attempting to initialize SDL.");
#endif
            try
            {
                SDL.SDL_SetMainReady();
                SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER | SDL.SDL_INIT_JOYSTICK);
                SdlInitialized = true;
#if DEBUG
                Debug.Log("SDL Successfully initialized.");
#endif
            }
            catch (DllNotFoundException)
            {
                SdlInitialized = false;
#if DEBUG
                Debug.Log("Failed to inizialize SDL.");
#endif
            }

            if (!SdlInitialized) return;

            _controller = Mod.Controller.AddComponent<DeviceManagerController>();
            AssignMappings(false);
        }

        public static void AssignMappings(bool update, bool verbose = false)
        {
            _controller.StartCoroutine(DeviceManagerController.AssignMappingsCoroutine(update, verbose));
        }

        internal delegate void AxisMotionEventHandler(SDL.SDL_Event e);

        internal delegate void BallMotionEventHandler(SDL.SDL_Event e);

        internal delegate void HatMotionEventHandler(SDL.SDL_Event e);

        internal delegate void ButtonEventHandler(SDL.SDL_Event e, bool down);

        internal delegate void KeyEventHandler(SDL.SDL_Event e, bool down);

        internal delegate void DeviceAddedEventHandler(SDL.SDL_Event e);

        internal delegate void DeviceRemovedEventHandler(SDL.SDL_Event e);

        internal delegate void DeviceRemappedEventHandler(SDL.SDL_Event e);

        // ReSharper disable once ClassNeverInstantiated.Local
        private class DeviceManagerController : MonoBehaviour
        {
            private void OnDestroy()
            {
                if (SdlInitialized)
                    SDL.SDL_Quit();
            }

            private void Update()
            {
                if (SdlInstalled && !SdlInitialized) InitSdl();
                if (!SdlInitialized) return;

                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) > 0)
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
#if DEBUG
                        Debug.Log("SDL Event: SDL_CONTROLLERDEVICEADDED.");
#endif
                            Controller.Add(e.cdevice.which);
                            OnDeviceAdded?.Invoke(e);
                            break;
                        case SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
#if DEBUG
                        Debug.Log("SDL Event: SDL_CONTROLLERDEVICEREREMOVED.");
#endif
                            Controller.RemoveDisconnected();
                            OnDeviceRemoved?.Invoke(e);
                            break;
                        case SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMAPPED:
#if DEBUG
                        Debug.Log("SDL Event: SDL_CONTROLLERDEVICEREMAPPED.");
#endif
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
#if DEBUG
                        Debug.Log("SDL Event: SDL_JOYDEVICEADDED.");
#endif
                            Controller.Add(e.jdevice.which);
                            OnDeviceAdded?.Invoke(e);
                            break;
                        case SDL.SDL_EventType.SDL_JOYDEVICEREMOVED:
#if DEBUG
                        Debug.Log("SDL Event: SDL_JOYDEVICEREMOVED.");
#endif
                            Controller.RemoveDisconnected();
                            OnDeviceRemoved?.Invoke(e);
                            break;
                    }
            }

            public static IEnumerator AssignMappingsCoroutine(bool update, bool verbose = false)
            {
                string mappings = null;

                if (update)
                {
                    var www = new WWW("http://lench4991.github.io/AdvancedControlsMod/db/gamecontrollerdb.txt");
                    yield return www;

                    if (www.isDone && string.IsNullOrEmpty(www.error))
                        mappings = www.text;
                    else if (verbose) Debug.Log("=> " + "unable to connect"); 
                }

                var dir = Application.dataPath + "/Mods/Resources/AdvancedControls/";
                const string file = "GameControllerMappings.txt";

                if (File.Exists(dir + file))
                {
                    var currentMappings = File.ReadAllText(dir + file);
                    if (update && mappings != null)
                        if (mappings == currentMappings)
                        {
                            if (verbose) Debug.Log("=> " + "DB is up to date"); // TODO: Localize
                        }
                        else
                        {
                            File.WriteAllText(dir + file, mappings);
                            if (verbose) Debug.Log("=> " + "DB update successful"); // TODO: Localize
                        }
                    else
                        mappings = currentMappings;
                }
                else
                {
                    if (update && mappings != null)
                    {
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        File.WriteAllText(dir + file, mappings);
                        if (verbose) Debug.Log("=> " + "DB downloaded"); // TODO: Localize
                    }
                }

                if (!SdlInitialized) yield break;

                if (mappings != null)
                    SDL.SDL_GameControllerAddMapping(mappings);

                try
                {
                    var envVar = Environment.GetEnvironmentVariable("SDL_GAMECONTROLLERCONFIG",
                        EnvironmentVariableTarget.User);
                    if (string.IsNullOrEmpty(envVar))
                    {
                        if (verbose) Debug.Log("=> " + "Env var set"); // TODO: Localize
                    }
                    else
                    {
                        if (verbose) Debug.Log("=> " + "Env var read");
                        SDL.SDL_GameControllerAddMapping(envVar);
                    }
                }
                catch
                {
                    if (verbose) Debug.Log("=> " + "Env var error"); // TODO: Localize
                }
            }
        }
    }
}