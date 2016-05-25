using UnityEngine;

namespace AdvancedControls.Axes
{
    public class StandardAxis : InputAxis
    {
        public override string Name { get; set; } = "new standard axis";
        public float Gravity { get; set; }
        public float Sensitivity { get; set; }
        public bool Snap { get; set; }
        public bool Invert { get; set; }
        public KeyCode PositiveKey { get; set; }
        public KeyCode NegativeKey { get; set; }

        private float last = 0;

        public StandardAxis(string name, KeyCode positiveKey = KeyCode.None, KeyCode negativeKey = KeyCode.None,
            float sensitivity = 1, float gravity = 1, bool snap = false, bool invert = false) : base(name)
        {
            Type = AxisType.Standard;
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
            OutputValue = 0;
        }

        public override void Update()
        {
            float g_force = OutputValue > 0 ? -Gravity : Gravity;
            float force = InputValue != 0 ? InputValue * Sensitivity : g_force;
            OutputValue = Mathf.Clamp(OutputValue + force * Time.deltaTime, -1, 1);
            if (Snap && Mathf.Abs(OutputValue - InputValue) > 1)
                OutputValue = 0;
            if (InputValue == 0 && (last > 0 != OutputValue > 0))
                OutputValue = 0;
            last = OutputValue;
        }

        public override InputAxis Clone()
        {
            return new StandardAxis(Name, PositiveKey, NegativeKey, Sensitivity, Gravity, Snap, Invert);
        }

        public override void Load(MachineInfo machineInfo)
        {
            if(machineInfo.MachineData.HasKey("ac-axis-" + Name + "-sensitivity"))
                Sensitivity = machineInfo.MachineData.ReadFloat("ac-axis-" + Name + "-sensitivity");
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-gravity"))
                Gravity = machineInfo.MachineData.ReadFloat("ac-axis-" + Name + "-gravity");
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-snap"))
                Snap = machineInfo.MachineData.ReadBool("ac-axis-" + Name + "-snap");
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-invert"))
                Invert = machineInfo.MachineData.ReadBool("ac-axis-" + Name + "-invert");
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-positivekey"))
                PositiveKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), machineInfo.MachineData.ReadString("ac-axis-" + Name + "-positivekey"));
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-key"))
                PositiveKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), machineInfo.MachineData.ReadString("ac-axis-" + Name + "-key"));
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-negativekey"))
                NegativeKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), machineInfo.MachineData.ReadString("ac-axis-" + Name + "-negativekey"));
        }

        public override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("ac-axis-" + Name + "-type", "standard");
            machineInfo.MachineData.Write("ac-axis-" + Name + "-sensitivity", Sensitivity);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-gravity", Gravity);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-snap", Snap);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-invert", Invert);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-positivekey", PositiveKey.ToString());
            machineInfo.MachineData.Write("ac-axis-" + Name + "-negativekey", NegativeKey.ToString());
        }
    }
}
