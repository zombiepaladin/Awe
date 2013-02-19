using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AweEditor
{
    public class TAG_Double : Tag
    {
        double data { get; set; }

        public TAG_Double(double _data, string _name, Tag _parent)
        {
            this.data = _data;
            this.name = _name;
            this.parent = _parent;
            this.tagType = TagType.TAG_Double;
        }
    }
}
