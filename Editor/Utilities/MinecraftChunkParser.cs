using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AweEditor
{
    class MinecraftChunkParser
    {
        MinecraftChunk mc;
        int xPos;
        int zPos;
        long LastUpdate;
        bool TerrainPopulated;
        byte[] Biomes = new byte[256];
        int[] HeightMap = new int[256];
        List<SubChunk> Sections = new List<SubChunk>();

        public MinecraftChunkParser(MinecraftChunk m)
        {
            mc = m;
        }
        public void Parse()
        {
            if (mc != null)
            {

            }
        }
    }
}
