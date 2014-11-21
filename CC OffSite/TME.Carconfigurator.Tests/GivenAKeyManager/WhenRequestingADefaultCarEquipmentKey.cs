using System;
using FluentAssertions;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAKeyManager
{
    public class WhenRequestingADefaultCarEquipmentKey : TestBase
    {
        private IKeyManager _keyManager;
        private string _expectedKey;
        private string _actualKey;
        private Guid _publicationId;
        private Guid _carId;

        protected override void Arrange()
        {
            _keyManager = new KeyManager();

            _publicationId = Guid.NewGuid();
            _carId = Guid.NewGuid();

            _expectedKey = "publication/" + _publicationId + "/car/" + _carId + "/assets/car-equipment/default";
        }

        protected override void Act()
        {
            _actualKey = _keyManager.GetDefaultCarEquipmentAssetsKey(_publicationId, _carId);
        }

        [Fact]
        public void ThenTheKeyShouldBeCorrect()
        {
            _actualKey.Should().Be(_expectedKey);
        } 
    }
}