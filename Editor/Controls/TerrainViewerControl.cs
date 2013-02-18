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
using AweEditor.Datatypes;
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
        Matrix[] instancedModelBones;
        public Model voxelModel;
        Stopwatch timer;
        DynamicVertexBuffer instanceVertexBuffer;
        Matrix[] instanceTransforms;

        // To store instance transform matrices in a vertex buffer, we use this custom
        // vertex type which encodes 4x4 matrices as a set of four Vector4 values.
        static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
            timer = Stopwatch.StartNew();
        }


        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {

            //Red background for testing
            Color backColor = new Color(255, 0, 0);
            GraphicsDevice.Clear(backColor);
            
            //Quit if no terrain to render
            if (voxelTerrain == null || voxelModel == null)
                return;

            if (instancedModelBones == null)
            {
                instancedModelBones = new Matrix[voxelModel.Bones.Count];
                voxelModel.CopyAbsoluteBoneTransformsTo(instancedModelBones);
            }

            //Setup camera
            float rotation = (float)timer.Elapsed.TotalSeconds;

            Vector3 eyePosition = Vector3.Zero;

            eyePosition.Z += 30 * 2;
            eyePosition.Y += 30;

            float aspectRatio = GraphicsDevice.Viewport.AspectRatio;

            float nearClip = .33f;
            float farClip = 30 * 100;
            
            Matrix world = Matrix.CreateRotationY(rotation);
            Matrix view = Matrix.CreateLookAt(new Vector3(10, 10, 10), Vector3.Zero, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView((float) (Math.PI / 2), aspectRatio,
                                                                nearClip, farClip);
            
            Array.Resize(ref instanceTransforms, voxelTerrain.blocks.Count);
            for(int i = 0; i < voxelTerrain.blocks.Count; i++)
            {
                instanceTransforms[i] = voxelTerrain.blocks[i].transform;
            }

            //DrawModelHardwareInstancing(voxelModel, instancedModelBones, instanceTransforms, view, projection);

            //Draw
            foreach (ModelMesh mesh in voxelModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.SpecularPower = 16;

                    foreach (TerrainBlockInstance block in voxelTerrain.blocks)
                    {
                        effect.World = block.transform * world;
                        mesh.Draw();
                    }
                }
            }
             
        }

        /// <summary>
        /// Taken from XNA model instancing example
        /// Efficiently draws several copies of a piece of geometry using hardware instancing.
        /// </summary>
        void DrawModelHardwareInstancing(Model model, Matrix[] modelBones,
                                         Matrix[] instances, Matrix view, Matrix projection)
        {
            if (instances.Length == 0)
                return;

            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((instanceVertexBuffer == null) ||
                (instances.Length > instanceVertexBuffer.VertexCount))
            {
                if (instanceVertexBuffer != null)
                    instanceVertexBuffer.Dispose();

                instanceVertexBuffer = new DynamicVertexBuffer(GraphicsDevice, instanceVertexDeclaration,
                                                               instances.Length, BufferUsage.WriteOnly);
            }

            // Transfer the latest instance transform matrices into the instanceVertexBuffer.
            instanceVertexBuffer.SetData(instances, 0, instances.Length, SetDataOptions.Discard);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                        new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                    );

                    GraphicsDevice.Indices = meshPart.IndexBuffer;

                    // Set up the instance rendering effect.
                    Effect effect = meshPart.Effect;
                    
                    //effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];
                    
                    effect.Parameters["World"].SetValue(modelBones[mesh.ParentBone.Index]);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    
                    // Draw all the instance copies in a single call.
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, instances.Length);
                      
                    }
                }
            }
        }
    }
}
