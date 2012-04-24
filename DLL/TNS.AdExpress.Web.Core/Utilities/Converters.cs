using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Classification;
using TNS.Classification.Universe;

namespace TNS.AdExpress.Web.Core.Utilities
{
    /// <summary>
    /// Converters class
    /// </summary>
    public class Converters
    {
        /// <summary>
        /// Convert Universe To String
        /// The string format will be :
        /// Universe Dictionary Key : Nomenclature group key : Accestype type key : Classification level key :  classification items ID list|
        /// </summary>
        /// <param name="universes">universes</param>
        /// <returns>universes string</returns>
        public static string ConvertUniverseToString(Dictionary<int, AdExpressUniverse> universes)
        {
            var builder = new StringBuilder();

            if (universes != null && universes.Count > 0)
            {
                foreach (KeyValuePair<int, AdExpressUniverse> kpv in universes)
                {
                    var univ = kpv.Value;
                    foreach (var kpv2 in univ.ElementsGroupDictionary)
                    {
                        var group = kpv2.Value;
                        var listLevel = group.GetLevelIdsList();
                        for (int m = 0; m < listLevel.Count; m++)
                        {
                            builder.AppendFormat("{0}:{1}:{2}:{3}:{4}|", kpv.Key, kpv2.Key, group.AccessType.GetHashCode(), listLevel[m], group.GetAsString(listLevel[m]));
                        }
                    }
                }

            }
            return builder.ToString().TrimEnd('|'); ;
        }

        /// <summary>
        /// Convert string to universe
        /// </summary>
        /// <param name="dimension">universe dimension</param>
        /// <param name="str">unverse string</param>
        /// <returns>AdExpressUniverse dictionary</returns>
        public static Dictionary<int, AdExpressUniverse> ConvertToUniverseDictionary(Dimension dimension, string str)
        {
            var universes = new Dictionary<int, AdExpressUniverse>();

            if (!string.IsNullOrEmpty(str))
            {
                var arr = str.Split(new[] { '|', ':' }, StringSplitOptions.RemoveEmptyEntries);
                long idOldUniv = long.MinValue, idOldGroup = long.MinValue;
                var univ = new AdExpressUniverse(dimension); ;
                NomenclatureElementsGroup group = null;
                var first = true;
                for (int i = 0; i < arr.Length; i = i + 5)
                {
                    var idUniv = Convert.ToInt64(arr[i]);
                    if (!first && idOldUniv != idUniv)
                    {
                        universes.Add(universes.Count, univ);
                        univ = new AdExpressUniverse(dimension);
                    }

                    var idGroup = Convert.ToInt64(arr[i + 1]);
                    if (idOldUniv != idUniv || idGroup != idOldGroup)
                        group = new NomenclatureElementsGroup(idGroup, (AccessType)Convert.ToInt32(arr[i + 2]));
                    group.AddItems(Convert.ToInt64(arr[i + 3]), arr[i + 4]);
                    if(!univ.Contains(idGroup))
                        univ.AddGroup(idGroup, group);

                    idOldUniv = Convert.ToInt64(arr[i]);
                    idOldGroup = Convert.ToInt64(arr[i + 1]);
                    first = false;
                }
                universes.Add(universes.Count, univ);
            }

            return universes;
        }

    }
}
