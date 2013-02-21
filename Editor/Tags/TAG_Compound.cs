using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AweEditor
{
    public class TAG_Compound : Tag
    {
        override List<Tag> data { get; set; }

        public TAG_Compound(string _name, Tag _parent)
        {
            this.data = new List<Tag>();
            this.name = _name;
            this.parent = _parent;
            this.tagType = TagType.TAG_Compound;
        }

        public void AddChild(Tag _child)
        {
            data.Add(_child);
        }
    }
}
