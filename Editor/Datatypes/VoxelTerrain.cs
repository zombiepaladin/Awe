using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
        GrassBlock = 2,
        WoodenPlanks = 5,
        Water = 8,
        Sand = 12,
        CobbleStone = 4,
        Gravel = 13,
        Wood = 17,
        Leaves = 18,
        Glass = 20,
        TallGrass = 31,
        Snow = 78,
        Ice = 79,
        BlockOfGold = 41,
        BlockOfIron = 42,
        Bricks = 45,
        IronDoor = 71,
        BookShelf = 47,
        Fire = 51

        // TODO: Implement remaining block types
    }

    /// <summary>
    /// A class to represent voxel terrain in 3d matrix
    /// </summary>
    public class VoxelTerrain
    {
        public VoxelTerrain()
        {
            WorldSize = 100;
        }

        public static int WorldSize = 100;

        private int[, ,] terrainMatrix = new int[WorldSize, WorldSize, WorldSize];
        // TODO: Complete class
        internal void addBlock(int x, int y, int z, BlockType blockType)
        {
            if (x < WorldSize && y < WorldSize && z < WorldSize)
            {
                terrainMatrix[x, y, z] = 1;
            }
        }
        
        public int getWorldSize()
        {
            return WorldSize;
        }
        
        public int[, ,] getTerrainMatrix()
        {
            return terrainMatrix;
        }
    }
}
