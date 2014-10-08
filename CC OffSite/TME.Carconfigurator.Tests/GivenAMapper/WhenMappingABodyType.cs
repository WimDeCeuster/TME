using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Tests.Shared;
using Xunit;
using FluentAssertions;

namespace TME.Carconfigurator.Tests.GivenAMapper
{
    public class WhenMappingABodyType : MappingTestBase
    {
        Mapper _mapper;
        ModelGenerationBodyType _bodyType;

        protected override void Arrange()
        {
            base.Arrange();

            _bodyType = Generation.BodyTypes.First();
        }

        protected override bool Filter(Model model, ModelGeneration generation)
        {
            return generation.BodyTypes.Any(bodyType => bodyType.AssetSet.Assets.Any(asset => {
                var detail = asset.AssetType.Details;
                return !String.IsNullOrWhiteSpace(detail.Mode) && !String.IsNullOrWhiteSpace(detail.View);
            }));
        }

        [Fact]
        public void ThenABodyTypeShouldMatch()
        {
            var bodyType = Context.ContextData[Language].GenerationBodyTypes.Single(bt => bt.ID == _bodyType.ID);

            bodyType.Description.ShouldBeEquivalentTo(_bodyType.Translation.Description);
            bodyType.FootNote.ShouldBeEquivalentTo(_bodyType.Translation.FootNote);
            bodyType.InternalCode.ShouldBeEquivalentTo(null);
            bodyType.LocalCode.ShouldBeEquivalentTo(null);
            bodyType.Name.Should().BeOneOf(_bodyType.Translation.Name, _bodyType.Name);
            bodyType.NumberOfDoors.ShouldBeEquivalentTo(_bodyType.NumberOfDoors);
            bodyType.NumberOfSeats.ShouldBeEquivalentTo(_bodyType.NumberOfSeats);
            bodyType.SortIndex.ShouldBeEquivalentTo(0);
            bodyType.ToolTip.ShouldBeEquivalentTo(_bodyType.Translation.ToolTip);

            //bodyType.VisibleIn.Should().NotBeEmpty();
        }
    }
}
