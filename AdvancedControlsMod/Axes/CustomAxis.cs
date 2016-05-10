using UnityEngine;
using LenchScripter;

namespace AdvancedControlsMod.Axes
{
    public class CustomAxis : Axis
    {

        public CustomAxis() : base() { }

        public string InitialisationCode { get; set; } = @"time = 0";
        public string UpdateCode { get; set; } =
@"time = time + Time.deltaTime
axis_value = Mathf.Sin(time)
return axis_value";

        public CustomAxis(string name = "new axis", string init = "", string update = "")
        {
            Name = name;
            InitialisationCode = init;
            UpdateCode = update;
        }

        public override void Update()
        {
            if (!Lua.IsActive) return;
            Output = Mathf.Clamp((float)Lua.Evaluate(@UpdateCode)[0], -1, 1);
        }

        public override void Reset()
        {
            if (!Lua.IsActive) return;
            Lua.Evaluate(InitialisationCode);
        }

        public CustomAxis Clone()
        {
            return new CustomAxis(Name, InitialisationCode, UpdateCode);
        }

    }
}
