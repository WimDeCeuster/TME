using System;
using FluentAssertions;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAKeyManager
{
    public class WhenRequestingACarPartsKey : TestBase
    {
        private IKeyManager _keyManager;
        private string _expectedKey;
        private string _actualKey;
        private Guid _publicationId;
        private Guid _carID;

        protected override void Arrange()
        {
            _keyManager = new KeyManager();

            _publicationId = Guid.NewGuid();
            _carID = Guid.NewGuid();

            _expectedKey = "publication/" + _publicationId + "/car/" + _carID + "/carparts";
        }

        protected override void Act()
        {
            _actualKey = _keyManager.GetCarCarPartsKey(_publicationId,  _carID);
        }

        [Fact]
        public void ThenTheKeyShouldBeCorrect()
        {
            _actualKey.Should().Be(_expectedKey);
        }
    }
}