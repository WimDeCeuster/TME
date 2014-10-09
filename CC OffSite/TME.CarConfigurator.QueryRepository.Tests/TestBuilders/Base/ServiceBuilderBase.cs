using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.Query.Tests.TestBuilders.Base
{
    public abstract class ServiceBuilderBase <T>
    {
        protected ISerialiser Serialiser;
        protected IService Service;
        protected IKeyManager KeyManager;

        public ServiceBuilderBase<T> WithSerializer(ISerialiser serialiser)
        {
            Serialiser = serialiser;

            return this;
        }

        public ServiceBuilderBase<T> WithService(IService service)
        {
            Service = service;

            return this;
        }

        public ServiceBuilderBase<T> WithKeyManager(IKeyManager keyManager)
        {
            KeyManager = keyManager;

            return this;
        }

        public abstract T Build();
    }
}