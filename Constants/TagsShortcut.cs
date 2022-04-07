using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_facts.Constants
{
    public static class TagsShortcut
    {
        public static string ReplaceTagsShortcut(string tag)
        {
            string mainName = TagShortcut.Where(ts => ts.Value.Contains(tag)).FirstOrDefault().Key;

            if (mainName == null)
                return tag;
            else
                return mainName;
        }

        public static readonly Dictionary<string, IList<string>> TagShortcut = new()
        {
            { "Miss Fortune", new ReadOnlyCollection<string>(new List<string> { "mf", "fortune" }) },
        };
    }
}
