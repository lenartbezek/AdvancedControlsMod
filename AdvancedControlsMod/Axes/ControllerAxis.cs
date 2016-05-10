using UnityEngine;

namespace AdvancedControlsMod.Axes
{
    public class ControllerAxis : Axis
    {
        public float Sensitivity { get; set; }
        public float Curvature { get; set; }
        public float Deadzone { get; set; }
        public bool Invert { get; set; }
        public string Axis { get; set; }

        public ControllerAxis(string axis, string name = "new axis", float sensitivity = 1, float curvature = 1, float deadzone = 0, bool invert = false)
        {
            Axis = axis;
            Name = name;
            Sensitivity = sensitivity;
            Curvature = curvature;
            Deadzone = deadzone;
            Invert = invert;
        }

        public struct Param
        {
            public string axis;
            public float sens;
            public float curv;
            public float dead;
            public bool inv;
        }

        public Param Parameters
        {
            get
            {
                return new Param()
                {
                    axis = Axis,
                    sens = Sensitivity,
                    curv = Curvature,
                    dead = Deadzone,
                    inv = Invert
                };
            }
        }

        public override float Input
        {
            get
            {
                return UnityEngine.Input.GetAxisRaw(Axis);
            }
        }

        public override float Output
        {
            get
            {
                return Process(Input);
            }
        }

        public float Process(float input)
        {
            float value;
            if (Mathf.Abs(input) < Deadzone)
                value = 0;
            else
                value = input > 0 ? input - Deadzone : input + Deadzone;
            value *= Sensitivity * (Invert ? -1 : 1);
            value = value > 0 ? Mathf.Pow(value, Curvature) : -Mathf.Pow(-value, Curvature);
            return Mathf.Clamp(value, -1, 1);
        }

        public ControllerAxis Clone()
        {
            return new ControllerAxis(Axis, Name, Sensitivity, Curvature, Deadzone, Invert);
        }

        public override void Reset() { }

        public override void Update() { }
    }
}
