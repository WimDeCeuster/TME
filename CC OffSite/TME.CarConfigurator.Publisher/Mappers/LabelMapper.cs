using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class LabelMapper : ILabelMapper
    {
        public Label MapLabel(Administration.Translations.Label label)
        {
            return new Label
            {
                Code = label.Definition.Code,
                Value = label.Value
            };
        }
    }
}
