using UnityEngine;

namespace AdvancedControls.Input
{
    internal class EventManager : MonoBehaviour
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

        private void Awake()
        {
            try
            {
                SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER | SDL.SDL_INIT_JOYSTICK);
                Controller.AssignMappings();
                SDL_Initialized = true;
            }
            catch
            {
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
