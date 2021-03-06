﻿using System;
using UnityEngine;

namespace Lench.AdvancedControls.Controls
{
    /// <summary>
    /// Slider control can control any block's slider.
    /// </summary>
    public class SliderControl : Control
    {
        /// <summary>
        /// Slider control key equals to the slider key.
        /// </summary>
        public override string Key => Slider;

        /// <summary>
        /// Creates a slider control for a block with given GUID.
        /// Slider property needs to be set before the control is used.
        /// </summary>
        /// <param name="guid">GUID of the block.</param>
        public SliderControl(Guid guid) : base(guid){}

        /// <summary>
        /// Slider display name that the block controls. Also sets the control's display name.
        /// </summary>
        public string Slider { get; set; }

        /// <summary>
        /// Applies the slider value.
        /// </summary>
        /// <param name="value">Value to be applied.</param>
        protected override void Apply(float value)
        {
            value = value > 0 
                ? Mathf.Lerp(Center, Max, value) 
                : Mathf.Lerp(Center, Min, -value);
            Block?.SetSliderValue(Slider, value);
        }

        /// <summary>
        /// Nothing to clear.
        /// </summary>
        protected override void ClearKeys()
        {
            // pass
        }

        internal override Control Clone()
        {
            var clone = new SliderControl(BlockGUID)
            {
                Slider = Slider,
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
