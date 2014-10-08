using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Service.Base
{
    public class ServiceBase
    {
        protected readonly ISerialiser Serialiser;
        protected readonly IService Service;
        protected readonly IKeyManager KeyManager;

        protected ServiceBase(ISerialiser serialiser, IService service, IKeyManager keyManager)
        {
            // TODO: create local defaults

            Serialiser = serialiser;
            Service = service;
            KeyManager = keyManager;
        }
    }
}