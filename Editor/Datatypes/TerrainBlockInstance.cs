using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AweEditor.Datatypes
{
    public class TerrainBlockInstance
    {
        private BlockType blockType;
        public Vector3 position;
        public Matrix transform;

        public TerrainBlockInstance(float x, float y, float z, BlockType blockType)
        {
            this.blockType = blockType;
            
            position = new Vector3(x, y, z);

            transform = Matrix.CreateTranslation(position);
        }
    }
}
