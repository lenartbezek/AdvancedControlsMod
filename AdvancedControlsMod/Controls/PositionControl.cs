using System;
using UnityEngine;
using Lench.AdvancedControls.Blocks;
// ReSharper disable VirtualMemberCallInConstructor

namespace Lench.AdvancedControls.Controls
{
    /// <summary>
    /// Position control creates analog control for piston position.
    /// </summary>
    public class PositionControl : Control
    {
        private Piston _piston;

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
        public override BlockHandler Block
        {
            get
            {
                return _piston;
            }
            protected set
            {
                _piston = value as Piston;
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
            value = value > 0 
                ? Mathf.Lerp(Center, Max, value) 
                : Mathf.Lerp(Center, Min, -value);
            _piston?.SetPosition(value);
        }

        /// <summary>
        /// Clears Extend key from piston.
        /// </summary>
        protected override void ClearKeys()
        {
            Block.ClearKeys("EXTEND");
        }

        internal override Control Clone()
        {
            var clone = new PositionControl(BlockGUID)
            {
                Name = Name,
                Enabled = Enabled,
                Axis = Axis,
                Block = Block,
                Min = Min,
                Center = Center,
                Max = Max
            };
            return clone;
        }
    }
}
