using Triton;

namespace LiveTelemetry
{
    public interface IGarageUserControl
    {
        event AnonymousSignal Close;
        event AnonymousSignal Chosen;

        void Draw();
    }
}