using System;
using FluentAssertions;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAKeyManager
{
    public class WhenRequestingASubModelGradesKey : TestBase
    {
        private IKeyManager _keyManager;
        private string _expectedKey;
        private string _actualKey;
        private Guid _publicationID;
        private Guid _timeFrameID;
        private Guid _submodelID;

        protected override void Arrange()
        {
            _keyManager = new KeyManager();

            _publicationID = Guid.NewGuid();
            _submodelID = Guid.NewGuid();
            _timeFrameID = Guid.NewGuid();

            _expectedKey = "publication/" + _publicationID + "/time-frame/" + _timeFrameID + "/submodel/" + _submodelID + "/grades";
        }

        protected override void Act()
        {
            _actualKey = _keyManager.GetSubModelGradesKey(_publicationID, _timeFrameID, _submodelID);
        }

        [Fact]
        public void ThenTheKeyShouldBeCorrect()
        {
            _actualKey.Should().Be(_expectedKey);
        }  
    }
}