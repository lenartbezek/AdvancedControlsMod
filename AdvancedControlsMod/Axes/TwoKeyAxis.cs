using UnityEngine;

namespace AdvancedControls.Axes
{
    public class TwoKeyAxis : InputAxis
    {
        public override string Name { get; set; } = "new two key axis";
        public float Gravity { get; set; }
        public float Sensitivity { get; set; }
        public bool Snap { get; set; }
        public bool Invert { get; set; }
        public KeyCode PositiveKey { get; set; }
        public KeyCode NegativeKey { get; set; }

        private float speed = 0;
        private float force = 0;
        private float last = 0;

        public TwoKeyAxis(string name, KeyCode positiveKey = KeyCode.None, KeyCode negativeKey = KeyCode.None,
            float sensitivity = 1, float gravity = 1, bool snap = false, bool invert = false) : base(name)
        {
            Type = AxisType.TwoKey;
            PositiveKey = positiveKey;
            NegativeKey = negativeKey;
            Sensitivity = sensitivity;
            Gravity = gravity;
            Snap = snap;
            Invert = invert;
            editor = new UI.TwoKeyAxisEditor(this);
        }

        public override float InputValue
        {
            get
            {
                float p = Input.GetKey(PositiveKey) ? 1 : 0;
                float n = Input.GetKey(NegativeKey) ? -1 : 0;
                return (p + n) * (Invert ? -1 : 1);
            }
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
            return new TwoKeyAxis(Name, PositiveKey, NegativeKey, Sensitivity, Gravity, Snap, Invert);
        }

        public override void Load(MachineInfo machineInfo)
        {
            Sensitivity = machineInfo.MachineData.ReadFloat("ac-axis-" + Name + "-sensitivity");
            Gravity = machineInfo.MachineData.ReadFloat("ac-axis-" + Name + "-gravity");
            Snap = machineInfo.MachineData.ReadBool("ac-axis-" + Name + "-snap");
            Invert = machineInfo.MachineData.ReadBool("ac-axis-" + Name + "-invert");
            PositiveKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), machineInfo.MachineData.ReadString("ac-axis-" + Name + "-positivekey"));
            NegativeKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), machineInfo.MachineData.ReadString("ac-axis-" + Name + "-negativekey"));
        }

        public override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("ac-axis-" + Name + "-type", "twokey");
            machineInfo.MachineData.Write("ac-axis-" + Name + "-sensitivity", Sensitivity);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-gravity", Gravity);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-snap", Snap);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-invert", Invert);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-positivekey", PositiveKey.ToString());
            machineInfo.MachineData.Write("ac-axis-" + Name + "-negativekey", NegativeKey.ToString());
        }
    }
}
