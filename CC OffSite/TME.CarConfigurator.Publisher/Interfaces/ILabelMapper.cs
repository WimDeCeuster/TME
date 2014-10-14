using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ILabelMapper
    {
        Label MapLabel(TME.CarConfigurator.Administration.Translations.Label label);
    }
}
