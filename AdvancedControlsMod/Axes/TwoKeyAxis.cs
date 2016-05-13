using UnityEngine;

namespace AdvancedControls.Axes
{
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

        public TwoKeyAxis(KeyCode positiveKey = KeyCode.None, KeyCode negativeKey = KeyCode.None,
            string name = "new axis",
            float sensitivity = 1, float gravity = 1, bool snap = false, bool invert = false) : base()
        {
            PositiveKey = positiveKey;
            NegativeKey = negativeKey;
            Name = name;
            Sensitivity = sensitivity;
            Gravity = gravity;
            Snap = snap;
            Invert = invert;
        }

        public override float InputValue
        {
            get
            {
                float p = UnityEngine.Input.GetKey(PositiveKey) ? 1 : 0;
                float n = UnityEngine.Input.GetKey(NegativeKey) ? -1 : 0;
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

        public TwoKeyAxis Clone()
        {
            return new TwoKeyAxis(PositiveKey, NegativeKey, Name, Sensitivity, Gravity, Snap, Invert);
        }

        public override void Load(MachineInfo machineInfo)
        {
            Sensitivity = machineInfo.MachineData.ReadFloat("AC-Axis-" + Name + "-Sensitivity");
            Gravity = machineInfo.MachineData.ReadFloat("AC-Axis-" + Name + "-Gravity");
            Snap = machineInfo.MachineData.ReadBool("AC-Axis-" + Name + "-Snap");
            Invert = machineInfo.MachineData.ReadBool("AC-Axis-" + Name + "-Invert");
            PositiveKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), machineInfo.MachineData.ReadString("AC-Axis-" + Name + "-PositiveKey"));
            NegativeKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), machineInfo.MachineData.ReadString("AC-Axis-" + Name + "-NegativeKey"));
        }

        public override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Type", "TwoKey");
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Sensitivity", Sensitivity);
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Gravity", Gravity);
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Snap", Snap);
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Invert", Invert);
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-PositiveKey", PositiveKey.ToString());
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-NegativeKey", NegativeKey.ToString());
        }
    }
}
