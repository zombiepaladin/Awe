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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AweEditor.Datatypes;
using Ionic.Zip;
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
        string saveLocation;

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

            // Don't start with a default save location
            saveLocation = null;
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
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = ContentPath();

            fileDialog.Title = "Load Game Data File";

            fileDialog.Filter = "Awe Game Data Files (*.awed)|*.awed";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenGameAssets(fileDialog.FileName);
                saveLocation = fileDialog.FileName;
            }
        }

        /// <summary>
        /// Event handler for the Save menu option
        /// </summary>
        void SaveMenuClicked(object sender, EventArgs e)
        {
            if (saveLocation == null)
                SaveAsMenuClicked(null, null);
            else if (!File.Exists(saveLocation))
                SaveAsMenuClicked(null, null);
            else
                SaveGameAssets(saveLocation);
        }

        /// <summary>
        /// Event handler for the SaveAs menu option
        /// </summary>
        void SaveAsMenuClicked(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();

            fileDialog.InitialDirectory = ContentPath();

            fileDialog.Title = "Save Game Data File";

            fileDialog.Filter = "Awe Game Data Files (*.awed)|*.awed";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveGameAssets(fileDialog.FileName);
                saveLocation = fileDialog.FileName;
            }
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
        /// Handles the opening of the game assets files
        /// </summary>
        /// <param name="fileName">The path of the game assets file to open</param>
        private void OpenGameAssets(string fileName)
        {
            Cursor = Cursors.WaitCursor;

            #region Unzip assets

            DirectoryInfo dirInfo = new DirectoryInfo(contentBuilder.OutputDirectory);
            if (!dirInfo.Exists)
                dirInfo.Create();

            using (ZipFile zipFile = ZipFile.Read(fileName))
            {
                zipFile.ExtractAll(dirInfo.FullName);
            }

            #endregion

            #region Load assets

            using (XmlTextReader xmlReader = new XmlTextReader(Path.Combine(dirInfo.FullName, "Manifest.xml")))
            {
                string dataType = "None";
                bool loadData = false;
                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (xmlReader.Name == "Models" || xmlReader.Name == "Textures" || xmlReader.Name == "VoxelTerrains")
                                dataType = xmlReader.Name;
                            if (xmlReader.Name == "Name")
                                loadData = true;
                            break;

                        case XmlNodeType.Text:
                            if (loadData)
                            {
                                if (dataType == "Models")
                                    LoadModel(xmlReader.Value, true);
                                else if (dataType == "Textures")
                                    LoadTexture(xmlReader.Value, true);
                                else if (dataType == "VoxelTerrains")
                                {
                                    XmlSerializer voxelSerializer = new XmlSerializer(typeof(VoxelTerrain));
                                    XmlTextReader voxelReader = new XmlTextReader(dirInfo.FullName + @"\VoxelTerrains\" + xmlReader.Value + ".xml");
                                    if (!gameManifest.VoxelTerrains.ContainsKey(xmlReader.Value))
                                    editorViewerControl.VoxelTerrain = gameManifest.VoxelTerrains[xmlReader.Value];
                                    voxelReader.Close();
                                }
                            }
                            loadData = false;
                            break;

                        case XmlNodeType.EndElement:
                            loadData = false;
                            break;
                    }
                }
            }

            #endregion

            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Handles the saving of the game assets file
        /// </summary>
        /// <param name="fileName">The path to save the game aseets at</param>
        private void SaveGameAssets(string fileName)
        {
            Cursor = Cursors.WaitCursor;

            DirectoryInfo dInfo = new DirectoryInfo(contentBuilder.OutputDirectory);
            using (FileStream mStream = new FileStream(Path.Combine(dInfo.Parent.Parent.FullName, "Manifest.xml"), FileMode.Create))
            {
                XmlTextWriter xmlWriter = new XmlTextWriter(mStream, System.Text.Encoding.UTF8);
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.Indentation = 4;
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Manifest");

                // Write the Model Information
                xmlWriter.WriteStartElement("Models");
                foreach (KeyValuePair<string, Model> modelPair in gameManifest.Models)
                {
                    string[] tokens = modelPair.Key.Split('\\');
                    int index = (tokens.Length == 2) ? 1 : 0;
                    xmlWriter.WriteElementString("Name", tokens[index]);
                }
                xmlWriter.WriteEndElement();

                // Write the Texture Information
                xmlWriter.WriteStartElement("Textures");
                foreach (KeyValuePair<string, Texture2D> texturePair in gameManifest.Textures)
                {
                    string[] tokens = texturePair.Key.Split('\\');
                    int index = (tokens.Length == 2) ? 1 : 0;
                    xmlWriter.WriteElementString("Name", tokens[index]);
                }
                xmlWriter.WriteEndElement();

                // Write the Voxel Terrain Information
                xmlWriter.WriteStartElement("VoxelTerrains");
                foreach (KeyValuePair<string, VoxelTerrain> terrainPair in gameManifest.VoxelTerrains)
                {
                    string[] tokens = terrainPair.Key.Split('\\');
                    int index = (tokens.Length == 2) ? 1 : 0;
                    xmlWriter.WriteElementString("Name", terrainPair.Key);
                }
                xmlWriter.WriteEndElement();

                // End the Xml Documents and Flush the Data to the Memory Stream
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();

                // Seek to the beginning of the information in the Memory Stream
                mStream.Seek(0, SeekOrigin.Begin);
                mStream.Flush();

                // Zip up files and save to disk
                using (ZipFile zipFile = new ZipFile())
                {
                    if (Directory.Exists(Path.Combine(contentBuilder.OutputDirectory, @"..\XnbBackups")))
                        zipFile.AddDirectory(Path.Combine(contentBuilder.OutputDirectory, @"..\XnbBackups"));
                    FileStream voxelStream = new FileStream(Path.Combine(dInfo.Parent.Parent.FullName, "TempVoxelData"), FileMode.Create);
                    if (gameManifest.VoxelTerrains.Count > 0)
                    {
                        zipFile.AddDirectoryByName("VoxelTerrains");
                        XmlSerializer voxelSerializer = new XmlSerializer(typeof(VoxelTerrain));
                        foreach (KeyValuePair<string, VoxelTerrain> voxelPair in gameManifest.VoxelTerrains)
                        {
                            voxelStream.Position = 0;
                            voxelStream.SetLength(0);
                            voxelSerializer.Serialize(voxelStream, voxelPair.Value);
                            voxelStream.Seek(0, SeekOrigin.Begin);
                            zipFile.AddEntry(@"VoxelTerrains\" + voxelPair.Key + ".xml", voxelStream);
                        }
                    }
                    zipFile.AddEntry("Manifest.xml", mStream);
                    zipFile.Save(fileName);
                    voxelStream.Close();
                }
            }

            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Event handler for the Import Model menu option.
        /// </summary>
        private void ImportModelMenuClicked(object sender, EventArgs e)
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
            gameManifest.VoxelTerrains.Add(Path.GetFileNameWithoutExtension(fileName), new VoxelTerrain(blocks));
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Loads a new 3D model file into the Game Project and displays
        /// it in the editorViewerControl.
        /// </summary>
        private void LoadModel(string fileName, bool fromBackup = false)
        {
            Cursor = Cursors.WaitCursor;

            // Determine the model's path
            string path = Path.Combine("Models", Path.GetFileNameWithoutExtension(fileName));

            string buildError = null;
            if (!fromBackup)
            {
                // Tell the ContentBuilder what to build.
                contentBuilder.Clear();
                contentBuilder.Add(fileName, path, null, "ModelProcessor");

                // Build this new model data.
                buildError = contentBuilder.Build();
            }

            if (string.IsNullOrEmpty(buildError))
            {
                // If the build succeeded, use the ContentManager to
                // load the temporary .xnb file that we just created.
                Model model = contentManager.Load<Model>(path);
                
                // Display the model in our EditorViewerControl
                if (!fromBackup)
                    editorViewerControl.Model = model;

                // Also store the model in our game manifest
                if (!gameManifest.Models.ContainsKey(path))
                    gameManifest.Models.Add(path, model);
                else
                {
                    gameManifest.Models.Remove(path);
                    gameManifest.Models.Add(path, model);
                }

                // Backup the Xnb files for this object so that they
                // can be used later to save the object
                BackupXnbFiles();
            }
            else
            {
                // If the build failed, display an error message.
                MessageBox.Show(buildError, "Error");
            }

            if (!fromBackup)
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
        private void LoadTexture(string fileName, bool fromBackup = false)
        {
            Cursor = Cursors.WaitCursor;
            
            // Determine the texture's path
            string path = Path.Combine("Textures", Path.GetFileNameWithoutExtension(fileName));

            string buildError = null;
            if (!fromBackup) 
            {
                // Tell the ContentBuilder what to build.
                contentBuilder.Clear();
                contentBuilder.Add(fileName, path, null, "TextureProcessor");

                // Build this new texture data.
                buildError = contentBuilder.Build();
            }

            if (string.IsNullOrEmpty(buildError))
            {
                // If the build succeeded, use the ContentManager to
                // load the temporary .xnb file that we just created.
                Texture2D texture = contentManager.Load<Texture2D>(path);

                // Display the texture in the EditorViewControl
                if (!fromBackup)
                    editorViewerControl.Texture = texture;

                // Store the texture in our game manifest
                if (!gameManifest.Models.ContainsKey(path))
                    gameManifest.Textures.Add(path, texture);
                else
                {
                    gameManifest.Textures.Remove(path);
                    gameManifest.Textures.Add(path, texture);
                }

                // Backup the Xnb files for this object so that they
                // can be used later to save the object
                BackupXnbFiles();
            }
            else
            {
                // If the build failed, display an error message.
                MessageBox.Show(buildError, "Error");
            }

            if (!fromBackup)
                Cursor = Cursors.Arrow;
        }

        private void BackupXnbFiles()
        {
            // Create the backup directory for the Xnb files
            string backupDirectory = Path.Combine(contentBuilder.OutputDirectory, @"..\XnbBackups");
            DirectoryInfo backupDir = new DirectoryInfo(backupDirectory);

            if (!backupDir.Exists)
                backupDir.Create();

            // Get information about the content directory
            DirectoryInfo contentDir = new DirectoryInfo(contentBuilder.OutputDirectory);

            // Copy the Xnb files in the base content directory
            foreach (FileInfo file in contentDir.GetFiles("*.xnb"))
            {
                file.CopyTo(Path.Combine(backupDir.FullName, file.Name),true);
            }
            
            // Copy the Xnb files out of the content folder's subdirectories
            DirectoryInfo[] subDir = contentDir.GetDirectories();
            foreach (DirectoryInfo directory in subDir)
            {
                DirectoryInfo copyDir = new DirectoryInfo(Path.Combine(backupDir.FullName, directory.Name));
                if (!copyDir.Exists)
                    copyDir.Create();
                foreach (FileInfo file in directory.GetFiles("*.xnb"))
                {
                    file.CopyTo(Path.Combine(copyDir.FullName, file.Name), true);
                }
            }
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
