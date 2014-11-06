using System;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Repository.Objects.Equipment;
using ExteriorColour = TME.CarConfigurator.Repository.Objects.Equipment.ExteriorColour;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class GradeAccessoryBuilder
    {
        private readonly GradeAccessory _gradeAccessory;

        public GradeAccessoryBuilder()
        {
            _gradeAccessory = new GradeAccessory();
        }

        public GradeAccessoryBuilder WithId(Guid id)
        {
            _gradeAccessory.ID = id;

            return this;
        }

        public GradeAccessoryBuilder WithCategory(CategoryInfo category)
        {
            _gradeAccessory.Category = category;

            return this;
        }

        public GradeAccessoryBuilder WithColour(ExteriorColour colour)
        {
            _gradeAccessory.ExteriorColour = colour;

            return this;
        }

        public GradeAccessoryBuilder WithLabels(params Label[] labels)
        {
            _gradeAccessory.Labels = labels.ToList();

            return this;
        }

        public GradeAccessoryBuilder WithLinks(params Link[] links)
        {
            _gradeAccessory.Links = links.ToList();

            return this;
        }

        public GradeAccessoryBuilder WithNotAvailableOn(params CarInfo[] carInfo)
        {
            _gradeAccessory.NotAvailableOn = carInfo.ToList();

            return this;
        }

        public GradeAccessoryBuilder WithOptionalOn(params CarInfo[] carInfo)
        {
            _gradeAccessory.OptionalOn = carInfo.ToList();

            return this;
        }

        public GradeAccessoryBuilder WithStandardOn(params CarInfo[] carInfo)
        {
            _gradeAccessory.StandardOn = carInfo.ToList();

            return this;
        }

        public GradeAccessory Build()
        {
            return _gradeAccessory;
        }
    }
}