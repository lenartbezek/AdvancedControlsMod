namespace Lench.AdvancedControls.Input
{
    /// <summary>
    /// Button interface for mapping in Input axes.
    /// </summary>
    public interface Button
    {
        /// <summary>
        /// Identifier string containing full information.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Button's display name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Button value is 1 if pressed and 0 if released.
        /// Analog buttons can take values in between.
        /// </summary>
        float Value { get; }

#pragma warning disable CS1591
        bool IsDown { get; }
        bool Pressed { get; }
        bool Released { get; }
        bool Connected { get; }
    }
}
