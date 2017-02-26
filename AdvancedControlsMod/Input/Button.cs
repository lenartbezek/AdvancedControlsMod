namespace Lench.AdvancedControls.Input
{
    /// <summary>
    ///     Button abstract class for mapping in Input axes.
    /// </summary>
    public abstract class Button
    {
        /// <summary>
        ///     Identifier string containing full information.
        /// </summary>
        public abstract string ID { get; }

        /// <summary>
        ///     Button's display name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        ///     Button value is 1 if pressed and 0 if released.
        ///     Analog buttons can take values in between.
        /// </summary>
        public abstract float Value { get; }
#pragma warning disable CS1591

        public abstract bool IsDown { get; }
        public abstract bool Pressed { get; }
        public abstract bool Released { get; }
        public abstract bool Connected { get; }


        protected bool Equals(Button other)
        {
            return ID == other.ID;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Button) obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}