using AdvancedControls.Input;
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
        public bool Smooth { get; set; }
        public int AxisID { get; set; }
        public int ControllerID { get; set; }

        public ControllerAxis(string name, int axis = 0, int controller = 0, float sensitivity = 1, float curvature = 1, float deadzone = 0, bool invert = false, bool raw = false) : base(name)
        {
            Type = AxisType.Controller;
            AxisID = axis;
            ControllerID = controller;
            Sensitivity = sensitivity;
            Curvature = curvature;
            Deadzone = deadzone;
            Invert = invert;
            Smooth = raw;
            editor = new UI.ControllerAxisEditor(this);
        }

        public struct Param
        {
            public int axis;
            public int controller;
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
                    axis = AxisID,
                    controller = ControllerID,
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
                if (Smooth)
                    return Controller.Get(ControllerID).GetAxisSmooth(AxisID);
                else
                    return Controller.Get(ControllerID).GetAxis(AxisID);
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
                return 0;
            else
                value = input > 0 ? input - Deadzone : input + Deadzone;
            value *= Sensitivity * (Invert ? -1 : 1);
            value = value > 0 ? Mathf.Pow(value, Curvature) : -Mathf.Pow(-value, Curvature);
            return Mathf.Clamp(value, -1, 1);
        }

        public override InputAxis Clone()
        {
            return new ControllerAxis(Name, AxisID, ControllerID, Sensitivity, Curvature, Deadzone, Invert, Smooth);
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
                Smooth = machineInfo.MachineData.ReadBool("ac-axis-" + Name + "-smooth");
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-axis"))
                AxisID = machineInfo.MachineData.ReadInt("ac-axis-" + Name + "-axis");
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-controller"))
                ControllerID = machineInfo.MachineData.ReadInt("ac-axis-" + Name + "-controller");
        }

        public override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("ac-axis-" + Name + "-type", "controller");
            machineInfo.MachineData.Write("ac-axis-" + Name + "-sensitivity", Sensitivity);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-curvature", Curvature);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-deadzone", Deadzone);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-invert", Invert);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-smooth", Smooth);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-axis", AxisID);
        }

        public override void Initialise() { }

        public override void Update() { }
    }
}
