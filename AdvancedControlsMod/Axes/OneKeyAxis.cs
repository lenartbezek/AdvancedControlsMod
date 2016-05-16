using UnityEngine;

namespace AdvancedControls.Axes
{
    public class OneKeyAxis : InputAxis
    {
        public override string Name { get; set; } = "new one key axis";
        public float Gravity { get; set; }
        public float Sensitivity { get; set; }
        public KeyCode Key { get; set; }

        private float speed = 0;
        private float force = 0;

        public OneKeyAxis(string name, KeyCode key = KeyCode.None,
            float sensitivity = 1, float gravity = 1) : base(name)
        {
            Type = AxisType.OneKey;
            Key = key;
            Sensitivity = sensitivity;
            Gravity = gravity;
            editor = new UI.OneKeyAxisEditor(this);
        }

        public override float InputValue
        {
            get
            {
                return Input.GetKey(Key) ? 1 : 0;
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
            force = InputValue != 0 ? InputValue * Sensitivity : -Gravity;
            speed += force * Time.deltaTime;
            OutputValue = Mathf.Clamp(OutputValue + speed * Time.deltaTime, 0, 1);
            if (OutputValue == 0 || OutputValue == 1)
                speed = 0;
        }

        public override InputAxis Clone()
        {
            return new OneKeyAxis(Name, Key, Sensitivity, Gravity);
        }

        public override void Load(MachineInfo machineInfo)
        {
            Sensitivity = machineInfo.MachineData.ReadFloat("ac-axis-" + Name + "-sensitivity");
            Gravity = machineInfo.MachineData.ReadFloat("ac-axis-" + Name + "-gravity");
            Key = (KeyCode)System.Enum.Parse(typeof(KeyCode), machineInfo.MachineData.ReadString("ac-axis-" + Name + "-key"));
        }

        public override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("ac-axis-" + Name + "-type", "onekey");
            machineInfo.MachineData.Write("ac-axis-" + Name + "-sensitivity", Sensitivity);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-gravity", Gravity);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-key", Key.ToString());
        }
    }
}
