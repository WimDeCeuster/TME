using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.S3.Shared.Result;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IS3LanguageService
    {
        Languages GetModelsOverviewPerLanguage(IContext context);
        Task<Result> PutModelsOverviewPerLanguage(IContext context, Languages languages);
    }
}
