#pragma warning disable CS0660, CS0661
namespace Lench.AdvancedControls.Input
{
    /// <summary>
    /// Button interface for mapping in Input axes.
    /// </summary>
    public abstract class Button
    {
        /// <summary>
        /// Identifier string containing full information.
        /// </summary>
        public abstract string ID { get; }

        /// <summary>
        /// Button's display name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Button value is 1 if pressed and 0 if released.
        /// Analog buttons can take values in between.
        /// </summary>
        public abstract float Value { get; }

        /// <summary>
        /// Compares buttons a and b by values.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Button a, Button b)
        {
            if ((object)a == null && (object)b == null)
                return true;
            if (((object)a == null) != ((object)b == null))
                return false;
            if (a.GetType() != b.GetType())
                return false;
            return a.ID == b.ID;
        }

        /// <summary>
        /// Compares buttons a and b by values.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Button a, Button b)
        {
            return !(a == b);
        }

#pragma warning disable CS1591
        public abstract bool IsDown { get; }
        public abstract bool Pressed { get; }
        public abstract bool Released { get; }
        public abstract bool Connected { get; }
    }
}
