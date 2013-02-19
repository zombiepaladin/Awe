using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AweEditor.Datatypes;

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

    /// <summary>
    /// A class to represent voxel terrain
    /// </summary>
    public class VoxelTerrain
    {
        public List<TerrainBlockInstance> blocks; //store as List for now, optimize later

        public VoxelTerrain(List<TerrainBlockInstance> blocks)
        {
            this.blocks = blocks;
        }
        
    }

}
