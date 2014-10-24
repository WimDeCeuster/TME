using System;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class SubModelBuilder
    {
        private readonly SubModel _subModel;

        public SubModelBuilder()
        {
            _subModel = new SubModel();
        }

        public SubModelBuilder WithID(Guid ID)
        {
            _subModel.ID = ID;
            return this;
        }

        public SubModelBuilder WithLabels(params Label[] labels)
        {
            _subModel.Labels = labels.ToList();
            return this;
        }

        public SubModelBuilder WithLinks(params Link[] links)
        {
            _subModel.Links = links.ToList();
            return this;
        }

        public SubModelBuilder WithStartingPrice(Price repoPrice)
        {
            _subModel.StartingPrice = repoPrice;
            return this;
        }

        public SubModelBuilder WithAssets(params Asset[] assets)
        {
            _subModel.Assets = assets.ToList();
            return this;
        }


        public SubModelBuilder WithGrades(params Grade[] grades)
        {
            _subModel.Grades = grades.ToList();
            return this;
        }

        public SubModel Build()
        {
            return _subModel;
        }
    }
}