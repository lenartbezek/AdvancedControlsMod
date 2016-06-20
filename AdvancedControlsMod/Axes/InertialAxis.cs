using UnityEngine;

namespace AdvancedControls.Axes
{
    public class InertialAxis : StandardAxis
    {
        public override string Name { get; set; } = "new inertial axis";
        public override AxisType Type { get { return AxisType.Inertial; } }

        private float speed = 0;
        private float last = 0;

        public InertialAxis(string name) : base(name) { }

        protected override void Initialise()
        {
            speed = 0;
            OutputValue = 0;
        }

        protected override void Update()
        {
            float g_force = OutputValue > 0 ? -Gravity : Gravity;
            float force = InputValue * Sensitivity + (1 - Mathf.Abs(InputValue)) * g_force;
            speed += force * Time.deltaTime;
            OutputValue = Mathf.Clamp(OutputValue + speed * Time.deltaTime, -1, 1);
            if (Snap && Mathf.Abs(OutputValue - InputValue) > 1)
            {
                speed = 0;
                OutputValue = 0;
            }
            if (InputValue == 0 && Gravity != 0 && (last > 0 != OutputValue > 0))
            {
                speed = 0;
                OutputValue = 0;
            }
            last = OutputValue;
            if (OutputValue == -1 || OutputValue == 1)
                speed = 0;
        }

        internal override InputAxis Clone()
        {
            var clone = new InertialAxis(Name);
            clone.PositiveBind = PositiveBind;
            clone.NegativeBind = NegativeBind;
            clone.Sensitivity = Sensitivity;
            clone.Gravity = Gravity;
            clone.Snap = Snap;
            clone.Invert = Snap;
            return clone;
        }
    }
}
