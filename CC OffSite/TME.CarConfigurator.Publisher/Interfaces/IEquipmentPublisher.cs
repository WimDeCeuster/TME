using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IEquipmentPublisher
    {
        Task<IEnumerable<Result>> PublishAsync(IContext context);
        Task<IEnumerable<Result>> PublishCategoriesAsync(IContext context);
        Task<IEnumerable<Result>> PublishSubModelGradeEquipmentAsync(IContext context);
    }
}
