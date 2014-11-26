using FluentAssertions;
using System;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAKeyManager
{
    public class WhenRequestingACarsKey : TestBase
    {
        private IKeyManager _keyManager;
        private string _expectedKey;
        private string _actualKey;
        private Guid _publicationId;
        private Guid _timeFrameId;

        protected override void Arrange()
        {
            _keyManager = new KeyManager();

            _publicationId = Guid.NewGuid();
            _timeFrameId = Guid.NewGuid();

            _expectedKey = "publication/" + _publicationId + "/time-frame/" + _timeFrameId + "/cars.json";
        }

        protected override void Act()
        {
            _actualKey = _keyManager.GetCarsKey(_publicationId, _timeFrameId);
        }

        [Fact]
        public void ThenTheKeyShouldBeCorrect()
        {
            _actualKey.Should().Be(_expectedKey);
        }
    }
}
