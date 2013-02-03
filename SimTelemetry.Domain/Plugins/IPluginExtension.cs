namespace SimTelemetry.Domain.Plugins
{
    public interface IPluginExtension : IPluginBase
    {
        

        void Initialize();
        void Deinitialize();
        
    }
}