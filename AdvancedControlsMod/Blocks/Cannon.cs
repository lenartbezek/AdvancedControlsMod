namespace Lench.AdvancedControls.Blocks
{
    /// <summary>
    ///     Handler for cannon blocks; Cannon and Shrapnel Cannon.
    /// </summary>
    public class Cannon : Block
    {
        private readonly CanonBlock _cb;

        /// <summary>
        ///     Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        public Cannon(BlockBehaviour bb) : base(bb)
        {
            _cb = bb.GetComponent<CanonBlock>();
        }

        /// <summary>
        ///     Shoots the cannon.
        /// </summary>
        public void Shoot()
        {
            _cb.Shoot();
        }
    }
}