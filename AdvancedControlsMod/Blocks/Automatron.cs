namespace Lench.AdvancedControls.Blocks
{
    /// <summary>
    ///     Handler for spaar's Automatron block.
    /// </summary>
    public class Automatron : Block
    {
        /// <summary>
        ///     Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        public Automatron(BlockBehaviour bb) : base(bb)
        {
        }

        /// <summary>
        ///     Triggers the block.
        /// </summary>
        public void Activate()
        {
            Bs.SendMessage("TriggerActions");
        }
    }
}