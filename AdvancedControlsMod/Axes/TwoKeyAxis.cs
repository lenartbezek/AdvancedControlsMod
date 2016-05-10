using UnityEngine;

namespace AdvancedControlsMod.Axes
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

        public TwoKeyAxis(KeyCode positiveKey, KeyCode negativeKey, string name = "new axis", float sensitivity = 1, float gravity = 1, bool snap = false, bool invert = false) : base()
        {
            PositiveKey = positiveKey;
            NegativeKey = negativeKey;
            Name = name;
            Sensitivity = sensitivity;
            Gravity = gravity;
            Snap = snap;
            Invert = invert;
        }

        public override float Input
        {
            get
            {
                float p = UnityEngine.Input.GetKey(PositiveKey) ? 1 : 0;
                float n = UnityEngine.Input.GetKey(NegativeKey) ? -1 : 0;
                return (p + n) * (Invert ? -1 : 1);
            }
        }

        public override void Reset()
        {
            speed = 0;
            force = 0;
            Output = 0;
        }

        public override void Update()
        {
            float g_force = Output > 0 ? -Gravity : Gravity;
            force = Input != 0 ? Input * Sensitivity : g_force;
            speed += force * Time.deltaTime;
            Output = Mathf.Clamp(Output + speed * Time.deltaTime, -1, 1);
            if (Snap && Mathf.Abs(Output - Input) > 1)
            {
                speed = 0;
                Output = 0;
            }
            if (Input == 0 && (last > 0 != Output > 0))
            {
                speed = 0;
                Output = 0;
            }
            last = Output;
            if (Output == -1 || Output == 1)
                speed = 0;
        }

        public TwoKeyAxis Clone()
        {
            return new TwoKeyAxis(PositiveKey, NegativeKey, Name, Sensitivity, Gravity, Snap, Invert);
        }
    }
}
