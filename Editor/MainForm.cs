#region File Description
//-----------------------------------------------------------------------------
// MainForm.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AweEditor.Datatypes;
using System.Collections.Generic;
using System.Text;
using AweEditor.Utilities;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline;
#endregion

namespace AweEditor
{
    /// <summary>
    /// Custom form provides the main user interface for the program.
    /// In this sample we used the designer to fill the entire form with a
    /// ModelViewerControl, except for the menu bar which provides the
    /// "File / Open..." option.
    /// </summary>
    public partial class MainForm : Form
    {
        ContentBuilder contentBuilder;
        ContentManager contentManager;
        GameManifest gameManifest;

        /// <summary>
        /// Constructs the main form.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            contentBuilder = new ContentBuilder();

            contentManager = new ContentManager(editorViewerControl.Services,
                                                contentBuilder.OutputDirectory);

            // Automatically start with an empty game manifest
            gameManifest = new GameManifest();
        }

        /// <summary>
        /// Event handler for the New menu option
        /// </summary>
        void NewMenuClicked(object sender, EventArgs e)
        {
            gameManifest = new GameManifest();
        }

        /// <summary>
        /// Event handler for the Open menu option
        /// </summary>
        void OpenMenuClicked(object sender, EventArgs e)
        {
            // TODO: Load game manifest and associated game resources from file
            throw new NotImplementedException();
        }

        /// <summary>
        /// Event handler for the Save menu option
        /// </summary>
        void SaveMenuClicked(object sender, EventArgs e)
        {
            // TODO: Save game manifest and resources to a file
            throw new NotImplementedException();
        }

        /// <summary>
        /// Event handler for the SaveAs menu option
        /// </summary>
        void SaveAsMenuClicked(object sender, EventArgs e)
        {
            // TODO: Save game manifest and resources to a new file
            throw new NotImplementedException();
        }

        /// <summary>
        /// Event handler for the Exit menu option.
        /// </summary>
        void ExitMenuClicked(object sender, EventArgs e)
        {
            Close();
        }

        #region Asset Importing Event Handlers & Helpers

        /// <summary>
        /// Event handler for the Import Model menu option.
        /// </summary>
        void ImportModelMenuClicked(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = ContentPath();

            fileDialog.Title = "Load Model";

            fileDialog.Filter = "Model Files (*.fbx;*.x)|*.fbx;*.x|" +
                                "FBX Files (*.fbx)|*.fbx|" +
                                "X Files (*.x)|*.x|" +
                                "All Files (*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                //editorViewerControl.UnpauseForm();
                ttcControlPanel.SelectTab("tpModelControls");
                ttcControlPanel.SelectTab("tpTerrainControls");
                LoadModel(fileDialog.FileName);
            }
        }

        

        /// <summary>
        /// Loads a new minecraft terrain file into the TerrainViewerControl.
        /// </summary>
        private void ImportVoxelTerrainMenuClicked(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();

            fd.InitialDirectory = ContentPath() + "/Terrains";

            fd.Title = "Import Voxel Terrain";

            fd.Filter = "Schematic and Region Files (*.schematic; *.mcr)|*.schematic;*.mcr|"+
                        "All File (*.*)|*.*";

            if (fd.ShowDialog() == DialogResult.OK)
            {
                editorViewerControl.UnpauseForm();
                ttcControlPanel.SelectTab("tpTerrainControls");
                LoadVoxelTerrain(fd.FileName);
                RepositionCamera();
                editorViewerControl.PauseForm();
            }

            createTerrianModelToolStripMenuItem.Enabled = true;
        }

        private void LoadVoxelTerrain(string fileName)
        {
            Cursor = Cursors.WaitCursor;

            string extension = Path.GetExtension(fileName).ToLower();

            List<BlockData> blocks;
            switch(extension)
            {
                case ".schematic":
                    SchematicProcessor schematicProcessor = new SchematicProcessor(fileName);
                    blocks = schematicProcessor.generateBlockData();
                    break;

                case ".mcr":
                    List<Chunk> chunkList = VoxelTerrainImporter.LoadTerrain(fileName);
                    blocks = VoxelTerrainImporter.GenerateBlocks(chunkList);
                    break;

                //TODO: Handle Anvil region files
                case ".mca": //Letting it fall through to default for now
                    //TODO:Delete
                    VoxelTerrainImporter.LoadTerrain(fileName);
                    blocks = new List<BlockData>();
                    break;

                default:
                    MessageBox.Show(String.Format("The {0} format is not accepted - Aborting", extension));
                    return;
            }

            editorViewerControl.VoxelTerrain = new VoxelTerrain(blocks);
            Cursor = Cursors.Arrow;
        }


        /// <summary>
        /// Loads a new 3D model file into the Game Project and displays
        /// it in the editorViewerControl.
        /// </summary>
        void LoadModel(string fileName)
        {
            Cursor = Cursors.WaitCursor;

            // Determine the model's path
            string path = Path.Combine("Models", Path.GetFileNameWithoutExtension(fileName));

            // Tell the ContentBuilder what to build.
            contentBuilder.Clear();
            contentBuilder.Add(fileName, path, null, "ModelProcessor");

            // Build this new model data.
            string buildError = contentBuilder.Build();

            if (string.IsNullOrEmpty(buildError))
            {
                // If the build succeeded, use the ContentManager to
                // load the temporary .xnb file that we just created.
                Model model = contentManager.Load<Model>(path);
                
                // Display the model in our EditorViewerControl
                editorViewerControl.Model = model;

                // Also store the model in our game manifest
                gameManifest.Models.Add(path, model);
            }
            else
            {
                // If the build failed, display an error message.
                MessageBox.Show(buildError, "Error");
            }

            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Imports a texture file 
        /// </summary>
        private void ImportTextureClicked(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();

            fd.InitialDirectory = ContentPath();

            fd.Title = "Import Image";

            fd.Filter = "Image Files (*.bmp;*.dds;*.dib;*.hdr;*.jpg;*.pfm;*.png;*.ppm;*.tga)|*.bmp;*.dds;*.dib;*.hdr;*.jpg;*.pfm;*.png;*.ppm;*.tga|" +
                                "Bitmap (*.bmp)|*.bmp|" +
                                "Portable network Graphic (*.png)|*.png|" +
                                "All Files (*.*)|*.*";

            if (fd.ShowDialog() == DialogResult.OK)
            {
                editorViewerControl.UnpauseForm();
                ttcControlPanel.SelectTab("tpTextureControls");
                LoadTexture(fd.FileName);
            }

        }

        /// <summary>
        /// Loads a new a texture from a file into the game project
        /// and displays it in the editorViewerControl
        /// </summary>
        /// <param name="fileName">The texture file to import</param>
        protected void LoadTexture(string fileName)
        {
            Cursor = Cursors.WaitCursor;
            
            // Determine the texture's path
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
                Texture2D texture = contentManager.Load<Texture2D>(path);

                // Display the texture in the EditorViewControl
                editorViewerControl.Texture = texture;

                // Store the texture in our game manifest
                gameManifest.Textures.Add(path, texture);
            }
            else
            {
                // If the build failed, display an error message.
                MessageBox.Show(buildError, "Error");
            }

            Cursor = Cursors.Arrow;
        }

        void CreateMeshMenuItemClicked(object sender, EventArgs e)
        {
            CreateTerrianModel(editorViewerControl.VoxelTerrain, "Default");
        }

        void CreateTerrianModel(VoxelTerrain terrian, string meshName)
        {
            Cursor = Cursors.WaitCursor;

            //Save the voxel terrian to a tempory file.
            string terrianFile = Path.Combine(Path.GetTempPath(), meshName + ".vox");
            terrian.SaveTo(terrianFile);

            //Pull the file through the pipeline.
            contentBuilder.Clear();
            contentBuilder.Add(terrianFile, meshName, "VoxelTerrianImporter", "ModelProcessor");

            string buildError = contentBuilder.Build();

            //Now we can treat the terrian as a normal model.
            if(string.IsNullOrEmpty(buildError))
            {
                Model terrianModel = contentManager.Load<Model>(meshName);


                editorViewerControl.UnpauseForm();
                editorViewerControl.TerrianModel = terrianModel;

                gameManifest.TerrianModels.Add(meshName, terrianModel);
            }
            else
            {
                MessageBox.Show("An error occured while generating the mesh:\n" + buildError, "Error");
            }

            Cursor = Cursors.Default;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Returns the directory containing content files
        /// </summary>
        /// <returns>The directory containing content files</returns>
        private static string ContentPath()
        {
            // Default to the directory which contains our content files.
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string relativePath = Path.Combine(assemblyLocation, "../../../../Content");
            string contentPath = Path.GetFullPath(relativePath);
            return contentPath;
        }

        #endregion

        private void btnGo_Click(object sender, EventArgs e)
        {
            editorViewerControl.UnpauseForm();
            RepositionCamera();
            editorViewerControl.PauseForm();
        }

        private void RepositionCamera()
        {
            Cursor = Cursors.WaitCursor;

            Vector3 cameraPosition = new Vector3((float)numCamX.Value, (float)numCamY.Value, (float)numCamZ.Value);
            
            float camYaw = (float)numCamYaw.Value;
            editorViewerControl.CamPosition = cameraPosition;
            editorViewerControl.CamYaw = MathHelper.ToRadians((float)numCamYaw.Value);
            editorViewerControl.CamPitch = MathHelper.ToRadians((float)numCamPitch.Value);
            editorViewerControl.CamRoll = MathHelper.ToRadians((float)numCamRoll.Value);

            editorViewerControl.DrawVoxelTerrain();
            this.Refresh();

            Cursor = Cursors.Arrow;
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            if (editorViewerControl.Paused)
                editorViewerControl.UnpauseForm();
            else
                editorViewerControl.PauseForm();
        }
    }
}
