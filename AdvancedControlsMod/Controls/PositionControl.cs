using System;
using UnityEngine;
using Lench.Scripter.Blocks;

namespace Lench.AdvancedControls.Controls
{
    /// <summary>
    /// Position control creates analog control for piston position.
    /// </summary>
    public class PositionControl : Control
    {
        private Piston piston;

        /// <summary>
        /// Creates a position control for a piston with given GUID.
        /// </summary>
        /// <param name="guid">GUID of the block.</param>
        public PositionControl(Guid guid) : base(guid)
        {
            Name = "POSITION";
            Min = 0;
            Center = 0.5f;
            Max = 1;
        }

        /// <summary>
        /// Control's block handler.
        /// Angle control only accepts Lench.ScripterMod.Blocks.Piston handlers.
        /// </summary>
        public override Block Block
        {
            get
            {
                return piston;
            }
            protected set
            {
                piston = value as Piston;
            }
        }

        /// <summary>
        /// Minimum (Left) interval value. Applied when bound axis value is -1.
        /// Piston controls interval is clamped between 0 and 1.
        /// </summary>
        public override float Min
        {
            get { return base.Min; }
            set { base.Min = Mathf.Clamp(value, 0, 1); }
        }

        /// <summary>
        /// Center interval value. Applied when bound axis value is 0.
        /// Piston controls interval is clamped between 0 and 1.
        /// </summary>
        public override float Center
        {
            get { return base.Center; }
            set { base.Center = Mathf.Clamp(value, 0, 1); }
        }

        /// <summary>
        /// Maximum (Right) interval value. Applied when bound axis value is +1.
        /// Piston controls interval is clamped between 0 and 1.
        /// </summary>
        public override float Max
        {
            get { return base.Max; }
            set { base.Max = Mathf.Clamp(value, 0, 1); }
        }

        /// <summary>
        /// Applies the value for piston position.
        /// </summary>
        /// <param name="value">Value to be applied.</param>
        protected override void Apply(float value)
        {
            if (value > 0)
                value = Mathf.Lerp(Center, Max, value);
            else
                value = Mathf.Lerp(Center, Min, -value);
            piston?.SetPosition(value);
        }

        internal override Control Clone()
        {
            var clone = new PositionControl(BlockGUID);
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
