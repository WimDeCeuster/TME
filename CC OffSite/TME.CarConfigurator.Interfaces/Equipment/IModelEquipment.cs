using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface IModelEquipment
    {
        /* NEAR FUTURE Enhancement
         * 
        IReadOnlyList<IModelAccessory> Accessories { get; }
        IReadOnlyList<IModelOption> Options { get; }
         */
        IReadOnlyList<ICategory> Categories { get; }
    }
}
