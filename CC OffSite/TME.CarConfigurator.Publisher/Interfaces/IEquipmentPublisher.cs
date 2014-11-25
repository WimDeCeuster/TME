using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;


namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IEquipmentPublisher
    {
        Task PublishAsync(IContext context);
        Task PublishCategoriesAsync(IContext context);
        Task PublishSubModelGradeEquipmentAsync(IContext context);
        Task PublishCarEquipmentAsync(IContext context);
    }
}
