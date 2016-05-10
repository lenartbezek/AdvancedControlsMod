using System;
using UnityEngine;
using LenchScripter.Blocks;

namespace AdvancedControls.Controls
{
    public class AngleControl : Control
    {
        private Steering steering;

        public AngleControl(string guid) : base(guid){}

        public override float Min { get; set; } = -45;
        public override float Max { get; set; } = 45;

        public override Block Block
        {
            get
            {
                if (steering != null) return steering;
                return null;
            }
            set
            {
                steering = value as Steering;
            }
        }

        public override void Apply(float value)
        {
            if (value > 0)
                value = Mathf.Lerp(Center, Max, value);
            else
                value = Mathf.Lerp(Center, Min, -value);
            steering?.SetAngle(value);
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
