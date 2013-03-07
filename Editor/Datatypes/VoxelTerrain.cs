using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AweEditor.Datatypes;
using System.IO;

namespace AweEditor
{
    /// <summary>
    /// BlockType corresponds to Minecraft Bock IDs for ease of import
    /// (see http://www.minecraftwiki.net/wiki/Data_values#Block_IDs)
    /// </summary>
    public enum BlockType
    {
        Air = 0,
        Stone = 1,
        Grass = 2,
        Dirt = 3,
        Bedrock = 7,
        Water = 9,
        Lava = 11,
        Sand = 12,
        Gravel = 13,
        Gold = 14,
        Iron = 15,
        Coal = 16,
        Wood = 17,
        Leaves = 18,
        Sandstone = 24,
        Moss = 48,
        Obsidian = 49,
        Diamond = 56,
        Ice = 79,
        Snow = 80,
        Pumpkin = 86,
        Jack_o_Lantern = 91,
        Melon = 103,
        Netherrack = 87,
        Mycelium = 110,
        Emerald = 129,
    }

    public struct BlockData
    {
        public int x, y, z;

        public byte type;
    }

    /// <summary>
    /// A class to represent voxel terrain
    /// </summary>
    public class VoxelTerrain
    {
        public static VoxelTerrain LoadFrom(string filename)
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

            return new VoxelTerrain(blocks);
        }

        public List<BlockData> blocks; //store as List for now, optimize later

        public VoxelTerrain(List<BlockData> blocks)
        {
            this.blocks = blocks;
        }

        public void SaveTo(string filename)
        {
            using (StreamWriter output = new StreamWriter(File.Create(filename)))
            {
                foreach (BlockData block in blocks)
                {
                    output.WriteLine(string.Format("{0}, {1}, {2}, {3}", block.x, block.y, block.z, block.type));
                }

                output.Flush();
                output.Close();
            }
        }
    }
}
