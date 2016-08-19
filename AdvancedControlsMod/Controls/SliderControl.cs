using System;
using UnityEngine;

namespace Lench.AdvancedControls.Controls
{
    /// <summary>
    /// Slider control can control any block's slider.
    /// </summary>
    public class SliderControl : Control
    {
        /// <summary>
        /// Creates a slider control for a block with given GUID.
        /// Slider property needs to be set before the control is used.
        /// </summary>
        /// <param name="guid">GUID of the block.</param>
        public SliderControl(Guid guid) : base(guid){}

        private string slider;

        /// <summary>
        /// Slider display name that the block controls. Also sets the control's display name.
        /// </summary>
        public string Slider {
            get
            {
                return slider;
            }
            set
            {
                slider = value;
                Name = slider;
            }
        }

        /// <summary>
        /// Applies the slider value.
        /// </summary>
        /// <param name="value">Value to be applied.</param>
        protected override void Apply(float value)
        {
            if (value > 0)
                value = Mathf.Lerp(Center, Max, value);
            else
                value = Mathf.Lerp(Center, Min, -value);
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
            var clone = new SliderControl(BlockGUID);
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
