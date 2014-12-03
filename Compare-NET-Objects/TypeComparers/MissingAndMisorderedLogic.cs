using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KellermanSoftware.CompareNetObjects.TypeComparers
{
    /// <summary>
    /// Compare lists using special key selectors and list missing and misorders as separate mismatches.
    /// </summary>
    public class MissingAndMisorderedLogic : BaseComparer
    {
        private readonly RootComparer _rootComparer;
        private readonly List<string> _alreadyCompared = new List<string>();

        /// <summary>
        /// Missing and misordered logic
        /// </summary>
        /// <param name="rootComparer"></param>
        public MissingAndMisorderedLogic(RootComparer rootComparer)
        {
            _rootComparer = rootComparer;
        }

        internal void CompareItems(CompareParms parms)
        {
            var list1 = (IList)parms.Object1;
            var list2 = (IList)parms.Object2;

            var keys1 = GetKeys(parms.Result, list1, parms.Config.CollectionMatchingKey);
            var keys2 = GetKeys(parms.Result, list2, parms.Config.CollectionMatchingKey);

            var keyCounts1 = keys1.GroupBy(x => x).ToDictionary(group => group.Key, group => group.Count());
            var keyCounts2 = keys2.GroupBy(x => x).ToDictionary(group => group.Key, group => group.Count());
            
            foreach (var key in keyCounts1.Keys.Intersect(keyCounts2.Keys))
            {
                var count1 = keyCounts1[key];
                var count2 = keyCounts2[key];

                if (count1 != count2)
                {
                    AddDifference(parms, String.Format("Key {0} has got mismatching occurences {1}, {2}", key, count1, count2));
                    if (parms.Result.ExceededDifferences || parms.Config.QuickFailLists)
                        return;
                }
            }

            var missingKeys1 = keys2.Except(keys1).ToList();
            var missingKeys2 = keys1.Except(keys2).ToList();

            foreach (var missingKey in missingKeys1)
            {
                var missingIndex = keys2.IndexOf(missingKey);
                AddDifference(parms, String.Format("Left side is missing item with key {0} (index {1})", missingKey, missingIndex), DifferenceType.Missing);
                if (parms.Result.ExceededDifferences || parms.Config.QuickFailLists)
                    return;
            }

            foreach (var missingKey in missingKeys2)
            {
                var missingIndex = keys1.IndexOf(missingKey);
                AddDifference(parms, String.Format("Right side is missing item with key {0} (index {1})", missingKey, missingIndex), DifferenceType.Missing);
                if (parms.Result.ExceededDifferences || parms.Config.QuickFailLists)
                    return;
            }

            var ignoreIndexes1 = missingKeys2/*.Concat(duplicateKeys1).Concat(duplicateKeys2).Distinct()*/.SelectMany(key => Enumerable.Range(0, keys1.Count).Where(i => keys1[i] == key)).Distinct().ToList();
            var ignoreIndexes2 = missingKeys1/*.Concat(duplicateKeys1).Concat(duplicateKeys2).Distinct()*/.SelectMany(key => Enumerable.Range(0, keys2.Count).Where(i => keys2[i] == key)).Distinct().ToList();

            var items1 = list1.Cast<object>().Where((item, i) => !ignoreIndexes1.Contains(i)).ToList();
            var items2 = list2.Cast<object>().Where((item, i) => !ignoreIndexes2.Contains(i)).ToList();

            var itemKeys1 = keys1.Where((item, i) => !ignoreIndexes1.Contains(i)).ToList();
            var itemKeys2 = keys2.Where((item, i) => !ignoreIndexes2.Contains(i)).ToList();

            var ignoreOrder = parms.Property != null && parms.Config.IgnoreOrderFor.Any(memberString => memberString == parms.Property.DeclaringType.FullName + "." + parms.Property.Name);
            CompareLists(parms, items1, items2, itemKeys1, itemKeys2, ignoreOrder);
        }

        private void CompareLists(CompareParms parms, List<object> items1, List<object> items2, List<string> itemKeys1, List<string> itemKeys2, bool ignoreOrder)
        {
            for (var i = 0; i < Math.Min(items1.Count, items2.Count); i++)
            {
                var item1 = items1[i];
                var item2 = items2[i];
                var key1 = itemKeys1[i];
                var key2 = itemKeys2[i];

                if (key1 == key2)
                {
                    var childParms = GetItemParms(parms, item1, item2, key1);
                    _rootComparer.Compare(childParms);
                }
                else
                {
                    var wrongIndex = itemKeys2.IndexOf(key1);
                    var misorderedItem2 = items2[wrongIndex];
                    var childParms = GetItemParms(parms, item1, misorderedItem2, key1);

                    if (!ignoreOrder)
                        AddDifference(GetItemParms(parms, item1, misorderedItem2, key1), String.Format("Misordered item with key {0} (index {1} vs {2})", key1, i, wrongIndex), DifferenceType.Misorder);

                    var match = _rootComparer.Compare(childParms);

                    if (!match && parms.Config.QuickFailLists)
                        return;
                }

                if (parms.Result.ExceededDifferences)
                    return;
            }
        }
        
        private CompareParms GetItemParms(CompareParms parms, object item1, object item2, string index)
        {
            var extra = item1 == null ? "(null)" : parms.Config.CustomStringifier(item1) ?? String.Empty;

            if (extra.Length > 0)
                extra = "(" + extra + ")";

            string currentBreadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, string.Empty, extra, index);

            return new CompareParms
            {
                Result = parms.Result,
                Config = parms.Config,
                ParentObject1 = parms.Object1,
                ParentObject2 = parms.Object2,
                Object1 = item1,
                Object2 = item2,
                MemberPath = parms.MemberPath + "[]",
                BreadCrumb = currentBreadCrumb,
                ClassDepth = parms.ClassDepth
            };
        }

        private List<string> GetKeys(ComparisonResult result, IList items, Dictionary<Type, Func<object, string>> matchingSpec)
        {
            return items.Cast<object>().Select((item, i) => GetKey(result, item, i, matchingSpec)).ToList();
        }

        private string GetKey(ComparisonResult result, object item, int index, Dictionary<Type, Func<object, string>> matchingSpec)
        {
            if (item == null)
                return "[" + index.ToString() + "]";

            var itemType = item.GetType();

            if (matchingSpec.Keys.Any(specType => specType.IsAssignableFrom(itemType))) { 
                return matchingSpec.First(entry => entry.Key.IsAssignableFrom(itemType)).Value(item);
            }

            return "[" + index.ToString() + "]";
        }
    }
}
