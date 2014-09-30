using TME.CarConfigurator.Publisher.Factory.Assets;

namespace TME.CarConfigurator.Publisher.Factory
{
    public static class Engine
    {
        public static Repository.Objects.Engine Create(Administration.Engine engine, Administration.ModelGenerationEngine generationEngine)
        {
            var result = new Repository.Objects.Engine
            {
                ID = generationEngine.ID,
                InternalCode = engine.BaseCode,
                LocalCode = engine.LocalCode,
                Name = generationEngine.Translation.Name,
                Description = generationEngine.Translation.Description,
                FootNote = generationEngine.Translation.FootNote,
                ToolTip = generationEngine.Translation.ToolTip,
                SortIndex = generationEngine.Index,

                Brochure =  generationEngine.Brochure,
                KeyFeature = generationEngine.KeyFeature,
                VisibleIn = VisibleInModeAndView.CreateList(generationEngine.AssetSet)
                

            };
            return result;
        }
    }
}
