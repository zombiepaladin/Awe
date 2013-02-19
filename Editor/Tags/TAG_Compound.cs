using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AweEditor
{
    public class TAG_Compound : Tag
    {
        List<Tag> children { get; set; }

        public TAG_Compound(string _name, Tag _parent)
        {
            this.children = new List<Tag>();
            this.name = _name;
            this.parent = _parent;
            this.tagType = TagType.TAG_Compound;
        }

        public void AddChild(Tag _child)
        {
            children.Add(_child);
        }
    }
}
