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

        public OneKeyAxis(KeyCode key = KeyCode.None,
            string name = "new one key axis",
            float sensitivity = 1, float gravity = 1) : base()
        {
            Key = key;
            Name = name;
            Sensitivity = sensitivity;
            Gravity = gravity;
            editor = new UI.OneKeyAxisEditor();
            editor.SetAxis(this);
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
            return new OneKeyAxis(Key, Name, Sensitivity, Gravity);
        }

        public override void Load(MachineInfo machineInfo)
        {
            Sensitivity = machineInfo.MachineData.ReadFloat("AC-Axis-" + Name + "-Sensitivity");
            Gravity = machineInfo.MachineData.ReadFloat("AC-Axis-" + Name + "-Gravity");
            Key = (KeyCode)System.Enum.Parse(typeof(KeyCode), machineInfo.MachineData.ReadString("AC-Axis-" + Name + "-Key"));
        }

        public override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Type", "OneKey");
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Sensitivity", Sensitivity);
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Gravity", Gravity);
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Key", Key.ToString());
        }
    }
}
