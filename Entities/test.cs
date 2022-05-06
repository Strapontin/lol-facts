using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_facts.Entities
{
    public class test
    {
        [Index(0)]
        public string Name { get; set; }
        [Index(1)]
        public string Text { get; set; }
        [Index(2)]
        public string OriginalText { get; set; }

        private List<string> _tags;
        [Index(3)]
        public List<string> Tags
        {
            get { return _tags; }
            set
            {
                if (value != null && value.Count == 1)
                {
                    _tags = value[0].Split(",").ToList();
                }
            }
        }
    }
}
