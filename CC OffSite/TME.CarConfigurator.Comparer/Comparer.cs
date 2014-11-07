using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;
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

            var rootComparer = RootComparerFactory.GetRootComparer();

            config.CustomComparers = new List<BaseTypeComparer> { new MyComparer(rootComparer) };

            //var differences = new StringBuilder();
            //
            //config.DifferenceCallback = difference => differences.AppendLine(String.Format(
            //    "{0} : {1} : {2} : {3}",
            //    difference.ActualName, difference.ExpectedName, difference.ParentPropertyName, difference.PropertyName
            //    ));

            config.MaxDifferences = 20;
            config.MaxStructDepth = 20;

            //config.CustomComparers = new[] { };

            var comparer = new CompareLogic(config);

            var x1 = new MyClass { Items = new List<String> { "a", "B", "c", "D", "e" } };
            //var x2 = new MyClass { Linked = x1, Items = new List<String> { "a", "c", "b" } };
            //x1.Linked = x2;

            var y1 = new MyClass { Items = new List<String> { "a", "b", "c", "d", "e" } };
            //var y2 = new MyClass { Items = new List<String> { "a", "c", "b" } };
            //var y3 = new MyClass { Linked = y2, Items = new List<String> { "a", "b", "c" } };
            //y1.Linked = y2;
            //y2.Linked = y3;

            var compareResult = comparer.Compare(x1, y1);

            result.Result = compareResult.DifferencesString;// +"\n" + differences.ToString();

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

    public class MyComparer: KellermanSoftware.CompareNetObjects.TypeComparers.BaseTypeComparer
    {
        public MyComparer(RootComparer rootComparer)
            : base(rootComparer)
        {

        }

        public override bool IsTypeMatch(Type type1, Type type2)
        {
            //return type1.isa
            return false;
            //return true;//throw new NotImplementedException();
        }

        public override void CompareType(CompareParms parms)
        {
            //var z = new PropertyComparer(RootComparer);
            
            this.RootComparer.Compare(parms);
            //throw new NotImplementedException();
        }
    }

    public class MyClass
    {
        public MyClass Linked { get; set; }
        public List<String> Items { get; set; }
    }
}
