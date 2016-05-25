using UnityEngine;

namespace AdvancedControls.Axes
{
    public class InertialAxis : StandardAxis
    {
        public override string Name { get; set; } = "new standard axis";

        private float speed = 0;
        private float force = 0;
        private float last = 0;

        public InertialAxis(string name, KeyCode positiveKey = KeyCode.None, KeyCode negativeKey = KeyCode.None,
            float sensitivity = 1, float gravity = 1, bool snap = false, bool invert = false) : base(name)
        {
            Type = AxisType.Inertial;
            PositiveKey = positiveKey;
            NegativeKey = negativeKey;
            Sensitivity = sensitivity;
            Gravity = gravity;
            Snap = snap;
            Invert = invert;
            editor = new UI.TwoKeyAxisEditor(this);
        }

        public override void Initialise()
        {
            speed = 0;
            force = 0;
            OutputValue = 0;
        }

        public override void Update()
        {
            float g_force = OutputValue > 0 ? -Gravity : Gravity;
            force = InputValue != 0 ? InputValue * Sensitivity : g_force;
            speed += force * Time.deltaTime;
            OutputValue = Mathf.Clamp(OutputValue + speed * Time.deltaTime, -1, 1);
            if (Snap && Mathf.Abs(OutputValue - InputValue) > 1)
            {
                speed = 0;
                OutputValue = 0;
            }
            if (InputValue == 0 && (last > 0 != OutputValue > 0))
            {
                speed = 0;
                OutputValue = 0;
            }
            last = OutputValue;
            if (OutputValue == -1 || OutputValue == 1)
                speed = 0;
        }

        public override InputAxis Clone()
        {
            return new InertialAxis(Name, PositiveKey, NegativeKey, Sensitivity, Gravity, Snap, Invert);
        }

        public override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("ac-axis-" + Name + "-type", "inertial");
            machineInfo.MachineData.Write("ac-axis-" + Name + "-sensitivity", Sensitivity);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-gravity", Gravity);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-snap", Snap);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-invert", Invert);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-positivekey", PositiveKey.ToString());
            machineInfo.MachineData.Write("ac-axis-" + Name + "-negativekey", NegativeKey.ToString());
        }
    }
}
