using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AweEditor.Tags
{
    public class TAG_Int_Array : Tag
    {
        override int[] data { get; set; }

        public TAG_Int_Array(int[] _data, string _name, Tag _parent)
        {
            this.data = _data;
            this.name = _name;
            this.parent = _parent;
            this.tagType = TagType.TAG_Int_Array;
        }
    }
}
