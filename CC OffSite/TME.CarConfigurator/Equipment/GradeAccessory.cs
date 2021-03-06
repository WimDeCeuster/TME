﻿using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Equipment
{
    public class GradeAccessory : GradeEquipmentItem<Repository.Objects.Equipment.GradeAccessory>, IGradeAccessory
    {
        public GradeAccessory(Repository.Objects.Equipment.GradeAccessory repositoryAccessory, Publication publication, Context context, IColourFactory colourFactory)
            : base(repositoryAccessory)
        {

        }
    }
}
