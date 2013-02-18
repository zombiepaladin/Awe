using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AweEditor
{
    class MinecraftChunk
    {
        public byte[] decryptedChunck;
        public MinecraftChunk(byte[] mc)
        {
            decryptedChunck = mc;
        }

    }
}
