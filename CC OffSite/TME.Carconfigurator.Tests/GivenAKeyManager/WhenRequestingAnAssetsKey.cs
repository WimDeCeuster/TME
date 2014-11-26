using System;
using FluentAssertions;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAKeyManager
{
    public class WhenRequestingAnAssetsKey : TestBase
    {
        private IKeyManager _keyManager;
        private string _expectedKey;
        private string _actualKey;
        private Guid _publicationId;
        private Guid _objectId;
        private string _view;
        private string _mode;

        protected override void Arrange()
        {
            _keyManager = new KeyManager();

            _publicationId = Guid.NewGuid();
            _objectId = Guid.NewGuid();

            _view = "a view";
            _mode = "a mode";

            _expectedKey = "publication/" + _publicationId + "/assets/" + _objectId + "/" + _view + "/" + _mode + ".json";
        }

        protected override void Act()
        {
            _actualKey = _keyManager.GetAssetsKey(_publicationId, _objectId, _view, _mode);
        }

        [Fact]
        public void ThenTheKeyShouldBeCorrect()
        {
            _actualKey.Should().Be(_expectedKey);
        }
    }
}