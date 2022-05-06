using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_facts.Entities
{
    public class FactsSearchLogged
    {
        public string UserName { get; set; }
        public string Mention { get; set; }
        public string Tag { get; set; }
        public int Count { get; set; }
        public bool IsCorrectSearch { get; set; }
    }
}
