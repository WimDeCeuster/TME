using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Comparer.Interfaces;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator.Comparer
{
    public class Comparer : IComparer
    {

        public ICompareResult Compare(IModel modelA, IModel modelB)
        {
            var result = new CompareResult();

            //Compare(modelA, modelB, "root", result);

            var config = new ComparisonConfig();
            config.IgnoreObjectTypes = true;
            config.InterfaceMembers = System.Reflection.Assembly.GetAssembly(typeof(IModel))
                       .GetTypes()
                       .Where(t => t.IsInterface)
                       .ToList();

            //config.max

            var comparer = new CompareLogic(config);

            var compareResult = comparer.Compare(modelA, modelB);

            result.Result = compareResult.DifferencesString;


            return result;
        }

        //Boolean Compare(object objectA, object objectB, String path, ICompareResult result)
        //{
        //    //var typeA = 
        //
        //    var isEqual = (objectA == null && objectB == null) || (objectA != null && objectA.Equals(objectB));
        //
        //    if (!isEqual)
        //        isEqual = (objectA == null && objectB != null) || (objectA != null && objectB == null);
        //    
        //    if (!isEqual)
        //        isEqual = objectA.GetType(objectA).GetProperties().All(property =>
        //        {
        //            var newPath = path + "." + property.Name;
        //            try
        //            {
        //                var valueA = property.GetValue(objectA);
        //                var valueB = property.GetValue(objectB);
        //
        //                return Compare(valueA, valueB, newPath, result);
        //            }
        //            catch (Exception ex)
        //            {
        //                result.Paths.Add(newPath, false);
        //                return false;
        //            }
        //        });
        //
        //    result.Paths.Add(path, allEqual);
        //    return allEqual;
        //}

    }
}
