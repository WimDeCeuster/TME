using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ILabelMapper
    {
        IReadOnlyList<Label> MapLabels(params IEnumerable<Administration.Translations.Label>[] labelSets);
    }
}