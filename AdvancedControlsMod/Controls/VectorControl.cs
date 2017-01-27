using Lench.AdvancedControls.Blocks;
using System;
using Lench.AdvancedControls.Resources;
using Lench.AdvancedControls.UI;
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
        /// Vector control key is either 'HORIZONTAL' (x), 'VERTICAL' (y), or 'POWER' (z), depending on the axis.
        /// </summary>
        public override string Key
        {
            get
            {
                switch (_vectorAxis)
                {
                    case global::Axis.X:
                        return "HORIZONTAL";
                    case global::Axis.Y:
                        return "VERTICAL";
                    case global::Axis.Z:
                        return "POWER";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Localized display name.
        /// </summary>
        public override string Name
        {
            get
            {
                switch (_vectorAxis)
                {
                    case global::Axis.X:
                        return Strings.VectorControl_AxisHorizontal;
                    case global::Axis.Y:
                        return Strings.VectorControl_AxisVertical;
                    case global::Axis.Z:
                        return Strings.VectorControl_AxisPower;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Creates a new VectorControl for a block with given guid.
        /// Axis specifies what property of vector thruster should be controlled.
        /// </summary>
        /// <param name="guid">Blocks' guid.</param>
        /// <param name="axis">global::Axis</param>
        public VectorControl(Guid guid, Axis axis) : base(guid)
        {
            _vectorAxis = axis;
        }

        /// <summary>
        /// Control's block handler.
        /// Vector control only accepts Lench.ScripterMod.Blocks.VectorThruster handlers.
        /// </summary>
        public override Block Block
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
