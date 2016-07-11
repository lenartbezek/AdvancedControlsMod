using System;
using Lench.Scripter.Blocks;
using UnityEngine;

namespace Lench.AdvancedControls.Controls
{
    public class InputControl : Control
    {
        public override string Name { get; set; } = "INPUT";

        private Cog cog;
        private Steering steering;
        private Spring spring;

        public InputControl(Guid guid) : base(guid){}

        public override Block Block
        {
            get
            {
                if (cog != null) return cog;
                if (steering != null) return steering;
                if (spring != null) return spring;
                return null;
            }
            set
            {
                cog = value as Cog;
                steering = value as Steering;
                spring = value as Spring;
            }
        }

        protected override void Apply(float value)
        {
            if (value > 0)
                value = Mathf.Lerp(Center, Max, value);
            else
                value = Mathf.Lerp(Center, Min, -value);
            cog?.SetInput(value);
            steering?.SetInput(value);
            spring?.SetInput(value);
        }

        internal override Control Clone()
        {
            var clone = new InputControl(BlockGUID);
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
