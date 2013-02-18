using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftImportTesting
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
        Water = 4,
    }

    /// <summary>
    /// A class to represent voxel terrain
    /// </summary>
    public class VoxelTerrain
    {
        private BlockType[][][] _terrain;
    }
}
