using Triton;

namespace LiveTelemetry
{
    public interface IGarageUserControl
    {
        event AnonymousSignal Close;
        event Signal Chosen;

        void Draw();
        void Resize();
    }
}