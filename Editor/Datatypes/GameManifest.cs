﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace AweEditor.Datatypes
{
    /// <summary>
    /// Represents all the assets needed for a single game
    /// </summary>
    public class GameManifest
    {
        /// <summary>
        /// A Dictonary of VoxelTerrains used within the game, 
        /// keyed by the asset name
        /// </summary>
        public Dictionary<string, VoxelTerrain> VoxelTerrains = new Dictionary<string, VoxelTerrain>();

        /// <summary>
        /// A Dictionary of Models used within the game,
        /// keyed by the asset name
        /// </summary>
        public Dictionary<string, Model> Models = new Dictionary<string, Model>();

        /// <summary>
        /// A Dictionary of Texture2D used within the game,
        /// keyed by the asset name
        /// </summary>
        public Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        /// <summary>
        /// A Dictionary of MeshContent used withing the game,
        /// keyed by the asset name
        /// </summary>
        public Dictionary<string, MeshContent> Meshes = new Dictionary<string, MeshContent>();
    }
}
