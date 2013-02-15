using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace AweEditor
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

        public MincraftImporter()
        {
        }

        public byte[] mincraftImport(string fn)
        {
            byte[] file = File.ReadAllBytes(fn);
            byte[] dcFile = Decompress(file);
            return dcFile;

        }
        static byte[] Decompress(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
    }
}
