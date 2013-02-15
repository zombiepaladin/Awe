using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AweEditor.Utilities
{
    public class MincraftImporter
    {
        public int xPos = 0;
        public int zPos = 0;
        public long LastUpdate;
        public bool TerrainPopulated;
        public byte[] Biomes = new byte[256];
        public int[] HeightMap = new int[256];
        public int y = 0;
        public byte[] Blocks = new byte[4096];
        public byte[] Add = new byte[2048];
        public byte[] Data = new byte[2048];
        public byte[] BlockLight = new byte[2048];
        public byte[] SkyLight = new byte[2048];
    }
}
