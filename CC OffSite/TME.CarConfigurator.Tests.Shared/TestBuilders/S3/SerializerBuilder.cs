using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Service;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders.S3
{
    public class SerializerBuilder
    {
        private readonly ISerialiser _serializer;

        private SerializerBuilder(ISerialiser serializer)
        {
            _serializer = serializer;
        }

        public static SerializerBuilder InitializeFake()
        {
            var serializer = A.Fake<ISerialiser>();

            return new SerializerBuilder(serializer);
        }

        public ISerialiser Build()
        {
            return _serializer;
        }
    }
}