using FakeItEasy;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders.S3
{
    public class S3ServiceBuilder
    {
        private readonly IService _service;

        private S3ServiceBuilder(IService service)
        {
            _service = service;
        }

        public static S3ServiceBuilder InitializeFake()
        {
            var service = A.Fake<IService>();

            return new S3ServiceBuilder(service);
        }

        public IService Build()
        {
            return _service;
        }
    }
}