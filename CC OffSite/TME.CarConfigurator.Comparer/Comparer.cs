using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Comparer.Interfaces;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Comparer
{
    public class Comparer : IComparer
    {
        public ICompareResult Compare(object modelA, object modelB)
        {
            var config = new ComparisonConfig();
            config.IgnoreObjectTypes = true;
            config.InterfaceMembers = System.Reflection.Assembly.GetAssembly(typeof(IModel))
                       .GetTypes()
                       .Where(t => t.IsInterface)
                       .ToList();

            config.MembersToIgnore = GetFullMemberNames(
                model => model.Assets.First().Hash,
                model => model.Engines.First().Type.Name,
                model => model.Engines.First().Type.Code,
                model => model.ColourCombinations.First().ID,
                model => model.ColourCombinations.First().SortIndex,
                model => model.Grades.First().Equipment.Options.First().TechnologyItem
                //model => model.Assets,
                //model => model.BodyTypes,
                //model => model.Brand,
                //model => model.CarConfiguratorVersion,
                //model => model.Cars,
                //model => model.ColourCombinations,
                //model => model.Description,
                //model => model.Engines,
                //model => model.Equipment,
                //model => model.FootNote,
                //model => model.FuelTypes,
                //model => model.Grades,
                //model => model.ID,
                //model => model.InternalCode,
                //model => model.Labels,
                //model => model.Links,
                //model => model.LocalCode,
                //model => model.Name,
                //model => model.Promoted,
                //model => model.SortIndex,
                //model => model.SSN,
                //model => model.Steerings,
                //model => model.SubModels,
                //model => model.TechnicalSpecifications,
                //model => model.ToolTip,
                //model => model.Transmissions,
                //model => model.WheelDrives
            );

            config.PathsToIgnore = new List<String>
            { 
                ".Grades[].Equipment.Accessories[].ExteriorColour.SortIndex",
                ".Grades[].Equipment.Options[].ExteriorColour.SortIndex",
                ".Cars[].Grade.Equipment.Accessories[].ExteriorColour.SortIndex",
                ".Cars[].Grade.Equipment.Options[].ExteriorColour.SortIndex",
                ".SubModels[].Grades[].Equipment.Accessories[].ExteriorColour.SortIndex",
                ".SubModels[].Grades[].Equipment.Options[].ExteriorColour.SortIndex"
            };

            config.IgnoreOrderFor = new List<String>
            {
                GetFullMemberName<IBodyType>(bodyType => bodyType.VisibleIn),
                //GetFullMemberName<IEngine>(engine => engine.VisibleIn),
                //GetFullMemberName<IGrade>(grade => grade.VisibleIn),
                //GetFullMemberName<ITransmission>(transmission => transmissions.VisibleIn),
                //GetFullMemberName<IWheelDrive>(wheelDrive => wheelDrive.VisibleIn)
                GetFullMemberName<IUpholstery>(upholstery => upholstery.VisibleIn)
            };

            Func<object, object, bool> invariantCompare = (item1, item2) => String.Equals((string)item1, (string)item2, StringComparison.InvariantCultureIgnoreCase);

            config.CustomPropertyComparers = new Dictionary<String, Func<object, object, bool>> {
                { GetFullMemberName<TME.CarConfigurator.Interfaces.Equipment.IEquipmentItem>(item => item.Path), invariantCompare },
                { GetFullMemberName<TME.CarConfigurator.Interfaces.Equipment.ICategory>(item => item.Path), invariantCompare },
                { GetFullMemberName<TME.CarConfigurator.Interfaces.Equipment.ICategoryInfo>(item => item.Path), invariantCompare },
                { GetFullMemberName<TME.CarConfigurator.Interfaces.TechnicalSpecifications.ICategory>(item => item.Path), invariantCompare },
                { GetFullMemberName<TME.CarConfigurator.Interfaces.TechnicalSpecifications.ICategoryInfo>(item => item.Path), invariantCompare }
            };

            //config.CollectionMatchingSpec = new Dictionary<Type, IEnumerable<string>> {
            //    { typeof(IBaseObject), new [] { GetMemberName<IBaseObject>(o => o.ID) } },
            //    { typeof(IVisibleInModeAndView), new [] {  } }
            //};

            config.CollectionMatchingKey = new Dictionary<Type, Func<object, string>> {
                { typeof(IBaseObject), o => ((IBaseObject)o).ID.ToString() },
                { typeof(IVisibleInModeAndView), o => ((IVisibleInModeAndView)o).View + "|" + ((IVisibleInModeAndView)o).Mode },
                { typeof(IAsset), o => {
                    var asset = o as IAsset; 
                    return String.Format("{0}-{1}-{2}-{3}-{4}", asset.ID, asset.AssetType.Code, asset.AssetType.ExteriorColourCode, asset.AssetType.UpholsteryCode, asset.AssetType.EquipmentCode);
                } },
                { typeof(IColourCombination), o => {
                    var colourCombination = o as IColourCombination;
                    return String.Format("{0}-{1}", colourCombination.ExteriorColour.ID, colourCombination.Upholstery.ID);
                } }
            };

            config.DetectMissingAndMisordered = true;

            var rootComparer = RootComparerFactory.GetRootComparer();

            //config.CustomComparers = new List<BaseTypeComparer> { new MyComparer(rootComparer) };



            //var differences = new StringBuilder();
            //
            //config.DifferenceCallback = difference => differences.AppendLine(String.Format(
            //    "{0} : {1} : {2} : {3}",
            //    difference.ActualName, difference.ExpectedName, difference.ParentPropertyName, difference.PropertyName
            //    ));

            config.MaxDifferences = -1;
            config.MaxStructDepth = 2;

            config.AllowPropertyExceptions = true;

            config.CustomStringifier = obj =>
            {
                return obj is IBaseObject ? ((IBaseObject)obj).ID.ToString() : null;
            };

            //config.CustomComparers = new[] { };

            //config.ShowBreadcrumb = true;

            var comparer = new CompareLogic(config);

            //var x1 = new MyClass { Items = new List<String> { "a", "B", "c", "D", "e" } };
            ////var x2 = new MyClass { Linked = x1, Items = new List<String> { "a", "c", "b" } };
            ////x1.Linked = x2;
            //
            //var y1 = new MyClass { Items = new List<String> { "a", "b", "c", "d", "e" } };
            ////var y2 = new MyClass { Items = new List<String> { "a", "c", "b" } };
            ////var y3 = new MyClass { Linked = y2, Items = new List<String> { "a", "b", "c" } };
            ////y1.Linked = y2;
            ////y2.Linked = y3;

            var compareResult = comparer.Compare(modelA, modelB);

            var result = new CompareResult(compareResult.Differences);

            return result;
        }

        String ToDifferenceString(Difference difference)
        {
            switch (difference.Type)
            {
                case DifferenceType.Mismatch:
                    return "Mismatch: " + difference.ToString();
                case DifferenceType.Missing:
                    return "Missing: " + difference.ToString();
                case DifferenceType.Misorder:
                    return "Misorder: " + difference.ToString();
                case DifferenceType.Exception:
                    return String.Format("Exception on path {0}", difference.ToString());
                case DifferenceType.NotImplemented:
                    return String.Format("Not implemented exception on path {0}", difference.MemberPath);
                default:
                    return "Unrecognised difference: " + difference.ToString();
            }
        }

        List<String> GetFullMemberNames(params Expression<Func<IModel, object>>[] expressions)
        {
            return expressions.Select(GetFullMemberName).ToList();
        }

        String GetFullMemberName<T>(Expression<Func<T, object>> expression)
        {
            var member = GetMember(expression);
            return member.DeclaringType.FullName + "." + member.Name;
        }

        String GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            var member = GetMember(expression);
            return member.Name;
        }

        MemberInfo GetMember<T>(Expression<Func<T, object>> expression)
        {
            var memberExpression = expression.Body is MemberExpression ? expression.Body as MemberExpression :
                       ((UnaryExpression)expression.Body).Operand as MemberExpression;

            return memberExpression.Member;
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
