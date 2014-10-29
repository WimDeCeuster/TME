using System;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IBaseMapper
    {
        TBase MapLocalizableDefaults<TBase>(TBase baseObject, Administration.BaseObjects.LocalizeableBusinessBase localizableObject)
            where TBase : Repository.Objects.Core.BaseObject;
        
        TBase MapTranslateableDefaults<TBase, TTranslateable>(
            TBase baseObject,
            TTranslateable translateableObject,
            String defaultName)
            where TBase : Repository.Objects.Core.BaseObject
            where TTranslateable : Administration.BaseObjects.TranslateableBusinessBase;

        TBase MapSortDefaults<TBase>(TBase baseObject, Administration.BaseObjects.ISortedIndex sortableObject)
            where TBase : Repository.Objects.Core.BaseObject;

        TBase MapDefaults<TBase, TTranslateable>(
            TBase baseObject,
            Administration.BaseObjects.LocalizeableBusinessBase localizableObject,
            TTranslateable translateableObject,
            String defaultName)
            where TBase : Repository.Objects.Core.BaseObject
            where TTranslateable : Administration.BaseObjects.TranslateableBusinessBase;

        TBase MapDefaultsWithSort<TBase, TTranslateableAndSortable>(
            TBase baseObject,
            Administration.BaseObjects.LocalizeableBusinessBase localizableObject,
            TTranslateableAndSortable translateableAndSortableObject,
            String defaultName)
            where TBase : Repository.Objects.Core.BaseObject
            where TTranslateableAndSortable : Administration.BaseObjects.TranslateableBusinessBase, Administration.BaseObjects.ISortedIndex;
    }
}
