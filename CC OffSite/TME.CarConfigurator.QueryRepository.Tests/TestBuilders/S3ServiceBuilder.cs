using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Service;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    public class S3ServiceBuilder
    {
        private readonly IS3Service _service;

        private S3ServiceBuilder(IS3Service service)
        {
            _service = service;
        }

        public static S3ServiceBuilder InitializeFake()
        {
            var service = A.Fake<IS3Service>();

            return new S3ServiceBuilder(service);
        }

        public IS3Service Build()
        {
            return _service;
        }
    }
}