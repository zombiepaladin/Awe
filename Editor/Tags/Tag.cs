using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AweEditor
{
    public abstract class Tag
    {
        public enum TagType 
        {
            TAG_Byte,
            TAG_Byte_Array,
            TAG_Compound,
            TAG_Double,
            TAG_Float,
            TAG_Int,
            TAG_Int_Array,
            TAG_List,
            TAG_Long,
            TAG_Short,
            TAG_String,
        }

        public Tag parent { get; set; }

        public TagType tagType { get; set; }

        public string name { get; set; }
    }
}
