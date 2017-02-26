using System.Reflection;

namespace Lench.AdvancedControls.Blocks
{
    /// <summary>
    ///     Handler for the Flamethrower block.
    /// </summary>
    public class Flamethrower : Block
    {
        private static readonly FieldInfo HoldFieldInfo = typeof(FlamethrowerController).GetField("holdToFire",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo KeyHeld = typeof(FlamethrowerController).GetField("keyHeld",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly FlamethrowerController _fc;
        private readonly MToggle _holdToFire;
        private bool _lastIgniteFlag;
        private bool _setIgniteFlag;

        /// <summary>
        ///     Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        public Flamethrower(BlockBehaviour bb) : base(bb)
        {
            _fc = bb.GetComponent<FlamethrowerController>();
            _holdToFire = HoldFieldInfo.GetValue(_fc) as MToggle;
        }

        // TODO: Add Active property

        /// <summary>
        ///     Remaining time of the flamethrower.
        /// </summary>
        public float RemainingTime
        {
            get { return 10 - _fc.timey; }
            set { _fc.timey = 10 - value; }
        }

        /// <summary>
        ///     Ignite the flamethrower.
        /// </summary>
        public void Ignite()
        {
            _setIgniteFlag = true;
        }

        /// <summary>
        ///     Handles igniting the Flamethrower.
        /// </summary>
        protected override void LateUpdate()
        {
            if (_setIgniteFlag)
            {
                if (!_fc.timeOut || StatMaster.GodTools.InfiniteAmmoMode)
                    if (_holdToFire.IsActive)
                        _fc.FlameOn();
                    else
                        _fc.Flame();
                _setIgniteFlag = false;
                _lastIgniteFlag = true;
            }
            else if (_lastIgniteFlag)
            {
                KeyHeld.SetValue(_fc, true);
                _lastIgniteFlag = false;
            }
        }
    }
}