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
using System.Collections.Generic;
using System.Collections;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
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
        TerrianModel,
    }

    /// <summary>
    /// Example control inherits from GraphicsDeviceControl, and displays
    /// a spinning 3D model. The main form class is responsible for loading
    /// the model: this control just displays it.
    /// </summary>
    class EditorViewerControl : GraphicsDeviceControl
    {
        EventHandler invalid;

        long currentDrawIndex = 1000;

        public EditorState EditorState {get; private set;}

        // Timer controls the rotation speed.
        Stopwatch timer;

        // SpriteBatch draws sprites on-screen
        SpriteBatch spriteBatch;

        ContentBuilder contentBuilder;
        ContentManager contentManager; //TODO: should we be using the ones declared in MainForm instead?

        public bool Paused { get; set; }

        #region Terrain Fields

        /// <summary>
        /// Gets or sets the VoxelTerrain
        /// </summary>
        public VoxelTerrain VoxelTerrain
        {
            get { return voxelTerrain; }
            set
            {
                voxelTerrain = value;
                EditorState = EditorState.VoxelTerrain;
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


        public Vector3 CamPosition { get; set; }
        public float CamYaw { get; set; }
        public float CamPitch { get; set; }
        public float CamRoll { get; set; }

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
                    MeasureModel(model);
                }

                EditorState = EditorState.Model;
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
            set
            {
                texture = value;

                if (texture != null)
                {
                    MeasureTexture();
                }

                EditorState = EditorState.Texture;
            }
        }

        Texture2D texture;

        Texture2D grassTexture;
        Texture2D stoneTexture;
        Texture2D woodTexture;

        Rectangle textureBounds;

        #endregion

        #region Terrian Model Fields

        public Model TerrianModel
        {
            get
            {
                return terrianModel;
            }
            set
            {
                terrianModel = value;
                if (terrianModel != null)
                    MeasureModel(terrianModel);
                EditorState = EditorState.TerrianModel;
            }
        }

        private Model terrianModel;

        #endregion

        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            this.CamPosition = Vector3.Zero;
            this.CamYaw = 0;
            this.CamPitch = 0;
            this.CamRoll = 0;

            contentBuilder = new ContentBuilder();
            contentManager = new ContentManager(this.Services, contentBuilder.OutputDirectory);

            loadModels();

            // Start the animation timer.
            timer = Stopwatch.StartNew();

            // Create the SpriteBatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Hook the idle event to constantly redraw our animation.
            this.Paused = true;
            invalid = delegate { Invalidate(); };
            UnpauseForm();
        }

        public void PauseForm()
        {
            if (!this.Paused)
            {
                Application.Idle -= invalid;
                this.Paused = true;
            }
        }

        public void UnpauseForm()
        {
            if (this.Paused)
            {
                Application.Idle += invalid;
                this.Paused = false;
            }
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

        public void ClearForm()
        {
            EditorState = AweEditor.EditorState.None;
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

        private void PrepGraphicsDevice()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);
            GraphicsDevice.Clear(backColor);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            PrepGraphicsDevice();

            // Render according to current editor state
            switch (EditorState)
            {
                case EditorState.VoxelTerrain:
                    DrawVoxelTerrain();
                    break;

                case EditorState.Model:
                    DrawModel(model);
                    break;

                case EditorState.Texture:
                    DrawTexture();
                    break;

                case EditorState.TerrianModel:
                    DrawModel(terrianModel);
                    break;

                default:
                    break;
            }
        }

        #region Drawing Methods

        /// <summary>
        /// Draw the current voxel terrain
        /// </summary>

        public void DrawVoxelTerrain()
        {
            PrepGraphicsDevice();

            //Lets do some sanity checking
            if (voxelTerrain == null || voxelPlaceHolderModel == null)
                return;

            Matrix[] transformInstances;
            Matrix view;
            Matrix projection;
            Matrix world = Matrix.CreateWorld(Vector3.Zero, Vector3.Backward, Vector3.Up);

            //Lets see where we want to put the camera
            //We will go ahead and use the CamPosition we already have
            //Now we just need the rotation
            Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(this.CamYaw, this.CamPitch, this.CamRoll);
            Vector3 camLookAt = this.CamPosition + Vector3.Transform(Vector3.Forward, rotationMatrix);

            float nearClip = 128 / 100f;
            float farClip = 128 * 100;

            float aspectRatio = GraphicsDevice.Viewport.AspectRatio;

            view = Matrix.CreateLookAt(this.CamPosition, camLookAt, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(1, aspectRatio, nearClip, farClip);

            //We'll need to initialize our bones if they haven't been
            if (instancedModelBones == null)
            {
                instancedModelBones = new Matrix[voxelPlaceHolderModel.Bones.Count];
                voxelPlaceHolderModel.CopyAbsoluteBoneTransformsTo(instancedModelBones);
            }

            const int maxBatchSize = 65536; //16x16x256
            int numberOfBlocks = voxelTerrain.blocks.Count;

            //This will be used as an index for instance transforms. i % maxBatchSize
            int matrixIndex;

            //Marks the offset of the block
            Vector3 tempPosition;
            Matrix tempTransform;
            
            int maxSize = Math.Min(1048574, voxelTerrain.blocks.Count);

            Array.Resize(ref instanceTransforms, maxSize);

            transformInstances = new Matrix[maxBatchSize];

            //marks the block itself
            BlockData block;
            byte[] blockTypeArray = new byte[transformInstances.Length];
            const float scale = 2;

            int batchNumber = (numberOfBlocks / maxBatchSize);
            if ((numberOfBlocks % maxBatchSize) > 0) batchNumber++;
            int matrixSize;

            for(int x = 0; x < batchNumber; x++)
            {
                matrixSize = maxBatchSize;
            
                if (x == batchNumber - 1) matrixSize = numberOfBlocks % maxBatchSize;

                for (int i = 0; i < matrixSize; i++)
                {
                    
                    block = voxelTerrain.blocks[(x * maxBatchSize) + i];
                    blockTypeArray[(blockTypeArray.Length - i) - 1] = block.type;
                    tempPosition = Vector3.Divide(new Vector3(block.x, block.y, block.z), scale);

                    tempTransform = Matrix.CreateTranslation(tempPosition);
                    instanceTransforms[(instanceTransforms.Length - (i /*- currentDrawIndex*/)) - 1] = tempTransform * world;
                    blockTypeArray[(blockTypeArray.Length - (i /*- currentDrawIndex*/)) - 1] = block.type;
                    transformInstances[i] = tempTransform * world;
                }

                Array.Clear(transformInstances, matrixSize, transformInstances.Length - matrixSize);

                DrawModelHardwareInstancing
                    (
                    voxelPlaceHolderModel, instancedModelBones, 
                    transformInstances, view, projection,blockTypeArray
                    );
            }
        }

        /// <summary>
        /// Taken from XNA model instancing example
        /// Efficiently draws several copies of a piece of geometry using hardware instancing.
        /// </summary>
        void DrawModelHardwareInstancing(Model model, Matrix[] modelBones,
                                         Matrix[] instances, Matrix view, Matrix projection,byte[] blockTypeArray)
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
                    for (int i = 0; i < grassInstancesArray.Length; i++)
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

                    // Draw all the instance copies in a single call.
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, Math.Min(1048574, instances.Length)); //TODO: should have warning or something when too big

                    }
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
        private void DrawModel(Model model)
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
        void MeasureModel(Model model)
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

        #endregion
    }
}
