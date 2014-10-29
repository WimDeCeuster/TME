using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.QueryServices
{
    public interface IColourService
    {
        IEnumerable<ColourCombination> GetColourCombinations(Guid publicationId, Guid publicationTimeFrameId, Context context);
    }
}
