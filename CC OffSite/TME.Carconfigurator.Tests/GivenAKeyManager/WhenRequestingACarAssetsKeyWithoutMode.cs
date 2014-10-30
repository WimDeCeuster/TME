using System;
using FluentAssertions;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAKeyManager
{
    public class WhenRequestingACarAssetsKeyWithoutMode : TestBase
    {
        private IKeyManager _keyManager;
        private string _expectedKey;
        private string _actualKey;
        private Guid _publicationId;
        private Guid _carId;
        private Guid _objectId;
        private string _view;
        private string _mode;

        protected override void Arrange()
        {
            _keyManager = new KeyManager();

            _publicationId = Guid.NewGuid();
            _carId = Guid.NewGuid();
            _objectId = Guid.NewGuid();

            _view = "a view";
            _mode = "";

            _expectedKey = "publication/" + _publicationId + "/car/" + _carId + "/assets/" + _objectId + "/" + _view;
        }

        protected override void Act()
        {
            _actualKey = _keyManager.GetAssetsKey(_publicationId, _carId, _objectId, _view, _mode);
        }

        [Fact]
        public void ThenTheKeyShouldBeCorrect()
        {
            _actualKey.Should().Be(_expectedKey);
        }
    }
}