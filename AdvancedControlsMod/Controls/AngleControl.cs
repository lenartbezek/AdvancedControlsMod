using System;
using UnityEngine;
using Lench.AdvancedControls.Blocks;

namespace Lench.AdvancedControls.Controls
{
    /// <summary>
    /// Angle control that applies angle value to steering blocks and steering hinges.
    /// </summary>
    public class AngleControl : Control
    {
        private Steering steering;

        /// <summary>
        /// Creates an angle control for a block with given GUID.
        /// </summary>
        /// <param name="guid">GUID of the block.</param>
        public AngleControl(Guid guid) : base(guid)
        {
            Name = "ANGLE";
            Min = -45;
            Center = 0;
            Max = 45;
        }

        /// <summary>
        /// Control's block handler.
        /// Angle control only accepts Lench.ScripterMod.Blocks.Steering handlers.
        /// </summary>
        public override Block Block
        {
            get
            {
                return steering;
            }
            protected set
            {
                steering = value as Steering;
            }
        }

        /// <summary>
        /// Applies the angle to the block.
        /// </summary>
        /// <param name="value">Angle in degrees.</param>
        protected override void Apply(float value)
        {
            if (value > 0)
                value = Mathf.Lerp(Center, Max, value);
            else
                value = Mathf.Lerp(Center, Min, -value);
            steering?.SetAngle(value);
        }

        /// <summary>
        /// Clears Left and Right keys.
        /// </summary>
        protected override void ClearKeys()
        {
            Block.ClearKeys("LEFT");
            Block.ClearKeys("RIGHT");
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
