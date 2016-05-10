using UnityEngine;
using LenchScripter;

namespace AdvancedControlsMod
{
    public abstract class Axis
    {
        public virtual string Name { get; set; } = "new axis";
        public virtual float Input { get; } = 0;
        public virtual float Output { get; set; } = 0;

        public Axis()
        {
            AdvancedControls.Instance.OnUpdate += Update;
            AdvancedControls.Instance.OnReset += Reset;
        }

        public abstract void Reset();
        public abstract void Update();
    }

    public class OneKeyAxis : Axis
    {
        public float Gravity { get; set; }
        public float Sensitivity { get; set; }
        public KeyCode Key { get; set; }

        private float speed = 0;
        private float force = 0;

        public OneKeyAxis(KeyCode key, string name = "new axis", float sensitivity = 1, float gravity = 1) : base()
        {
            Key = key;
            Name = name;
            Sensitivity = sensitivity;
            Gravity = gravity;
        }

        public override float Input
        {
            get
            {
                return UnityEngine.Input.GetKey(Key) ? 1 : 0;
            }
        }

        public override void Reset()
        {
            speed = 0;
            force = 0;
            Output = 0;
        }

        public override void Update()
        {
            force = Input != 0 ? Input * Sensitivity : - Gravity;
            speed += force * Time.deltaTime;
            Output = Mathf.Clamp(Output + speed * Time.deltaTime, 0, 1);
            if (Output == 0 || Output == 1)
                speed = 0;
        }

        public OneKeyAxis Clone()
        {
            return new OneKeyAxis(Key, Name, Sensitivity, Gravity);
        }
    }

    public class TwoKeyAxis : Axis
    {
        public float Gravity { get; set; }
        public float Sensitivity { get; set; }
        public bool Snap { get; set; }
        public bool Invert { get; set; }
        public KeyCode PositiveKey { get; set; }
        public KeyCode NegativeKey { get; set; }

        private float speed = 0;
        private float force = 0;
        private float last = 0;

        public TwoKeyAxis(KeyCode positiveKey, KeyCode negativeKey, string name = "new axis", float sensitivity = 1, float gravity = 1, bool snap = false, bool invert = false) : base()
        {
            PositiveKey = positiveKey;
            NegativeKey = negativeKey;
            Name = name;
            Sensitivity = sensitivity;
            Gravity = gravity;
            Snap = snap;
            Invert = invert;
        }

        public override float Input
        {
            get
            {
                float p = UnityEngine.Input.GetKey(PositiveKey) ? 1 : 0;
                float n = UnityEngine.Input.GetKey(NegativeKey) ? -1 : 0;
                return (p + n) * (Invert ? -1 : 1);
            }
        }

        public override void Reset()
        {
            speed = 0;
            force = 0;
            Output = 0;
        }

        public override void Update()
        {
            float g_force = Output > 0 ? - Gravity : Gravity;
            force = Input != 0 ? Input * Sensitivity : g_force;
            speed += force * Time.deltaTime;
            Output = Mathf.Clamp(Output + speed * Time.deltaTime, -1, 1);
            if (Snap && Mathf.Abs(Output - Input) > 1)
            {
                speed = 0;
                Output = 0;
            }
            if (Input == 0 && (last > 0 != Output > 0))
            {
                speed = 0;
                Output = 0;
            }
            last = Output;
            if (Output == -1 || Output == 1)
                speed = 0;
        }

        public TwoKeyAxis Clone()
        {
            return new TwoKeyAxis(PositiveKey, NegativeKey, Name, Sensitivity, Gravity, Snap, Invert);
        }
    }

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
            value = value > 0 ? Mathf.Pow(value, Curvature) : - Mathf.Pow(-value, Curvature);
            return Mathf.Clamp(value, -1, 1);
        }

        public ControllerAxis Clone()
        {
            return new ControllerAxis(Axis, Name, Sensitivity, Curvature, Deadzone, Invert);
        }

        public override void Reset(){}

        public override void Update(){}
    }

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
