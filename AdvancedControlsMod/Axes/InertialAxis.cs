using UnityEngine;
using AdvancedControls.Input;

namespace AdvancedControls.Axes
{
    public class InertialAxis : StandardAxis
    {
        public override string Name { get; set; } = "new inertial axis";

        private float speed = 0;
        private float last = 0;

        public InertialAxis(string name, Button pos_bind = null, Button neg_bind = null,
                            float sensitivity = 1, float gravity = 1, bool snap = false, bool invert = false)
            : base(name, pos_bind, neg_bind, sensitivity, gravity, snap, invert)
        {
            Type = AxisType.Inertial;
        }

        public override void Initialise()
        {
            speed = 0;
            OutputValue = 0;
        }

        public override void Update()
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

        public override InputAxis Clone()
        {
            return new InertialAxis(Name, PositiveBind, NegativeBind, Sensitivity, Gravity, Snap, Invert);
        }
    }
}
