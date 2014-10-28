using System;
using FluentAssertions;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAKeyManager
{
    public class WhenRequestingAGradePacksKey : TestBase
    {
        private IKeyManager _keyManager;
        private string _expectedKey;
        private string _actualKey;
        private Guid _publicationId;
        private Guid _timeFrameId;
        private Guid _gradeId;

        protected override void Arrange()
        {
            _keyManager = new KeyManager();

            _publicationId = Guid.NewGuid();
            _timeFrameId = Guid.NewGuid();
            _gradeId = Guid.NewGuid();

            _expectedKey = "publication/" + _publicationId + "/time-frame/" + _timeFrameId + "/grade/" + _gradeId + "/packs";
        }

        protected override void Act()
        {
            _actualKey = _keyManager.GetGradePacksKey(_publicationId, _timeFrameId, _gradeId);
        }

        [Fact]
        public void ThenTheKeyShouldBeCorrect()
        {
            _actualKey.Should().Be(_expectedKey);
        }
    }
}