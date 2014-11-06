using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Core;
using AdministrationLabel = TME.CarConfigurator.Administration.Translations.Label;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class LabelMapper : ILabelMapper
    {
        public IReadOnlyList<Label> MapLabels(params IEnumerable<AdministrationLabel>[] labelSets)
        {
            return labelSets.Select(NotEmpty).Aggregate(MergeLabels).Select(MapLabel).ToList();
        }
        
        static Label MapLabel(AdministrationLabel label)
        {
            return new Label()
                {
                    Code = label.Definition.Code, 
                    Value = label.Value
                };
        }

        static IReadOnlyList<AdministrationLabel> NotEmpty(IEnumerable<AdministrationLabel> labels)
        {
            return labels.Where(label => !string.IsNullOrWhiteSpace(label.Value)).ToList();
        }

        static IReadOnlyList<AdministrationLabel> MergeLabels(IReadOnlyList<AdministrationLabel> priorityLabels, IReadOnlyList<AdministrationLabel> backupLabels)
        {
            var priorityCodes = priorityLabels.Select(label => label.Definition.Code).ToList();
            var backupCodes = backupLabels.Select(label => label.Definition.Code).ToList();
            var missingCodes = backupCodes.Except(priorityCodes, StringComparer.InvariantCultureIgnoreCase);
            var missingLabels = missingCodes.Select(code => backupLabels.Single(label => String.Equals(code, label.Definition.Code, StringComparison.InvariantCultureIgnoreCase)));

            return priorityLabels.Concat(missingLabels).ToList();
        }
    }
}
