using System;
using System.Collections.Generic;
using UnityEngine;
using Lench.AdvancedControls.Blocks;

// ReSharper disable VirtualMemberCallInConstructor

namespace Lench.AdvancedControls.Controls
{
    /// <summary>
    /// Angle control that applies angle value to steering blocks and steering hinges.
    /// </summary>
    public class AngleControl : Control
    {
        private Steering _steering;

        /// <summary>
        /// Creates an angle control for a block with given GUID.
        /// </summary>
        /// <param name="guid">GUID of the block.</param>
        public AngleControl(Guid guid) : base(guid)
        {
            Min = -45;
            Center = 0;
            Max = 45;
        }

        /// <summary>
        /// Angle control key is 'ANGLE'.
        /// </summary>
        public override string Key => "ANGLE";

        /// <summary>
        /// Localized display name of the control.
        /// </summary>
        public override string Name => "ANGLE";

        /// <summary>
        /// Control's block handler.
        /// </summary>
        public override Block Block
        {
            get
            {
                return _steering;
            }
            protected set
            {
                _steering = value as Steering;
            }
        }

        /// <summary>
        /// Applies the angle to the block.
        /// </summary>
        /// <param name="value">Angle in degrees.</param>
        protected override void Apply(float value)
        {
            value = value > 0 
                ? Mathf.Lerp(Center, Max, value) 
                : Mathf.Lerp(Center, Min, -value);
            if (_steering != null)
                _steering.Angle = value;
        }

        /// <summary>
        /// Clears Left and Right keys.
        /// </summary>
        protected override void ClearKeys()
        {
            Block.Keys["left"] = new List<KeyCode>();
            Block.Keys["right"] = new List<KeyCode>();
        }

        internal override Control Clone()
        {
            var clone = new AngleControl(BlockGUID)
            {
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
