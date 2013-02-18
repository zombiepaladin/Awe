using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftImportTesting
{
    /// <summary>
    /// Handles converting transition data to actual VoxelData.
    /// </summary>
    public class VoxelConverter
    {
        private static BlockType BlockIDtoType(int id)
        {
            switch (id)
            {
                case 12: //Sand
                case 3: //Dirt
                case 13: //Gravel
                case 60: //Farmland
                    return BlockType.Dirt;
                case 2: //Grass
                    return BlockType.Grass;
                case 1: //Stone
                case 4: //Cobblestone
                case 7: //Bedrock
                case 24: //Sandstone
                case 14: //Gold Ore
                case 15: //Iron Ore
                case 16: //Coal Ore
                case 56: //Diamond Ore
                case 73: //Redstone Ore
                case 74: //Glowing Redstone ORe
                    return BlockType.Stone;
                case 8: //Water
                case 9: //Stationary Water
                case 10: //Lava
                case 11: //Stationary Lava
                case 79: 
                    return BlockType.Water;
                default:
                    return BlockType.Air;
            }
        }
    }
}
