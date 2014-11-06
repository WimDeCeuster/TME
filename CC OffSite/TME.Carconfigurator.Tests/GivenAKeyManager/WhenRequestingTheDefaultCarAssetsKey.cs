using System;
using FluentAssertions;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAKeyManager
{
    public class WhenRequestingTheDefaultCarAssetsKey : TestBase
    {
        private IKeyManager _keyManager;
        private string _expectedKey;
        private string _actualKey;
        private Guid _publicationId;
        private Guid _objectId;
        private Guid _carId;

        protected override void Arrange()
        {
            _keyManager = new KeyManager();

            _publicationId = Guid.NewGuid();
            _carId = Guid.NewGuid();
            _objectId = Guid.NewGuid();

            _expectedKey = "publication/" + _publicationId + "/car/" + _carId + "/assets/" + _objectId + "/default";
        }

        protected override void Act()
        {
            _actualKey = _keyManager.GetDefaultCarAssetsKey(_publicationId, _carId, _objectId);
        }

        [Fact]
        public void ThenTheKeyShouldBeCorrect()
        {
            _actualKey.Should().Be(_expectedKey);
        }
    }
}