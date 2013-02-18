using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AweEditor
{
    public class SubChunk
    {
        public int Y;
        public byte[] Blocks = new byte[4096];
        public byte[] Add = new byte[2048];
        public byte[] Data = new byte[2048];
        public byte[] BlockLight = new byte[2048];
        public byte[] SkyLight = new byte[2048];

        public SubChunk()
        {
            Y = 0;
        }
    }
}
