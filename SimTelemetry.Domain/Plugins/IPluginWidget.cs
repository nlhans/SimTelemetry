namespace SimTelemetry.Domain.Plugins
{
    public interface IPluginWidget : IPluginBase
    {

        void Initialize();
        void Deinitialize();
        
    }
}