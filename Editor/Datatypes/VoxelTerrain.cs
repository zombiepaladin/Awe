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
        // TODO: Implement remaining block types
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
