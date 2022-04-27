using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_facts.Entities
{
    public class Fact
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string OriginalText { get; set; }
        public List<string> Tags { get; set; }
    }
}
