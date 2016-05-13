using UnityEngine;

namespace AdvancedControls.Axes
{
    public class ControllerAxis : Axis
    {
        public float Sensitivity { get; set; }
        public float Curvature { get; set; }
        public float Deadzone { get; set; }
        public bool Invert { get; set; }
        public bool Raw { get; set; }
        public string Axis { get; set; } = "Vertical";

        public ControllerAxis(string axis = "Vertical", string name = "new axis", float sensitivity = 1, float curvature = 1, float deadzone = 0, bool invert = false)
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

        public ControllerAxis Clone()
        {
            return new ControllerAxis(Axis, Name, Sensitivity, Curvature, Deadzone, Invert);
        }

        public override void Load(MachineInfo machineInfo)
        {
            Sensitivity = machineInfo.MachineData.ReadFloat("AC-Axis-" + Name + "-Sensitivity");
            Curvature = machineInfo.MachineData.ReadFloat("AC-Axis-" + Name + "-Curvature");
            Deadzone = machineInfo.MachineData.ReadFloat("AC-Axis-" + Name + "-Deadzone");
            Invert = machineInfo.MachineData.ReadBool("AC-Axis-" + Name + "-Invert");
            Raw = machineInfo.MachineData.ReadBool("AC-Axis-" + Name + "-Raw");
            Axis = machineInfo.MachineData.ReadString("AC-Axis-" + Name + "-Axis");
        }

        public override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Type", "Controller");
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Sensitivity", Sensitivity);
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Curvature", Curvature);
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Deadzone", Deadzone);
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Invert", Invert);
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Raw", Raw);
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Axis", Axis);
        }

        public override void Initialise() { }

        public override void Update() { }
    }
}
