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
using AweEditor;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Reflection;
using System.Collections;
#endregion

namespace AweEditor
{
    /// <summary>
    /// The states in which the EditorViewerControl can be
    /// </summary>
    public enum EditorState
    {
        None,
        VoxelTerrain,
        Model,
        Texture,
    }

    /// <summary>
    /// Example control inherits from GraphicsDeviceControl, and displays
    /// a spinning 3D model. The main form class is responsible for loading
    /// the model: this control just displays it.
    /// </summary>
    class EditorViewerControl : GraphicsDeviceControl
    {
        long currentDrawIndex = 1000;

        EditorState editorState = EditorState.None;

        // Timer controls the rotation speed.
        Stopwatch timer;

        // SpriteBatch draws sprites on-screen
        SpriteBatch spriteBatch;

        ContentBuilder contentBuilder;
        ContentManager contentManager; //TODO: should we be using the ones declared in MainForm instead?

        #region Terrain Fields

        /// <summary>
        /// Gets or sets the VoxelTerrain
        /// </summary>
        public VoxelTerrain VoxelTerrain
        {
            get { return voxelTerrain; }
            set { 
                voxelTerrain = value;
                editorState = EditorState.VoxelTerrain;
            }
        }
        VoxelTerrain voxelTerrain;
        
        //Generic block to render, will need different types later
        public Model voxelPlaceHolderModel;

        //Used for hardware rendering
        private Matrix[] instancedModelBones;
        private DynamicVertexBuffer instanceVertexBuffer;
        private Matrix[] instanceTransforms;
        
        // To store instance transform matrices in a vertex buffer, we use this custom
        // vertex type which encodes 4x4 matrices as a set of four Vector4 values.
        static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        //Can be useful for seeing the interior of solid structures
        private bool doubleSpaceBlocks = false;

        #endregion

        #region Model Fields

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

                editorState = EditorState.Model;
            }
        }

        Model model;
        
        // Cache information about the model size and position.
        Matrix[] boneTransforms;
        Vector3 modelCenter;
        float modelRadius;

        #endregion

        #region Texture Fields

        /// <summary>
        /// Gets or sets the current texture
        /// </summary>
        public Texture2D Texture 
        {
            get { return texture; }
            set { 
                texture = value;

                if (texture != null)
                {
                    MeasureTexture();
                }

                editorState = EditorState.Texture;
            }
        }

        Texture2D texture;

        Texture2D grassTexture;
        Texture2D stoneTexture;
        Texture2D woodTexture;

        Rectangle textureBounds;

        #endregion


        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            contentBuilder = new ContentBuilder();
            contentManager = new ContentManager(this.Services, contentBuilder.OutputDirectory);

            loadModels();

            loadTextures();

            // Start the animation timer.
            timer = Stopwatch.StartNew();

            // Create the SpriteBatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
        }

        private void loadTextures()
        {
            // Determine the texture's path
            string[] textureFiles = { "grass.jpg", "stone.jpg", "wood.png" };
            Texture2D[] textures = new Texture2D[textureFiles.Length];
            //string fileName = textureFiles[0];
            for (int textureNum = 0; textureNum < textureFiles.Length; textureNum++)
            {
                string fileName = (Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, "../../../../Content/Textures/")) + (textureFiles[textureNum]));
                string path = Path.Combine("Textures", Path.GetFileNameWithoutExtension(fileName));

                // Tell the ContentBuilder what to build.
                contentBuilder.Clear();
                contentBuilder.Add(fileName, path, null, "TextureProcessor");

                // Build this new texture data.
                string buildError = contentBuilder.Build();

                if (string.IsNullOrEmpty(buildError))
                {
                    // If the build succeeded, use the ContentManager to
                    // load the temporary .xnb file that we just created.
                    textures[textureNum] = contentManager.Load<Texture2D>(path);

                    // Store the texture in our game manifest
                    // gameManifest.Textures.Add(path, texture);
                }
                else
                {
                    // If the build failed, display an error message.
                    MessageBox.Show(buildError, "Error");
                }
            }

            grassTexture = textures[0];
            stoneTexture = textures[1];
            woodTexture = textures[2];
        }

        private void loadModels()
        {
            #region Load Placeholder Block Model
            contentManager.Unload();

            // Tell the ContentBuilder what to build.
            contentBuilder.Clear();
            contentBuilder.Add(Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, "../../../../Content")) + "\\Cats.fbx", "Model", null, "InstancedModelProcessor");

            // Build this new model data.
            string buildError = contentBuilder.Build();

            if (string.IsNullOrEmpty(buildError))
            {
                // If the build succeeded, use the ContentManager to
                // load the temporary .xnb file that we just created.
                voxelPlaceHolderModel = contentManager.Load<Model>("Model");
            }
            else
            {
                // If the build failed, display an error message.
                MessageBox.Show(buildError, "Error");
            }
            #endregion
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);
            GraphicsDevice.Clear(backColor);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Render according to current editor state
            switch (editorState)
            {
                case EditorState.VoxelTerrain:
                    DrawVoxelTerrain();
                    break;

                case EditorState.Model:
                    DrawModel();
                    break;

                case EditorState.Texture:
                    DrawTexture();
                    break;

                default:
                    break;
            }
        }

        #region Drawing Methods

        /// <summary>
        /// Draw the current voxel terrain
        /// </summary>
        private void DrawVoxelTerrain()
        {
            #region Terrain Rendering Setup


            //Stop if no terrain to render
            if (voxelTerrain == null || voxelPlaceHolderModel == null)
                return;

            //init bones
            if (instancedModelBones == null) //TODO: move to loadModels when implemented
            {
                instancedModelBones = new Matrix[voxelPlaceHolderModel.Bones.Count];
                voxelPlaceHolderModel.CopyAbsoluteBoneTransformsTo(instancedModelBones);
            }

            //Setup camera
            float aspectRatio = GraphicsDevice.Viewport.AspectRatio;

            float rotation = (float)timer.Elapsed.TotalSeconds;
            //float rotation = 1.5f;
            Matrix world = Matrix.CreateRotationY(0);//rotation);

            //Populate instances here to find max length for camera
            int maxDist = 0;

            int maxSize = Math.Min(1048574, voxelTerrain.blocks.Count);

            Array.Resize(ref instanceTransforms, maxSize);

            Vector3 position = new Vector3();
            BlockData block = new BlockData();
            Matrix transform = new Matrix();

            float scale = 2;
            if (doubleSpaceBlocks) //inverted because dividing by scale
                scale = 1;

            int maxX, maxY, maxZ;
            maxX = maxY = maxZ = 0;

            int minX, minY, minZ;
            minX = minY = minZ = int.MaxValue;

            byte blockType;
            byte[] blockTypeArray = new byte[instanceTransforms.Length];

            for (long i = 0; i < maxSize; i++)
            {
                block = voxelTerrain.blocks[(int)i];

                position.X = block.x / scale; //TODO: fix hardcoded scaling
                position.Y = block.y / scale;
                position.Z = block.z / scale;
                blockType = block.type;

                maxX = Math.Max(maxX, (int)position.X);
                maxY = Math.Max(maxY, (int)position.Y);
                maxZ = Math.Max(maxZ, (int)position.Z);

                minX = Math.Min(minX, (int)position.X);
                minY = Math.Min(minY, (int)position.Y);
                minZ = Math.Min(minZ, (int)position.Z);

                //find distance from origin
                int distFromZero = (int)position.Length();
                //update maxDist if bigger
                if (distFromZero > maxDist)
                    maxDist = distFromZero;

                transform = Matrix.CreateTranslation(position);
                instanceTransforms[(instanceTransforms.Length - (i /*- currentDrawIndex*/)) - 1] = transform * world; //TODO: remove backwards test
                blockTypeArray[(blockTypeArray.Length - (i /*- currentDrawIndex*/)) - 1] = blockType;
            }
            //Debug.WriteLine("{0}", voxelTerrain.blocks[65000]);
            //currentDrawIndex = ((currentDrawIndex + maxSize) > voxelTerrain.blocks.Count) ? 0 : currentDrawIndex + maxSize;

            //Continue camera setup
            //Vector3 eyePosition = Vector3.Zero;

            //float nearClip = maxDist / 50.0f;
            //float farClip = maxDist * 50;

            //Matrix view = Matrix.CreateLookAt(new Vector3(40, 300, 30), new Vector3(0, 0, 0), Vector3.Up);
            //Matrix projection = Matrix.CreatePerspectiveFieldOfView((float)(Math.PI / 2), aspectRatio,
            //                                                    nearClip, farClip);

            modelCenter = new Vector3((minX + maxX) / 2, (minY + maxY)/2, (minZ + maxZ) / 2);
            Vector3 eyePosition = Vector3.Zero;

            eyePosition.Z = minZ + 20;// maxZ + 1;
            eyePosition.X = minX - 5;// maxX + 1;
            eyePosition.Y = maxY - 1;

            Debug.WriteLine("Max = ({0},{1},{2})", maxX, maxY, maxZ);
            Debug.WriteLine("Min = ({0},{1},{2})", minX, minY, minZ);
            Debug.WriteLine("EyePosition = ({0},{1},{2})", eyePosition.X, eyePosition.Y, eyePosition.Z);
            Debug.WriteLine("ModelCenter = ({0},{1},{2})", modelCenter.X, modelCenter.Y, modelCenter.Z);

            aspectRatio = GraphicsDevice.Viewport.AspectRatio;

            float nearClip = 128 / 100f;
            float farClip = 128 * 100;

            world = Matrix.CreateRotationY(rotation);
            Matrix view = Matrix.CreateLookAt(eyePosition, modelCenter, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, aspectRatio,
                                                                nearClip, farClip);

            #endregion

            DrawModelHardwareInstancing(voxelPlaceHolderModel, instancedModelBones, instanceTransforms, view, projection, blockTypeArray);
        }

          /// <summary>
        /// Taken from XNA model instancing example
        /// Efficiently draws several copies of a piece of geometry using hardware instancing.
        /// </summary>
        void DrawModelHardwareInstancing(Model model, Matrix[] modelBones,
                                         Matrix[] instances, Matrix view, Matrix projection, byte [] blockTypeArray)
        {
            if (instances.Length == 0)
                return;

            //create lists for instances with different textures
            ArrayList grassInstances = new ArrayList();
            ArrayList stoneInstances = new ArrayList();
            ArrayList woodInstances = new ArrayList();

            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((instanceVertexBuffer == null) ||
                (instances.Length > instanceVertexBuffer.VertexCount))
            {
                if (instanceVertexBuffer != null)
                    instanceVertexBuffer.Dispose();

                instanceVertexBuffer = new DynamicVertexBuffer(GraphicsDevice, instanceVertexDeclaration,
                                                               instances.Length, BufferUsage.WriteOnly);
            }

            

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
                    
                    effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];
                    
                    effect.Parameters["World"].SetValue(modelBones[mesh.ParentBone.Index]);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);

                    for (int index = 0; index < instances.Length; index++)
                    {
                        //separate different textures blocks
                        switch (blockTypeArray[index])
                        {
                            case 2:
                            case 31:
                            case 9:
                                grassInstances.Add(instances[index]);
                                break;
                            case 1:
                            case 4:
                            case 35:
                            case 42:
                            case 44:
                            case 98:
                            case 109:
                                stoneInstances.Add(instances[index]);
                                break;
                            case 5:
                            case 17:
                            case 20:
                            case 53:
                            case 85:
                            case 126:
                                woodInstances.Add(instances[index]);
                                break;
                            default:
                                grassInstances.Add(instances[index]);
                                break;
                        }
                    }

                    //draw GRASS
                    // Transfer the current instance transform matrices into the instanceVertexBuffer.
                    Matrix[] grassInstancesArray = new Matrix[grassInstances.Count];
                    for (int i = 0; i < grassInstancesArray.Length; i++ )
                    {
                        grassInstancesArray[i] = (Matrix)grassInstances[i];
                    }

                    effect.Parameters["Texture"].SetValue(grassTexture);
                    drawTexturedInstancedPrimitives(grassInstancesArray, effect, meshPart);


                    //draw STONE
                    // Transfer the current instance transform matrices into the instanceVertexBuffer.
                    Matrix[] stoneInstancesArray = new Matrix[stoneInstances.Count];
                    for (int i = 0; i < stoneInstancesArray.Length; i++)
                    {
                        stoneInstancesArray[i] = (Matrix)stoneInstances[i];
                    }

                    effect.Parameters["Texture"].SetValue(stoneTexture);
                    drawTexturedInstancedPrimitives(stoneInstancesArray, effect, meshPart);

                    //draw WOOD
                    // Transfer the current instance transform matrices into the instanceVertexBuffer.
                    Matrix[] woodInstancesArray = new Matrix[woodInstances.Count];
                    for (int i = 0; i < woodInstancesArray.Length; i++)
                    {
                        woodInstancesArray[i] = (Matrix)woodInstances[i];
                    }

                    effect.Parameters["Texture"].SetValue(woodTexture);
                    drawTexturedInstancedPrimitives(woodInstancesArray, effect, meshPart);
                    
                }
            }
        }

        private void drawTexturedInstancedPrimitives(Matrix[] instancesArray, Effect effect, ModelMeshPart meshPart)
        {
            if (instancesArray.Length != 0)
            {
                instanceVertexBuffer.SetData(instancesArray, 0, instancesArray.Length, SetDataOptions.Discard);

                // Draw all the GRASS instance copies in a single call.
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                           meshPart.NumVertices, meshPart.StartIndex,
                                                           meshPart.PrimitiveCount, Math.Min(1048574, instancesArray.Length)); //TODO: should have warning or something when too big


                }
            }
        }

       
    

        /// <summary>
        /// Draw the current model
        /// </summary>
        private void DrawModel()
        {
            if (model != null)
            {
                // Compute camera matrices.
                float rotation = (float)timer.Elapsed.TotalSeconds;

                Vector3 eyePosition = modelCenter;

                eyePosition.Z += modelRadius * 2;
                eyePosition.Y += modelRadius;

                float aspectRatio = GraphicsDevice.Viewport.AspectRatio;

                float nearClip = modelRadius / 100;
                float farClip = modelRadius * 100;

                Matrix world = Matrix.CreateRotationY(rotation);
                Matrix view = Matrix.CreateLookAt(eyePosition, modelCenter, Vector3.Up);
                Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, aspectRatio,
                                                                    nearClip, farClip);

                // Draw the model.
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = boneTransforms[mesh.ParentBone.Index] * world;
                        effect.View = view;
                        effect.Projection = projection;

                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.SpecularPower = 16;
                        effect.TextureEnabled = true;
                        effect.Texture = texture;
                    }

                    mesh.Draw();
                }
            }
        }

        /// <summary>
        /// Draw the current texture
        /// </summary>
        public void DrawTexture()
        {
            if (texture != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, textureBounds, Color.White);
                spriteBatch.End();
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Whenever a new texture is selected, we center it in the window
        /// </summary>
        private void MeasureTexture()
        {
            textureBounds = texture.Bounds;
            Vector2 clientSize = new Vector2(ClientSize.Width, ClientSize.Height);
            Vector2 textureSize = new Vector2(textureBounds.Width, textureBounds.Height);
            Vector2 offset = (clientSize - textureSize) / 2.0f;
            textureBounds.X = (int)offset.X;
            textureBounds.Y = (int)offset.Y;
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

                modelRadius = Math.Max(modelRadius,  meshRadius);
            }
        }

        #endregion
    }
}
