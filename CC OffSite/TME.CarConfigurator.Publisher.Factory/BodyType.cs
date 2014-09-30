using TME.CarConfigurator.Publisher.Factory.Assets;

namespace TME.CarConfigurator.Publisher.Factory
{
    public static class BodyType
    {
        public static Repository.Objects.BodyType Create(Administration.BodyType bodyType, Administration.ModelGenerationBodyType generationBodyType)
        {
            var result = new Repository.Objects.BodyType
            {
                ID = generationBodyType.ID,
                InternalCode = bodyType.BaseCode,
                LocalCode = bodyType.LocalCode,
                Name = generationBodyType.Translation.Name,
                Description = generationBodyType.Translation.Description,
                FootNote = generationBodyType.Translation.FootNote,
                ToolTip = generationBodyType.Translation.ToolTip,
                SortIndex = generationBodyType.Index,

                NumberOfDoors = generationBodyType.NumberOfDoors,
                NumberOfSeats = generationBodyType.NumberOfSeats,
                VisibleIn = VisibleInModeAndView.CreateList(generationBodyType.AssetSet)
            };
            return result;
        }
    }
}
