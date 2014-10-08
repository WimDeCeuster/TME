using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Service;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    public class KeyManagerBuilder  
    {
        private readonly IKeyManager _manager;

        private KeyManagerBuilder(IKeyManager manager)
        {
            _manager = manager;
        }

        public static KeyManagerBuilder InitializeFake()
        {
            var manager = A.Fake<IKeyManager>();

            return new KeyManagerBuilder(manager);
        }

        public IKeyManager Build()
        {
            return _manager;
        }
    }
}