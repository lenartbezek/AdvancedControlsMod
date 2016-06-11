using System.IO;
using UnityEngine;

namespace AdvancedControls.Input
{
    public class EventManager : MonoBehaviour
    {
        public delegate void AxisMotionEventHandler(SDL.SDL_Event e);
        public event AxisMotionEventHandler OnAxisMotion;

        public delegate void BallMotionEventHandler(SDL.SDL_Event e);
        public event BallMotionEventHandler OnBallMotion;

        public delegate void HatMotionEventHandler(SDL.SDL_Event e);
        public event HatMotionEventHandler OnHatMotion;

        public delegate void ButtonEventHandler(SDL.SDL_Event e, bool down);
        public event ButtonEventHandler OnButton;

        public delegate void KeyEventHandler(SDL.SDL_Event e, bool down);
        public event KeyEventHandler OnKey;

        public delegate void DeviceAddedEventHandler(SDL.SDL_Event e);
        public event DeviceAddedEventHandler OnDeviceAdded;

        public delegate void DeviceRemovedEventHandler(SDL.SDL_Event e);
        public event DeviceRemovedEventHandler OnDeviceRemoved;

        public delegate void DeviceRemappedEventHandler(SDL.SDL_Event e);
        public event DeviceRemappedEventHandler OnDeviceRemapped;

        public bool SDL_Initialized = false;

        private void Start()
        {
            try
            {
                SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER | SDL.SDL_INIT_JOYSTICK);
                Controller.AssignMappings();
                SDL_Initialized = true;
                Debug.Log("[AdvancedControlsMod]: SDL2 initialized.");
            }
            catch (FileNotFoundException e)
            {
                Debug.Log("[AdvancedControlsMod]: SDL2 error.");
                Debug.LogException(e);
                enabled = false;
            }
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
                        Controller.AddController(e.cdevice.which);
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
                        Controller.AddJoystick(e.jdevice.which);
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
