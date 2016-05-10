using UnityEngine;

namespace AdvancedControlsMod.Axes
{
    public class OneKeyAxis : Axis
    {
        public float Gravity { get; set; }
        public float Sensitivity { get; set; }
        public KeyCode Key { get; set; }

        private float speed = 0;
        private float force = 0;

        public OneKeyAxis(KeyCode key, string name = "new axis", float sensitivity = 1, float gravity = 1) : base()
        {
            Key = key;
            Name = name;
            Sensitivity = sensitivity;
            Gravity = gravity;
        }

        public override float Input
        {
            get
            {
                return UnityEngine.Input.GetKey(Key) ? 1 : 0;
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
            force = Input != 0 ? Input * Sensitivity : -Gravity;
            speed += force * Time.deltaTime;
            Output = Mathf.Clamp(Output + speed * Time.deltaTime, 0, 1);
            if (Output == 0 || Output == 1)
                speed = 0;
        }

        public OneKeyAxis Clone()
        {
            return new OneKeyAxis(Key, Name, Sensitivity, Gravity);
        }
    }
}
