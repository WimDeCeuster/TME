using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.CommandServices
{
    public interface ICarEquipmentService
    {
        Task PutCarEquipment(String brand, String country, Guid publicationID, Guid carID, CarEquipment carEquipment);
    }
}