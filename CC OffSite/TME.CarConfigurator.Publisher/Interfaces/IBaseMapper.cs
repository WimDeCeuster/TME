using System.Collections.Generic;
using TME.CarConfigurator.Administration.BaseObjects;
using TME.CarConfigurator.Administration.Translations;
namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IBaseMapper
    {
        T MapLocalizableDefaults<T>(T baseObject, LocalizeableBusinessBase localizableObject)
            where T : Repository.Objects.Core.BaseObject;

        T MapTranslateableDefaults<T>(T baseObject, TranslateableBusinessBase translateableObject, params IEnumerable<Label>[] additionalLabelSources)
            where T : Repository.Objects.Core.BaseObject;

        T MapTranslateableDefaultsWithSort<T, TTranslateableAndSortable>(T baseObject, TTranslateableAndSortable translateableAndSortableObject, params IEnumerable<Label>[] additionalLabelSources)
            where T : Repository.Objects.Core.BaseObject
            where TTranslateableAndSortable : TranslateableBusinessBase, ISortedIndex;
        
        T MapSortDefaults<T>(T baseObject, ISortedIndex sortableObject)
            where T : Repository.Objects.Core.BaseObject;

        T MapDefaults<T>(T baseObject, LocalizeableBusinessBase localizableObject, TranslateableBusinessBase translateableObject, params IEnumerable<Label>[] additionalLabelSources)
            where T : Repository.Objects.Core.BaseObject;

        T MapDefaults<T>(T baseObject, LocalizeableBusinessBase localizableObject, params IEnumerable<Label>[] additionalLabelSources)
            where T : Repository.Objects.Core.BaseObject;
        
        T MapDefaultsWithSort<T, TTranslateableAndSortable>(T baseObject, LocalizeableBusinessBase localizableObject, TTranslateableAndSortable translateableAndSortableObject, params IEnumerable<Label>[] additionalLabelSources)
            where T : Repository.Objects.Core.BaseObject
            where TTranslateableAndSortable : TranslateableBusinessBase, ISortedIndex;

        T MapDefaultsWithSort<T, TLocalizeableAndSortable>(T baseObject, TLocalizeableAndSortable localizeableAndSortableObject, params IEnumerable<Label>[] additionalLabelSources)
            where T : Repository.Objects.Core.BaseObject
            where TLocalizeableAndSortable : LocalizeableBusinessBase, ISortedIndex;
    }
}
