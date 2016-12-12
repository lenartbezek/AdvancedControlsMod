using Lench.AdvancedControls.Blocks;
using System;
using UnityEngine;

namespace Lench.AdvancedControls.Controls
{
    /// <summary>
    /// 3 axis vector control.
    /// </summary>
    public class VectorControl : Control
    {
        private readonly Axis _vectorAxis;
        private VectorThruster _vt;

        /// <summary>
        /// Creates a new VectorControl for a block with given guid.
        /// Axis specifies what property of vector thruster should be controlled.
        /// </summary>
        /// <param name="guid">Blocks' guid.</param>
        /// <param name="axis">global::Axis</param>
        public VectorControl(Guid guid, Axis axis = global::Axis.X) : base(guid)
        {
            _vectorAxis = axis;
            if (_vectorAxis == global::Axis.X)
                Name = "HORIZONTAL";
            if (_vectorAxis == global::Axis.Y)
                Name = "VERTICAL";
            if (_vectorAxis == global::Axis.Z)
                Name = "POWER";
        }

        /// <summary>
        /// Control's block handler.
        /// Vector control only accepts Lench.ScripterMod.Blocks.VectorThruster handlers.
        /// </summary>
        public override BlockHandler Block
        {
            get
            {
                return _vt;
            }
            protected set
            {
                _vt = value as VectorThruster;
            }
        }

        /// <summary>
        /// Applies the value to the vector thruster.
        /// HorizontalBias for X axis,
        /// VerticalBias for Y axis and
        /// Power for Z axis.
        /// </summary>
        /// <param name="value">Value to be applied.</param>
        protected override void Apply(float value)
        {
            value = value > 0 
                ? Mathf.Lerp(Center, Max, value) 
                : Mathf.Lerp(Center, Min, -value);
            if (_vectorAxis == global::Axis.X)
                _vt.HorizontalBias = value;
            if (_vectorAxis == global::Axis.Y)
                _vt.VerticalBias = value;
            if (_vectorAxis == global::Axis.Z)
                _vt.Power = value;
        }

        /// <summary>
        /// VectorThruster control is disabled on external control.
        /// No clearing keys needed.
        /// </summary>
        protected override void ClearKeys()
        {
            // pass
        }

        internal override Control Clone()
        {
            var clone = new VectorControl(BlockGUID, _vectorAxis)
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
