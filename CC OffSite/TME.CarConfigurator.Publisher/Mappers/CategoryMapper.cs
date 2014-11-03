using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class CategoryMapper : ICategoryMapper
    {
        IBaseMapper _baseMapper;

        public CategoryMapper(IBaseMapper baseMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");

            _baseMapper = baseMapper;
        }

        public CategoryInfo MapEquipmentCategoryInfo(Administration.EquipmentCategoryInfo categoryInfo, Administration.EquipmentCategories categories)
        {
            var category = categories.Find(categoryInfo.ID);

            return new CategoryInfo
            {
                ID = category.ID,
                Path = GetPath(category).ToLowerInvariant(),
                SortIndex = GetSortIndex(category)
            };
        }

        public Category MapEquipmentCategory(Administration.EquipmentCategory category)
        {
            var mappedCategory = new Category
            {
                ParentCategoryID = category.ParentCategory == null ? null : (Guid?)category.ParentCategory.ID,
                Path = GetPath(category).ToLowerInvariant(),
                SortIndex = GetSortIndex(category)
            };

            return _baseMapper.MapDefaults(mappedCategory, category, category);
        }

        /// <summary>
        /// Get the translated path
        /// </summary>
        static String GetPath(Administration.EquipmentCategory category)
        {
            var name = category.Translation.Name.DefaultIfEmpty(category.Name) ;
            var parentPath = category.ParentCategory == null ? "" : GetPath(category.ParentCategory);
            return String.IsNullOrWhiteSpace(parentPath) ? name : String.Format("{0}/{1}", GetPath(category.ParentCategory), name);
        }

        /// <summary>
        /// Get the flattened index
        /// </summary>
        static Int32 GetSortIndex(Administration.EquipmentCategory category)
        {
            var siblingDescendantCount = GetPreviousSiblings(category).Sum(sibling => GetDescendantCount(sibling));

            var index = category.Index + siblingDescendantCount;

            return category.ParentCategory == null ? index : index + GetSortIndex(category.ParentCategory) + 1;
        }

        static IEnumerable<Administration.EquipmentCategory> GetPreviousSiblings(Administration.EquipmentCategory category)
        {
            if (category.ParentCategory == null)
                return new List<Administration.EquipmentCategory>();
            else
                return category.ParentCategory.Categories.Where(sibling => sibling.Index < category.Index);
        }

        static Int32 GetDescendantCount(Administration.EquipmentCategory category)
        {
            return category.Categories.Count + category.Categories.Sum(child => GetDescendantCount(child));
        }
    }
}
