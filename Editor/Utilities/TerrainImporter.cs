using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using zlib;

namespace AweEditor
{

    
    class TerrainImporter
    {
        static string fileName = "r.0.1.MCA";
        public TerrainImporter() { }


        public void importTerrain(TerrainViewerControl terrainViewerControl)
        {
            int counter = 0;
            string line;
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
            /*
            using (GZipStream gzipStream = new GZipStream(File.OpenRead(fullPath), CompressionMode.Decompress))
            {
                using(StreamReader sr = new StreamReader(gzipStream))
                {
                    //Matt try something like this as a hint / starting point 
                    while((line = sr.ReadLine())!=null)
                    {
                        System.Console.WriteLine(line);
                        counter++;
                    }
                    sr.Close();
                }   
            }
            System.Console.WriteLine("There were {0} lines.", counter);
            
            /*
            // Read the file and display it line by line.
            System.IO.StreamReader file =
                new System.IO.StreamReader(fullPath);
            while ((line = file.ReadLine()) != null)
            {
                //System.Console.WriteLine(line);
                counter++;
            }

            file.Close();
            System.Console.WriteLine("There were {0} lines.", counter);
            // Suspend the screen.
            System.Console.ReadLine();
             * */
            VoxelTerrain terrain = new VoxelTerrain();
            for (int x = 0; x < 100; x++)
            {
                for (int z = 0; z < 100; z++)
                {
                    terrain.addBlock(x, 0, z, "defaultType");
                }
            }
            for (int y = 0; y < 100; y++)
            {
                terrain.addBlock(50, y, 50, "defaultType");
            }
            terrain.addBlock(1, 1, 1, "defaultType");
            //terrain.addBlock(0, 0, 0, "defaultType");
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
