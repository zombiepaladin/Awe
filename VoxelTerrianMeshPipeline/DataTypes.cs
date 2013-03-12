using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace VoxelTerrianMeshPipeline
{
    public struct BlockData
    {
        public int x, y, z;

        public byte type;
    }

    /// <summary>
    /// Holder for Triangle verticies indexes.
    /// </summary>
    public struct Triangle
    {
        public int Vertex1Index;
        public int Vertex2Index;
        public int Vertex3Index;
        public MaterialContent Material;
    }
}
