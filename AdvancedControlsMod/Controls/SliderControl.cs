using System;
using UnityEngine;

namespace AdvancedControls.Controls
{
    public class SliderControl : Control
    {
        public SliderControl(string guid) : base(guid){}

        public bool PositiveOnly { get; set; }
        public string Slider { get; set; }

        public override float Min
        {
            get { return base.Min; }

            set
            {
                base.Min = PositiveOnly && value < 0 ? 0 : value;
            }
        }

        public override void Apply(float value)
        {
            if (PositiveOnly)
                value = Mathf.Lerp(Min, Max, value);
            else if (value > 0)
                value = Mathf.Lerp(Center, Max, value);
            else
                value = Mathf.Lerp(Center, Min, -value);
            Block.setSliderValue(Slider, value);
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
