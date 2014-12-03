using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TME.CarConfigurator.Comparer.Interfaces;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Interfaces.Rules;
using IEquipmentCategory = TME.CarConfigurator.Interfaces.Equipment.ICategory;
using IEquipmentCategoryInfo = TME.CarConfigurator.Interfaces.Equipment.ICategoryInfo;
using ISpecificationCategory = TME.CarConfigurator.Interfaces.TechnicalSpecifications.ICategory;
using ISpecificationCategoryInfo = TME.CarConfigurator.Interfaces.TechnicalSpecifications.ICategoryInfo;
using IEquipmentExteriorColour = TME.CarConfigurator.Interfaces.Equipment.IExteriorColour;

namespace TME.CarConfigurator.Comparer
{
    public class Comparer : Interfaces.IComparer
    {
        public ICompareResult Compare(object modelA, object modelB)
        {
            var config = new ComparisonConfig();
            config.IgnoreObjectTypes = true;
            config.InterfaceMembers = Assembly.GetAssembly(typeof(IModel))
                       .GetTypes()
                       .Where(t => t.IsInterface)
                       .ToList();

            config.ExpectedName = "Expected.Model";
            config.ActualName = "Actual.Model";

            config.MembersToIgnore = GetFullMemberNames(
                model => model.Assets.First().Hash,
                model => model.Engines.First().Type.Name,
                model => model.Engines.First().Type.Code,
                model => model.ColourCombinations.First().ID,
                model => model.ColourCombinations.First().SortIndex,
                model => model.Grades.First().Equipment.Options.First().TechnologyItem,
                model => model.Cars.First().TechnicalSpecifications.First().ValueFormat,
                model => model.Equipment.Categories.First().Parent, // fix circular reference
                model => model.TechnicalSpecifications.Categories.First().Parent, // fix circular reference
                model => model.Cars.First().Packs.First().Equipment.Accessories.First().KeyFeature,
                model => model.Cars.First().Packs.First().Equipment.Options.First().KeyFeature,
                model => model.Cars.First().Packs.First().Equipment.ExteriorColourTypes.First().KeyFeature,
                model => model.Cars.First().Packs.First().Equipment.UpholsteryTypes.First().KeyFeature
            );

            config.MembersToIgnore.Add(GetFullMemberName<IEquipmentExteriorColour>(c => c.SortIndex));
            config.MembersToIgnore.Add(GetFullMemberName<IRule>(c => c.Category));
            
            config.PathsToIgnore = new List<String>
            { 
                ".Grades[].Equipment.Accessories[].ExteriorColour.SortIndex",
                ".Grades[].Equipment.Options[].ExteriorColour.SortIndex",
                ".Cars[].Grade.Equipment.Accessories[].ExteriorColour.SortIndex",
                ".Cars[].Grade.Equipment.Options[].ExteriorColour.SortIndex",
                ".SubModels[].Grades[].Equipment.Accessories[].ExteriorColour.SortIndex",
                ".SubModels[].Grades[].Equipment.Options[].ExteriorColour.SortIndex",
                ".SubModels[].Grades[].Assets[]",
                ".ColourCombinations[].ExteriorColour.Price",
                ".ColourCombinations[].Upholstery.Price",
                ".ColourCombinations[].Upholstery.SortIndex",
                ".ColourCombinations[].ExteriorColour.SortIndex",
                ".ColourCombinations[].SortIndex",
                ".Cars[].ColourCombinations[].Upholstery.SortIndex",
                ".Cars[].ColourCombinations[].SortIndex",
                ".ColourCombinations[].ExteriorColour.Assets",
                ".ColourCombinations[].Upholstery.Assets"
            };

            config.IgnoreOrderFor = new List<String>
            {
                GetFullMemberName<IBodyType>(bodyType => bodyType.VisibleIn),
                GetFullMemberName<IEngine>(engine => engine.VisibleIn),
                GetFullMemberName<IUpholstery>(upholstery => upholstery.VisibleIn),
                GetFullMemberName<ICarEquipmentItem>(option => option.VisibleIn),
                GetFullMemberName<ICarPart>(part => part.VisibleIn),
                GetFullMemberName<ICarPack>(carPack => carPack.AvailableForExteriorColours),
                GetFullMemberName<ICarPack>(carPack => carPack.AvailableForUpholsteries),
                GetFullMemberName<ICarEquipmentItem>(item => item.AvailableForExteriorColours),
                GetFullMemberName<ICarEquipmentItem>(item => item.AvailableForUpholsteries),
                GetFullMemberName<IModel>(item => item.Assets),
                GetFullMemberName<ICarPackUpholsteryType>(type => type.ColourCombinations),
                GetFullMemberName<ICarPackExteriorColourType>(type => type.ColourCombinations),
                GetFullMemberName<IGradeEquipmentItem>(item => item.OptionalOn),
                GetFullMemberName<IGradeEquipmentItem>(item => item.NotAvailableOn),
                GetFullMemberName<IGradeEquipmentItem>(item => item.StandardOn),
                GetFullMemberName<ICarEquipmentItem>(item => item.Rules),
                GetFullMemberName<ICarPack>(pack => pack.Rules)
            };

            config.AllowMissingFor = new List<String>
            {
                GetFullMemberName<IBodyType>(bodyType => bodyType.VisibleIn),
                GetFullMemberName<ICarColourCombination>(colourCombination => colourCombination.VisibleIn),
                GetFullMemberName<IColourCombination>(colourCombination => colourCombination.VisibleIn),
                GetFullMemberName<CarConfigurator.Interfaces.Colours.IExteriorColour>(exteriorColour => exteriorColour.VisibleIn),
                GetFullMemberName<IUpholstery>(upholstery => upholstery.VisibleIn),
                GetFullMemberName<ICarEquipmentItem>(item => item.VisibleIn),
                GetFullMemberName<ICarPart>(carPart => carPart.VisibleIn),
                GetFullMemberName<IEngine>(engine => engine.VisibleIn),
                GetFullMemberName<IGrade>(grade => grade.VisibleIn),
                GetFullMemberName<ITransmission>(transmission => transmission.VisibleIn),
                GetFullMemberName<IWheelDrive>(wheelDrive => wheelDrive.VisibleIn),
                GetFullMemberName<IGrade>(grade => grade.VisibleIn),
                GetFullMemberName<IGrade>(grade => grade.VisibleIn),
                GetFullMemberName<IGrade>(grade => grade.VisibleIn),
                
            };

            Func<object, object, bool?> ignoreCase = (item1, item2) => String.Equals((string)item1, (string)item2, StringComparison.InvariantCultureIgnoreCase);
            Func<object, object, bool?> emptyIsValid = (item1, item2) => ((string)item1) == string.Empty ? true : (bool?)null;

            config.CustomPropertyComparers = new Dictionary<String, Func<object, object, bool?>> {
                { GetFullMemberName<IEquipmentItem>(item => item.Path), ignoreCase },
                { GetFullMemberName<IEquipmentCategory>(item => item.Path), ignoreCase },
                { GetFullMemberName<IEquipmentCategoryInfo>(item => item.Path), ignoreCase },
                { GetFullMemberName<ISpecificationCategory>(item => item.Path), ignoreCase },
                { GetFullMemberName<ISpecificationCategoryInfo>(item => item.Path), ignoreCase },
                { GetFullMemberName<IEquipmentExteriorColour>(item => item.Name), emptyIsValid }
            };

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
                } },
                { typeof(IUpholsteryInfo), o => ((IUpholsteryInfo)o).ID.ToString()},
                { typeof(IExteriorColourInfo), o => ((IExteriorColourInfo)o).ID.ToString()},
                { typeof(IColourCombinationInfo), o => ((IColourCombinationInfo)o).ExteriorColour.ID.ToString() + "|" + ((IColourCombinationInfo)o).Upholstery.ID.ToString() },
                { typeof(ICarInfo), o => ((ICarInfo)o).ShortID.ToString() },
                { typeof(IRule), o => ((IRule)o).ShortID.ToString() }
            };

            config.DetectMissingAndMisordered = true;
            //config.QuickFailLists = true;

            config.ShowBreadcrumb = false;
            config.MaxDifferences = -1;
            config.MaxClassDepth = -1;

            config.AllowPropertyExceptions = true;

            config.CustomStringifier = obj => obj is IBaseObject ? ((IBaseObject)obj).ID.ToString() : null;

            var comparer = new CompareLogic(config);

            var compareResult = comparer.Compare(modelA, modelB);

            var result = new CompareResult(compareResult.Differences);

            return result;
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

        MemberInfo GetMember<T>(Expression<Func<T, object>> expression)
        {
            var memberExpression = expression.Body is MemberExpression ? expression.Body as MemberExpression :
                       ((UnaryExpression)expression.Body).Operand as MemberExpression;

            return memberExpression.Member;
        }

    }
}
