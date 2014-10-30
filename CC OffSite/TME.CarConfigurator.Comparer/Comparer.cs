using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

            var config = new ComparisonConfig();
            config.IgnoreObjectTypes = true;
            config.InterfaceMembers = System.Reflection.Assembly.GetAssembly(typeof(IModel))
                       .GetTypes()
                       .Where(t => t.IsInterface)
                       .ToList();

            config.MembersToIgnore = GetMemberIgnoreNames(
                model => model.Assets.First().Hash,
                model => model.Engines.First().Type.Name,
                model => model.Engines.First().Type.Code
            );

            var comparer = new CompareLogic(config);

            var compareResult = comparer.Compare(modelA, modelB);

            result.Result = compareResult.DifferencesString;

            return result;
        }

        List<String> GetMemberIgnoreNames(params Expression<Func<IModel, object>>[] expressions)
        {
            return expressions.Select(expression =>
            {
                var member = (expression.Body as MemberExpression).Member;

                return member.DeclaringType.FullName + "." + member.Name;
            }).ToList();
        }
    }
}
