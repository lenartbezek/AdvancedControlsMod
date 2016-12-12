using System.Reflection;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace Lench.AdvancedControls.Blocks
{
    /// <summary>
    /// Handler for the Grabber block.
    /// </summary>
    public class Grabber : BlockHandler
    {
        private static readonly FieldInfo JoinFieldInfo = typeof(GrabberBlock).GetField("joinOnTriggerBlock", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly GrabberBlock _gb;
        private readonly JoinOnTriggerBlock _join;

        /// <summary>
        /// Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        public Grabber(BlockBehaviour bb) : base(bb)
        {
            _gb = bb.GetComponent<GrabberBlock>();
            _join = JoinFieldInfo.GetValue(_gb) as JoinOnTriggerBlock;
        }

        /// <summary>
        /// Invokes the block's action.
        /// Throws ActionNotFoundException if the block does not posess such action.
        /// </summary>
        /// <param name="actionName">Display name of the action.</param>
        public override void Action(string actionName)
        {
            actionName = actionName.ToUpper();
            if (actionName == "DETACH")
            {
                Detach();
                return;
            }
            throw new ActionNotFoundException("Block " + BlockName + " has no " + actionName + " action.");
        }

        /// <summary>
        /// Detach or grab with the Grabber.
        /// </summary>
        public void Detach()
        {
            _join.OnKeyPressed();
        }
    }
}
