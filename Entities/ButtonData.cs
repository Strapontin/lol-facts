using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_facts.Entities
{
    public class ButtonData
    {
        public ButtonData()
        { }

        public ButtonData(ButtonActionEnum buttonActionEnum, string tag, int index)
        {
            Action = buttonActionEnum;
            Tag = tag;
            Index = index;
        }

        public ButtonActionEnum Action { get; set; }
        public string Tag { get; set; }
        public int Index { get; set; }
    }

    public enum ButtonActionEnum
    {
        Left = 1,
        Right = 2,
    }
}
