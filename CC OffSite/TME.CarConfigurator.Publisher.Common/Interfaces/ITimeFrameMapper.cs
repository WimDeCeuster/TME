using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher.Common.Interfaces
{
    public interface ITimeFrameMapper
    {
        IReadOnlyList<TimeFrame> GetTimeFrames(String language, IContext context);
    }
}
