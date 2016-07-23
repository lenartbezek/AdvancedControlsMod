using UnityEngine;

namespace Lench.AdvancedControls.Axes
{
    /// <summary>
    /// Inertial axis takes input from one or two keys and moves the output value at variable speed.
    /// </summary>
    public class InertialAxis : StandardAxis
    {
        private float speed = 0;
        private float last = 0;

        /// <summary>
        /// Creates an inertial axis with given name.
        /// </summary>
        /// <param name="name">Name of the axis.</param>
        public InertialAxis(string name) : base(name)
        {
            Type = AxisType.Inertial;
        }

        /// <summary>
        /// Resets output value and it's speed to zero.
        /// </summary>
        protected override void Initialise()
        {
            speed = 0;
            OutputValue = 0;
        }

        /// <summary>
        /// Reads input value and updates output value depending on pressed keys and settings.
        /// </summary>
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

        /// <summary>
        /// Compares axis tuning parameters.
        /// </summary>
        /// <param name="other">Axis to compare with.</param>
        /// <returns>Returns true if axes are identical.</returns>
        public override bool Equals(InputAxis other)
        {
            var cast = other as InertialAxis;
            if (cast == null) return false;
            return this.Name == cast.Name &&
                   this.Sensitivity == cast.Sensitivity &&
                   this.Gravity == cast.Gravity &&
                   this.Snap == cast.Snap &&
                   this.Invert == cast.Invert &&
                   this.PositiveBind.ID == cast.PositiveBind.ID &&
                   this.NegativeBind.ID == cast.NegativeBind.ID;
        }
    }
}
