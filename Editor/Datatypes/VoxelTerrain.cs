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
        // TODO: Implement remaining block types
    }

    /// <summary>
    /// A class to represent voxel terrain
    /// </summary>
    public class VoxelTerrain
    {
        private static int WORLDSIZE = 100;

        private int[, ,] terrainMatrix = new int[WORLDSIZE, WORLDSIZE, WORLDSIZE];
        // TODO: Complete class
        internal void addBlock(int x, int y, int z, string blockType)
        {
            if (x < WORLDSIZE && y < WORLDSIZE && z < WORLDSIZE)
            {
                terrainMatrix[x, y, z] = 1;
            }
        }

        public int getWorldSize()
        {
            return WORLDSIZE;
        }

        public int[, ,] getTerrainMatrix()
        {
            return terrainMatrix;
        }
    }
}
