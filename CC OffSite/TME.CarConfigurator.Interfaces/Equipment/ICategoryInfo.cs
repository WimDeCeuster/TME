using System;

namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface ICategoryInfo
    {
        Guid ID { get; }
        String Path { get; }
        int SortIndex { get; }
    }
}
