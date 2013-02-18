#region File Description
//-----------------------------------------------------------------------------
// ModelViewerControl.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AweEditor
{
    /// <summary>
    /// Example control inherits from GraphicsDeviceControl, and displays
    /// a voxel terrain. The main form class is responsible for loading
    /// the terrain: this control just displays it.
    /// </summary>
    class TerrainViewerControl : GraphicsDeviceControl
    {
        /// <summary>
        /// Gets or sets the current voxel terrain.
        /// </summary>
        public VoxelTerrain VoxelTerrain
        {
            get { return voxelTerrain; }
            set { voxelTerrain = value; }
        }

        VoxelTerrain voxelTerrain;

        /// <summary>
        /// Gets or sets the current model.
        /// </summary>
        public Model Model
        {
            get { return model; }

            set
            {
                model = value;

                if (model != null)
                {
                    MeasureModel();
                }
            }
        }

        Model model;

        // Cache information about the model size and position.
        Matrix[] boneTransforms;
        Vector3 modelCenter;
        float modelRadius;


        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

        }


        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            if (model != null)
            {

                Vector3 eyePosition = modelCenter;

                //eyePosition.Z += modelRadius;
                //eyePosition.Y += modelRadius;
                eyePosition.Z += 40;
                eyePosition.Y += 40;
                eyePosition.X += 40;

                float aspectRatio = GraphicsDevice.Viewport.AspectRatio;

                float nearClip = modelRadius / 100;
                float farClip = modelRadius * 100;

                Matrix world;
                Matrix view = Matrix.CreateLookAt(eyePosition, modelCenter, Vector3.Up);
                Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, aspectRatio,
                                                                    nearClip, farClip);
                for (int x = 0; x < voxelTerrain.getWorldSize(); x++)
                {
                    for (int y = 0; y < voxelTerrain.getWorldSize(); y++)
                    {
                        for (int z = 0; z < voxelTerrain.getWorldSize(); z++)
                        {
                            if (voxelTerrain.getTerrainMatrix()[x, y, z] == 1)
                            {
                                // Draw the model.
                                foreach (ModelMesh mesh in model.Meshes)
                                {
                                    foreach (BasicEffect effect in mesh.Effects)
                                    {
                                        world = Matrix.CreateScale(1 / modelRadius) * Matrix.CreateTranslation(x, y, z);
                                        effect.World = boneTransforms[mesh.ParentBone.Index] * world;
                                        effect.View = view;
                                        effect.Projection = projection;

                                        effect.EnableDefaultLighting();
                                        effect.PreferPerPixelLighting = true;
                                        effect.SpecularPower = 16;
                                    }

                                    mesh.Draw();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Whenever a new model is selected, we examine it to see how big
        /// it is and where it is centered. This lets us automatically zoom
        /// the display, so we can correctly handle models of any scale.
        /// </summary>
        void MeasureModel()
        {
            // Look up the absolute bone transforms for this model.
            boneTransforms = new Matrix[model.Bones.Count];

            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Compute an (approximate) model center position by
            // averaging the center of each mesh bounding sphere.
            modelCenter = Vector3.Zero;

            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere meshBounds = mesh.BoundingSphere;
                Matrix transform = boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);

                modelCenter += meshCenter;
            }

            modelCenter /= model.Meshes.Count;

            // Now we know the center point, we can compute the model radius
            // by examining the radius of each mesh bounding sphere.
            modelRadius = 0;

            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere meshBounds = mesh.BoundingSphere;
                Matrix transform = boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);

                float transformScale = transform.Forward.Length();

                float meshRadius = (meshCenter - modelCenter).Length() +
                                   (meshBounds.Radius * transformScale);

                modelRadius = Math.Max(modelRadius, meshRadius);
            }
        }

    }

}
