namespace Lench.AdvancedControls.Blocks
{
    /// <summary>
    ///     Handler for the Grenade block.
    /// </summary>
    public class Grenade : Block
    {
        private readonly ControllableBomb _cb;

        /// <summary>
        ///     Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        public Grenade(BlockBehaviour bb) : base(bb)
        {
            _cb = bb.GetComponent<ControllableBomb>();
        }

        /// <summary>
        ///     Detonate the grenade.
        /// </summary>
        public void Detonate()
        {
            _cb.StartCoroutine_Auto(_cb.Explode());
        }
    }
}