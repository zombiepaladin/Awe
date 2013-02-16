using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AweEditor
{
    class SubChunk
    {
        int Y;
        byte[] Blocks = new byte[4096];
        byte[] Add = new byte[2048];
        byte[] Data = new byte[2048];
        byte[] BlockLight = new byte[2048];
        byte[] SkyLight = new byte[2048];

        public SubChunk()
        {
            Y = 0;
        }
    }
}
