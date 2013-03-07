using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.IO;

namespace VoxelTerrianMeshPipeline
{
    [ContentImporter(".vox", DefaultProcessor = "ModelProcessor", DisplayName = "VoxelTerrianImporter")]
    public class VoxelTerrianImporter : ContentImporter<MeshContent>
    {
        public override MeshContent Import(string filename, ContentImporterContext context)
        {
            MarchingCubesGenerator generator = new MarchingCubesGenerator();
            return generator.March(LoadFrom(filename), true);
        }

        public static List<BlockData> LoadFrom(string filename)
        {
            List<BlockData> blocks = new List<BlockData>();

            using (StreamReader input = new StreamReader(File.OpenRead(filename)))
            {
                string buffer;
                while ((buffer = input.ReadLine()) != null)
                {
                    string[] data = buffer.Split(',');
                    BlockData block = new BlockData()
                    {
                        x = Int32.Parse(data[0]),
                        y = Int32.Parse(data[1]),
                        z = Int32.Parse(data[2]),
                        type = Byte.Parse(data[3])
                    };
                    blocks.Add(block);
                }
            }

            return blocks;
        }
    }
}
