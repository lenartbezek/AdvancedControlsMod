using System;
using UnityEngine;
using Lench.Scripter.Blocks;

namespace Lench.AdvancedControls.Controls
{
    public class AngleControl : Control
    {
        public override string Name { get; set; } = "ANGLE";

        private Steering steering;

        public AngleControl(Guid guid) : base(guid)
        {
            Min = -45;
            Center = 0;
            Max = 45;
        }

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

        protected override void Apply(float value)
        {
            if (value > 0)
                value = Mathf.Lerp(Center, Max, value);
            else
                value = Mathf.Lerp(Center, Min, -value);
            steering?.SetAngle(value);
        }

        internal override Control Clone()
        {
            var clone = new AngleControl(BlockGUID);
            clone.Name = Name;
            clone.Enabled = Enabled;
            clone.Axis = Axis;
            clone.Block = Block;
            clone.Min = Min;
            clone.Center = Center;
            clone.Max = Max;
            return clone;
        }
    }
}
