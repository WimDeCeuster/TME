using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ILabelMapper
    {
        Label MapLabel(Administration.Translations.Label label);
        List<Label> MapLabels(IEnumerable<Administration.Translations.Label> label);
    }
}
