using UnityEngine;
using AdvancedControls.Input;

namespace AdvancedControls.Axes
{
    public class StandardAxis : InputAxis
    {
        public override string Name { get; set; } = "new standard axis";
        public float Gravity { get; set; }
        public float Sensitivity { get; set; }
        public bool Snap { get; set; }
        public bool Invert { get; set; }
        public Button PositiveBind { get; set; }
        public Button NegativeBind { get; set; }

        private float last = 0;

        public StandardAxis(string name, Button pos_bind = null, Button neg_bind = null,
            float sensitivity = 1, float gravity = 1, bool snap = false, bool invert = false) : base(name)
        {
            Type = AxisType.Standard;
            PositiveBind = pos_bind;
            NegativeBind = neg_bind;
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
                float p = PositiveBind != null ? PositiveBind.Value : 0;
                float n = NegativeBind != null ? NegativeBind.Value * -1 : 0;
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
            float force = InputValue * Sensitivity + (1 - Mathf.Abs(InputValue)) * g_force;
            OutputValue = Mathf.Clamp(OutputValue + force * Time.deltaTime, -1, 1);
            if (Snap && Mathf.Abs(OutputValue - InputValue) > 1)
                OutputValue = 0;
            if (InputValue == 0 && (last > 0 != OutputValue > 0))
                OutputValue = 0;
            last = OutputValue;
        }

        public override InputAxis Clone()
        {
            return new StandardAxis(Name, PositiveBind, NegativeBind, Sensitivity, Gravity, Snap, Invert);
        }

        public override void Load(MachineInfo machineInfo)
        {
            
        }

        public override void Save(MachineInfo machineInfo)
        {
            
        }
    }
}
