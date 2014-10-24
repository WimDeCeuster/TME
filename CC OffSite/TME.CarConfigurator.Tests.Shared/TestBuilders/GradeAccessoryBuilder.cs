using System;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Equipment;

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

        public GradeAccessory Build()
        {
            return _gradeAccessory;
        }
    }
}