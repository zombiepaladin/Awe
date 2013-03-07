using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.IO;
using AweEditor.Utilities.MarchingCubes;

namespace AweEditor
{
    [ContentImporter(".vox", DefaultProcessor = "ModelProcessor", DisplayName = "VoxelTerrianImporter")]
    public class VoxelTerrianImporter : ContentImporter<MeshContent>
    {
        public override MeshContent Import(string filename, ContentImporterContext context)
        {
            MarchingCubesGenerator generator = new MarchingCubesGenerator();
            return generator.March(VoxelTerrain.LoadFrom(filename).blocks, true);
        }
    }
}
