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
            string mainName = TagShortcut.Where(ts => ts.Value.Any(v => v.FormatTag() == tag.FormatTag())).FirstOrDefault().Key;

            if (mainName == null)
                return tag;
            else
                return mainName;
        }

        public static readonly Dictionary<string, IList<string>> TagShortcut = new()
        {
            { "Miss Fortune", new ReadOnlyCollection<string>(new List<string> { "mf", "fortune" }) },
            { "Razorfin", new ReadOnlyCollection<string>(new List<string> { "WharfRat", "RatDeQuai", "RatsDeQuai", "RatsQuai", "RatsQuais" }) },
            { "Harrowing", new ReadOnlyCollection<string>(new List<string> { "NuitDeLHorreur" }) },
            { "Enlightened", new ReadOnlyCollection<string>(new List<string> { "Illumine", "Illumines",  }) },
            { "ShadowIsles", new ReadOnlyCollection<string>(new List<string> { "IlesObscures", "IlesObscure", "IleObscure", "IleObscures" }) },
            { "AbyssalEye", new ReadOnlyCollection<string>(new List<string> { "RegardDesProfondeurs", "RegardsDesProfondeurs", "RegardProfondeurs", "RegardsProfondeurs" }) },
            { "DragonShark", new ReadOnlyCollection<string>(new List<string> { "RequinDragon" }) },
            { "GiantCrab", new ReadOnlyCollection<string>(new List<string> { "CrabeGeant" }) },
            { "GiantSquid", new ReadOnlyCollection<string>(new List<string> { "CalamarGeant" }) },
            { "GoldenNarwhal", new ReadOnlyCollection<string>(new List<string> { "NarvalDore" }) },
            { "ScuttlerCrab", new ReadOnlyCollection<string>(new List<string> { "Carapateur" }) },
            { "WingEaredDevour", new ReadOnlyCollection<string>(new List<string> { "DevoreursaOreillesAilees" }) },
        };
    }
}
