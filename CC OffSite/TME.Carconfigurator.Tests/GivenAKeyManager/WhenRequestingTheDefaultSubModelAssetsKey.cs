using System;
using FluentAssertions;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAKeyManager
{
    public class WhenRequestingTheDefaultSubModelAssetsKey : TestBase
    {
        private IKeyManager _keyManager;
        private string _expectedKey;
        private string _actualKey;
        private Guid _publicationId;
        private Guid _objectId;
        private Guid _subModelID;

        protected override void Arrange()
        {
            _keyManager = new KeyManager();

            _publicationId = Guid.NewGuid();
            _subModelID = Guid.NewGuid();
            _objectId = Guid.NewGuid();

            _expectedKey = "publication/" + _publicationId + "/submodel/" + _subModelID + "/assets/" + _objectId + "/default";
        }

        protected override void Act()
        {
            _actualKey = _keyManager.GetDefaultSubModelAssetsKey(_publicationId, _subModelID, _objectId);
        }

        [Fact]
        public void ThenTheKeyShouldBeCorrect()
        {
            _actualKey.Should().Be(_expectedKey);
        } 
    }
}