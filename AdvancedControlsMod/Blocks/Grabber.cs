using System.Reflection;

namespace Lench.AdvancedControls.Blocks
{
    /// <summary>
    ///     Handler for the Grabber block.
    /// </summary>
    public class Grabber : Block
    {
        private static readonly FieldInfo JoinFieldInfo = typeof(GrabberBlock).GetField("joinOnTriggerBlock",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly JoinOnTriggerBlock _join;

        /// <summary>
        ///     Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        public Grabber(BlockBehaviour bb) : base(bb)
        {
            var gb = bb.GetComponent<GrabberBlock>();
            _join = JoinFieldInfo.GetValue(gb) as JoinOnTriggerBlock;
        }

        /// <summary>
        ///     Detach or grab with the Grabber.
        /// </summary>
        public void Detach()
        {
            _join.OnKeyPressed();
        }
    }
}