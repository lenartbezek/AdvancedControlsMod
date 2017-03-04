using UnityEngine;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Lench.AdvancedControls.Axes
{
    /// <summary>
    /// Mouse axis is a screen-size independent mouse position axis.
    /// </summary>
    public class MouseAxis : InputAxis
    {
        /// <summary>
        /// Horizontal (X) or vertical (Y) axis.
        /// </summary>
        public Axis Axis { get; set; }

        /// <summary>
        /// Position on the screen where axis returns zero.
        /// 0 is center, -1 is left edge and +1 is right edge.
        /// </summary>
        public float Center
        {
            get { return _center; }
            set
            {
                _center = Mathf.Clamp(value, -1, 1);
            }
        }
        private float _center;

        /// <summary>
        /// Size of the range between -1 and +1 axis values.
        /// 1 for full screen and 0 for one pixel.
        /// </summary>
        public float Range
        {
            get { return _range; }
            set
            {
                _range = Mathf.Clamp(value, 0, 1);
            }
        }
        private float _range;

        /// <summary>
        /// Creates a new mouse axis with given name.
        /// </summary>
        /// <param name="name">Name of the axis.</param>
        public MouseAxis(string name) : base(name)
        {
            Type = AxisType.Mouse;
            Center = 0;
            Range = 0.5f;
            Axis = Axis.X;
        }

        /// <summary>
        /// Mouse axis output value.
        /// </summary>
        public override float OutputValue
        {
            get
            {
                var mousePos = Axis == Axis.X ? UnityEngine.Input.mousePosition.x : UnityEngine.Input.mousePosition.y;
                float screenSize = Axis == Axis.X ? Screen.width : Screen.height;
                var rangeSize = Range == 0 ? 1 : screenSize * Range / 2f;
                var center = screenSize/2f + screenSize/2f * Center;
                return Mathf.Clamp((mousePos - center) / rangeSize, -1f, 1f);
            }
        }

        /// <summary>
        /// Compares axis tuning parameters.
        /// </summary>
        /// <param name="other">Axis to compare with.</param>
        /// <returns>Returns true if axes are identical.</returns>
        public override bool Equals(InputAxis other)
        {
            var cast = other as MouseAxis;
            if (cast == null) return false;
            return Name == cast.Name &&
                   Axis == cast.Axis &&
                   Center == cast.Center &&
                   Range == cast.Range;
        }

        /// <summary>
        /// Mouse axis requires no initialisation.
        /// </summary>
        protected override void Initialise() { }

        /// <summary>
        /// Mouse axis requires no update.
        /// </summary>
        protected override void Update(){ }

        internal override InputAxis Clone()
        {
            var clone = new MouseAxis(Name)
            {
                Axis = Axis,
                Center = Center,
                Range = Range
            };
            return clone;
        }
    }
}
