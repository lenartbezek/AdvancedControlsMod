using System.Collections.Generic;
using static Lench.AdvancedControls.Input.SDL;

namespace Lench.AdvancedControls.Input
{
    /// <summary>
    ///     Handling mapping buttons and axes to different game controllers.
    /// </summary>
    public class Mapping
    {
        private static readonly Dictionary<string, SDL_GameControllerButton> ButtonMappingStrings =
            new Dictionary<string, SDL_GameControllerButton>
            {
                {"a", SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A},
                {"b", SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B},
                {"x", SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X},
                {"y", SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y},
                {"back", SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK},
                {"guide", SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_GUIDE},
                {"start", SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START},
                {"leftstick", SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSTICK},
                {"rightstick", SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSTICK},
                {"leftshoulder", SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER},
                {"rightshoulder", SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER}
            };

        private static readonly Dictionary<string, SDL_GameControllerAxis> AxisMappingStrings =
            new Dictionary<string, SDL_GameControllerAxis>
            {
                {"leftx", SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX},
                {"lefty", SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY},
                {"rightx", SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX},
                {"righty", SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY},
                {"lefttrigger", SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT},
                {"righttrigger", SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT}
            };

        private readonly Dictionary<SDL_GameControllerAxis, int> _axisMappingDict =
            new Dictionary<SDL_GameControllerAxis, int>();

        private readonly Dictionary<SDL_GameControllerButton, int> _buttonMappingDict =
            new Dictionary<SDL_GameControllerButton, int>();

        /// <summary>
        ///     Creates a new Mapping object from mapping string.
        /// </summary>
        /// <param name="mapping">
        ///     String format: https://wiki.libsdl.org/SDL_GameControllerAddMapping
        ///     Example mapping:
        ///     c05000000000000c405000000000000,PS4 Controller,
        ///     a:b1,b:b2,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b12,
        ///     leftshoulder:b4,leftstick:b10,lefttrigger:a3,leftx:a0,lefty:a1,rightshoulder:b5,
        ///     rightstick:b11,righttrigger:a4,rightx:a2,righty:a5,start:b9,x:b0,y:b3,platform:Mac OS X,
        /// </param>
        public Mapping(string mapping)
        {
            String = mapping;
            var list = mapping.Split(',');

            foreach (var m in list)
            {
                var ms = m.Split(':');
                if (ms.Length < 2) continue;
                var name = ms[0];
                var bind = ms[1];
                int index;
                try
                {
                    index = int.Parse(bind.Substring(1));
                }
                catch
                {
                    continue;
                }

                if (ButtonMappingStrings.ContainsKey(name))
                    _buttonMappingDict[ButtonMappingStrings[name]] = index;

                if (AxisMappingStrings.ContainsKey(name))
                    _axisMappingDict[AxisMappingStrings[name]] = index;
            }
        }

        /// <summary>
        ///     String used to initialize this mapping object.
        /// </summary>
        public string String { get; }

        /// <summary>
        ///     Wrapper for GetIndexForAxis(SDL_GameControllerAxis i).
        /// </summary>
        public int this[SDL_GameControllerAxis key] => GetIndexForAxis(key);

        /// <summary>
        ///     Wrapper for GetIndexForButton(SDL_GameControllerButton i).
        /// </summary>
        public int this[SDL_GameControllerButton key] => GetIndexForButton(key);

        /// <summary>
        ///     Get physical index of an axis described by the enumerator.
        /// </summary>
        /// <param name="i">SDL_GameControllerAxis enum</param>
        public int GetIndexForAxis(SDL_GameControllerAxis i)
        {
            return _axisMappingDict.ContainsKey(i) ? _axisMappingDict[i] : (int) i;
        }

        /// <summary>
        ///     Get physical index of a button described by the enumerator.
        /// </summary>
        /// <param name="i">SDL_GameControllerButton enum</param>
        public int GetIndexForButton(SDL_GameControllerButton i)
        {
            return _buttonMappingDict.ContainsKey(i) ? _buttonMappingDict[i] : (int) i;
        }
    }
}