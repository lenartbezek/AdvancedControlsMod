namespace Lench.AdvancedControls.Input
{
    public interface Button
    {
        string ID { get; }
        bool IsDown { get; }
        bool Pressed { get; }
        bool Released { get; }
        float Value { get; }
        string Name { get; }
        bool Connected { get; }
    }
}
