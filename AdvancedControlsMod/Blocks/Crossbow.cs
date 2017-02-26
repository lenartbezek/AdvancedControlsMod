namespace Lench.AdvancedControls.Blocks
{
    /// <summary>
    ///     Handler for the crossbow block.
    /// </summary>
    public class Crossbow : Block
    {
        private readonly CrossBowBlock _cbb;

        /// <summary>
        ///     Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        public Crossbow(BlockBehaviour bb) : base(bb)
        {
            _cbb = bb.GetComponent<CrossBowBlock>();
        }

        /// <summary>
        ///     Number of arrows remaining.
        /// </summary>
        public int Ammo
        {
            get { return _cbb.ammo; }
            set { _cbb.ammo = value; }
        }

        /// <summary>
        ///     Shoots the crossbow.
        /// </summary>
        public void Shoot()
        {
            _cbb.SendMessage("FIRE");
        }
    }
}