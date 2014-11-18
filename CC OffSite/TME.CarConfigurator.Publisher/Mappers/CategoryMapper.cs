using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using EquipmentCategoryInfo = TME.CarConfigurator.Repository.Objects.Equipment.CategoryInfo;
using EquipmentCategory = TME.CarConfigurator.Repository.Objects.Equipment.Category;
using SpecificationCategory = TME.CarConfigurator.Repository.Objects.TechnicalSpecifications.Category;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class CategoryMapper : ICategoryMapper
    {
        readonly IBaseMapper _baseMapper;

        public CategoryMapper(IBaseMapper baseMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");

            _baseMapper = baseMapper;
        }

        public EquipmentCategoryInfo MapEquipmentCategoryInfo(Administration.EquipmentCategoryInfo categoryInfo, Administration.EquipmentCategories categories)
        {
            var category = categories.Find(categoryInfo.ID);

            return new EquipmentCategoryInfo
            {
                ID = category.ID,
                Path = GetPath(category, cat => cat.ParentCategory),
                SortIndex = GetSortIndex(category, cat => cat.ParentCategory, cat => cat.Categories)
            };
        }

        public EquipmentCategory MapEquipmentCategory(Administration.EquipmentCategory category)
        {
            var mappedCategory = new EquipmentCategory
            {
                ParentCategoryID = category.ParentCategory == null ? null : (Guid?)category.ParentCategory.ID,
                Path = GetPath(category, cat => cat.ParentCategory),
                SortIndex = GetSortIndex(category, cat => cat.ParentCategory, cat => cat.Categories)
            };

            return _baseMapper.MapDefaults(mappedCategory, category);
        }

        public SpecificationCategory MapSpecificationCategory(Administration.SpecificationCategory category)
        {
            var mappedCategory = new SpecificationCategory
            {
                ParentCategoryID = category.ParentCategory == null ? null : (Guid?)category.ParentCategory.ID,
                Path = GetPath(category, cat => cat.ParentCategory),
                SortIndex = GetSortIndex(category, cat => cat.ParentCategory, cat => cat.Categories) - 1
            };

            return _baseMapper.MapDefaults(mappedCategory, category);
        }

        /// <summary>
        /// Get the translated path
        /// </summary>
        static String GetPath<T>(T category, Func<T, T> getParent)
            where T : Administration.BaseObjects.TranslateableBusinessBase
        {
            var name = category.Translation.Name.DefaultIfEmpty(category.BaseName);
            var parent = getParent(category);
            var parentPath = parent == null ? "" : GetPath(parent, getParent);
            var fullPath = String.IsNullOrWhiteSpace(parentPath) ? name : String.Format("{0}/{1}", GetPath(parent, getParent), name);
            return fullPath.ToLowerInvariant();
        }

        /// <summary>
        /// Get the flattened index
        /// </summary>
        static Int32 GetSortIndex<T>(T category, Func<T, T> getParent, Func<T, IReadOnlyList<T>> getDescendants)
            where T : class, Administration.BaseObjects.ISortedIndex
        {
            var siblingDescendantCount = GetPreviousSiblings(category, getParent(category), getDescendants).Sum(sibling => GetDescendantCount(sibling, getDescendants));

            var index = category.Index + siblingDescendantCount;
            var parentCategory = getParent(category);

            return parentCategory == null ? index : index + GetSortIndex(parentCategory, getParent, getDescendants) + 1;
        }

        static IEnumerable<T> GetPreviousSiblings<T>(T category, T parent, Func<T, IReadOnlyList<T>> getDescendants)
            where T : class, Administration.BaseObjects.ISortedIndex 
        {
            return parent == null ? new List<T>() : getDescendants(parent).Where(sibling => sibling.Index < category.Index);
        }

        static Int32 GetDescendantCount<T>(T category, Func<T, IReadOnlyList<T>> getDescendants)
            where T : Administration.BaseObjects.ISortedIndex
        {
            var descendants = getDescendants(category);
            return descendants.Count + descendants.Sum(child => GetDescendantCount(child, getDescendants));
        }
    }
}
