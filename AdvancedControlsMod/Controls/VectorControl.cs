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
        private Axis vectorAxis;
        private VectorThruster vt;

        /// <summary>
        /// Creates a new VectorControl for a block with given guid.
        /// Axis specifies what property of vector thruster should be controlled.
        /// </summary>
        /// <param name="guid">Blocks' guid.</param>
        /// <param name="axis">global::Axis</param>
        public VectorControl(Guid guid, Axis axis = global::Axis.X) : base(guid)
        {
            vectorAxis = axis;
            if (vectorAxis == global::Axis.X)
                Name = "HORIZONTAL";
            if (vectorAxis == global::Axis.Y)
                Name = "VERTICAL";
            if (vectorAxis == global::Axis.Z)
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
                return vt;
            }
            protected set
            {
                vt = value as VectorThruster;
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
            if (value > 0)
                value = Mathf.Lerp(Center, Max, value);
            else
                value = Mathf.Lerp(Center, Min, -value);
            if (vectorAxis == global::Axis.X)
                vt.HorizontalBias = value;
            if (vectorAxis == global::Axis.Y)
                vt.VerticalBias = value;
            if (vectorAxis == global::Axis.Z)
                vt.Power = value;
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
            var clone = new VectorControl(BlockGUID, vectorAxis);
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
