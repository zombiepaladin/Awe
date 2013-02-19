using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using zlib;

namespace AweEditor
{

    /*
     * TerrainImporter: reads input file, finds and decompresses chunk 
     * Reading chunk functionality not implemented.
     * 
     * */
    class TerrainImporter
    {
        static string fileName = "r.0.1.MCA";
        public TerrainImporter() { }


        public void importTerrain(TerrainViewerControl terrainViewerControl)
        {
            string path = fileName;
            string fullPath = System.IO.Path.GetFullPath(path);

            byte[] bytes = File.ReadAllBytes(fullPath);
            int index = 8192;
            long length = 0;
            for (int i = index; i < index + 4; i++)
            {
                length = (length << 4) + (bytes[i] & 0xff);
            }
            byte compressionType = bytes[index + 4];
            byte[] data = new byte[length];
            for (int i = 0; i < length; i++)
            {
                data[i] = bytes[i + index + 5];
            }
            byte[] decompressedChunk;

            DecompressData(data, out decompressedChunk);
            
            VoxelTerrain terrain = new VoxelTerrain();
            int worldSize = terrain.getWorldSize();
            for (int x = 0; x < worldSize; x++)
            {
                for (int z = 0; z < worldSize; z++)
                {
                    terrain.addBlock(x, 0, z, BlockType.Stone);
                }
            }

            terrain.addBlock(1, 1, 1, BlockType.Stone);
            terrainViewerControl.VoxelTerrain = terrain;
        }

        public static void DecompressData(byte[] inData, out byte[] outData)
        {
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream))
            using (Stream inMemoryStream = new MemoryStream(inData))
            {
                CopyStream(inMemoryStream, outZStream);
             //   outZStream.finish();
                outData = outMemoryStream.ToArray();
            }
        }

        public static void CopyStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[2000];
            int len;
            while ((len = input.Read(buffer, 0, 2000)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }   
    }
}
