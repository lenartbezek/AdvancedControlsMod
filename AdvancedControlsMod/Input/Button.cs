namespace AdvancedControls.Input
{
    public interface Button
    {
        bool IsDown { get; }
        bool Pressed { get; }
        bool Released { get; }
        float Value { get; }
        string Name { get; }
        bool Connected { get; }
    }
}
