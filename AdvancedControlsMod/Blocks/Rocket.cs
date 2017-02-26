namespace Lench.AdvancedControls.Blocks
{
    /// <summary>
    ///     Handler for the Rocket block.
    /// </summary>
    public class Rocket : Block
    {
        private readonly TimedRocket _tr;

        /// <summary>
        ///     Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        public Rocket(BlockBehaviour bb) : base(bb)
        {
            _tr = bb.GetComponent<TimedRocket>();
        }

        /// <summary>
        ///     Is true if the rocket has fired.
        /// </summary>
        public bool HasFired => _tr.hasFired;

        /// <summary>
        ///     Is true if the rocket has exploded.
        /// </summary>
        public bool HasExploded => _tr.hasExploded;

        /// <summary>
        ///     Launch the rocket.
        /// </summary>
        public void Launch()
        {
            if (_tr.hasFired) return;
            _tr.hasFired = true;
            _tr.StartCoroutine(_tr.Fire(0));
        }
    }
}