using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AweEditor
{
    public class TAG_Long : Tag
    {
        override long data { get; set; }

        public TAG_Long(long _data, string _name, Tag _parent)
        {
            this.data = _data;
            this.name = _name;
            this.parent = _parent;
            this.tagType = TagType.TAG_Long;
        }
    }
}
