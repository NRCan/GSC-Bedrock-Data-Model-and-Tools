using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSC_ProjectEditor
{
    public class Dictionaries
    {
        /// <summary>
        /// Will sort a given string, string dictionary
        /// </summary>
        /// <param name="inDico">The dictionary to sort</param>
        /// <returns></returns>
        public static SortedDictionary<string, string> GetSortedStringDico(Dictionary<string, string> inDico)
        {
            SortedDictionary<string, string> sortedDico = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> kv in inDico)
            {
                sortedDico[kv.Key] = kv.Value;
            }

            return sortedDico;
        }

    }
}
