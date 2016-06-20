using System;
using UnityEngine;

namespace AdvancedControls.Controls
{
    public class SliderControl : Control
    {
        public SliderControl(Guid guid) : base(guid){}

        public override string Name { get; set; } = "SLIDER";
        public override bool PositiveOnly { get; set; } = false;

        private string slider;
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

        protected override void Apply(float value)
        {
            if (value > 0)
                value = Mathf.Lerp(Center, Max, value);
            else
                value = Mathf.Lerp(Center, Min, -value);
            Block?.SetSliderValue(Slider, value);
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
