using System;
using System.IO;
using System.Collections;
using UnityEngine;

namespace Lench.AdvancedControls.Input
{
    internal class DeviceManager : SingleInstance<DeviceManager>
    {
        internal delegate void AxisMotionEventHandler(SDL.SDL_Event e);
        internal event AxisMotionEventHandler OnAxisMotion;

        internal delegate void BallMotionEventHandler(SDL.SDL_Event e);
        internal event BallMotionEventHandler OnBallMotion;

        internal delegate void HatMotionEventHandler(SDL.SDL_Event e);
        internal event HatMotionEventHandler OnHatMotion;

        internal delegate void ButtonEventHandler(SDL.SDL_Event e, bool down);
        internal event ButtonEventHandler OnButton;

        internal delegate void KeyEventHandler(SDL.SDL_Event e, bool down);
        internal event KeyEventHandler OnKey;

        internal delegate void DeviceAddedEventHandler(SDL.SDL_Event e);
        internal event DeviceAddedEventHandler OnDeviceAdded;

        internal delegate void DeviceRemovedEventHandler(SDL.SDL_Event e);
        internal event DeviceRemovedEventHandler OnDeviceRemoved;

        internal delegate void DeviceRemappedEventHandler(SDL.SDL_Event e);
        internal event DeviceRemappedEventHandler OnDeviceRemapped;

        public bool SDL_Initialized = false;
        public bool AutoDBUpdate = true;

        public override string Name { get { return "ACM: Device Manager"; } }

        private void Start()
        {
            try
            {
                SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER | SDL.SDL_INIT_JOYSTICK);
                StartCoroutine(AssignMappings());
                SDL_Initialized = true;
            }
            catch (Exception e)
            {
                Debug.Log("Error while initializing SDL engine.");
                Debug.LogException(e);
                enabled = false;
            }
        }

        private IEnumerator AssignMappings()
        {
            string mappings = null;

            if (AutoDBUpdate)
            {
                var www = new WWW("https://raw.githubusercontent.com/lench4991/AdvancedControlsMod/master/Resources/AdvancedControls/GameControllerMappings.txt");
                yield return www;

                if (www.isDone && string.IsNullOrEmpty(www.error))
                    mappings = www.text;
            }

            var dir = Application.dataPath + @"\Mods\Resources\AdvancedControls\";
            var file = "GameControllerMappings.txt";

            if (File.Exists(dir + file))
            {
                if (AutoDBUpdate && mappings != null)
                    File.WriteAllText(dir + file, mappings);
                else
                    mappings = File.ReadAllText(dir + file);
            }
            else
            {
                if (AutoDBUpdate && mappings != null)
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    File.WriteAllText(dir + file, mappings);
                }
            }

            if (mappings != null)
                SDL.SDL_GameControllerAddMapping(mappings);
        }

        private void OnDestroy()
        {
            if (SDL_Initialized)
                SDL.SDL_Quit();
        }

        private void Update()
        {
            if (!SDL_Initialized)
                return;

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
