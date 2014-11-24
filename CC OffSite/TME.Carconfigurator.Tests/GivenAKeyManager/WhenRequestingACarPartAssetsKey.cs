using System;
using FluentAssertions;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAKeyManager
{
    public class WhenRequestingACarPartAssetsKey : TestBase
    {
        private IKeyManager _keyManager;
        private string _expectedKey;
        private string _actualKey;
        private Guid _publicationId;
        private Guid _carId;
        private const string VIEW = "view";
        private const string MODE = "mode";

        protected override void Arrange()
        {
            _keyManager = new KeyManager();

            _publicationId = Guid.NewGuid();
            _carId = Guid.NewGuid();

            _expectedKey = "publication/" + _publicationId + "/car/" + _carId + "/assets/car-parts/" + VIEW + "/" + MODE;
        }

        protected override void Act()
        {
            _actualKey = _keyManager.GetCarPartAssetsKey(_publicationId, _carId, VIEW, MODE);
        }

        [Fact]
        public void ThenTheKeyShouldBeCorrect()
        {
            _actualKey.Should().Be(_expectedKey);
        } 
    }
}