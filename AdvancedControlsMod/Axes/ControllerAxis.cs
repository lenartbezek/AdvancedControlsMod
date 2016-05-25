using UnityEngine;

namespace AdvancedControls.Axes
{
    public class ControllerAxis : InputAxis
    {
        public override string Name { get; set; } = "new controller axis";
        public float Sensitivity { get; set; }
        public float Curvature { get; set; }
        public float Deadzone { get; set; }
        public bool Invert { get; set; }
        public bool Raw { get; set; }
        public string Axis { get; set; } = "Vertical";

        public ControllerAxis(string name, string axis = "Vertical", float sensitivity = 1, float curvature = 1, float deadzone = 0, bool invert = false, bool raw = false) : base(name)
        {
            Type = AxisType.Controller;
            Axis = axis;
            Sensitivity = sensitivity;
            Curvature = curvature;
            Deadzone = deadzone;
            Invert = invert;
            Raw = raw;
            editor = new UI.ControllerAxisEditor(this);
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

        public override float InputValue
        {
            get
            {
                if (Raw)
                    return Input.GetAxisRaw(Axis);
                else
                {
                    return Input.GetAxis(Axis);
                }
            }
        }

        public override float OutputValue
        {
            get
            {
                return Process(InputValue);
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

        public override InputAxis Clone()
        {
            return new ControllerAxis(Name, Axis, Sensitivity, Curvature, Deadzone, Invert, Raw);
        }

        public override void Load(MachineInfo machineInfo)
        {
            if(machineInfo.MachineData.HasKey("ac-axis-" + Name + "-sensitivity"))
                Sensitivity = machineInfo.MachineData.ReadFloat("ac-axis-" + Name + "-sensitivity");
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-curvature"))
                Curvature = machineInfo.MachineData.ReadFloat("ac-axis-" + Name + "-curvature");
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-deadzone"))
                Deadzone = machineInfo.MachineData.ReadFloat("ac-axis-" + Name + "-deadzone");
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-invert"))
                Invert = machineInfo.MachineData.ReadBool("ac-axis-" + Name + "-invert");
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-raw"))
                Raw = machineInfo.MachineData.ReadBool("ac-axis-" + Name + "-raw");
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-axis"))
                Axis = machineInfo.MachineData.ReadString("ac-axis-" + Name + "-axis");
        }

        public override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("ac-axis-" + Name + "-type", "controller");
            machineInfo.MachineData.Write("ac-axis-" + Name + "-sensitivity", Sensitivity);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-curvature", Curvature);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-deadzone", Deadzone);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-invert", Invert);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-raw", Raw);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-axis", Axis);
        }

        public override void Initialise() { }

        public override void Update() { }
    }
}
