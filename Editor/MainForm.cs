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
using System.IO.Compression;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AweEditor.Datatypes;
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
            // TODO: Load game manifest and associated game resources from file
            throw new NotImplementedException();
        }

        /// <summary>
        /// Event handler for the Save menu option
        /// </summary>
        void SaveMenuClicked(object sender, EventArgs e)
        {
            if (saveLocation == null)
            {
                SaveAsMenuClicked(null, null);
            }
            else
            {
                SaveGameManifest(saveLocation);
            }
        }

        /// <summary>
        /// Event handler for the SaveAs menu option
        /// </summary>
        void SaveAsMenuClicked(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = ContentPath();

            fileDialog.Title = "Load Game Data File";

            fileDialog.Filter = "Awe Game Data Files (*.awed)|*.awed";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                // If save was successful, store the filepath
                // so that we can use the save button later.
                if (SaveGameManifest(fileDialog.FileName))
                {
                    saveLocation = fileDialog.FileName;
                }
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
        /// Handles the serialization and saving of the game manifest
        /// </summary>
        /// <param name="fileName">The path to save the game manifest at</param>
        /// <returns>A boolean indicating whether the save was successful</returns>
        private bool SaveGameManifest(string fileName)
        {
            // TODO: Serialization of game data

            // TODO: Zip up files and save to disk

            return false;
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
                LoadModel(fileDialog.FileName);
            }
        }

        /// <summary>
        /// Loads a new minecraft terrain file into the TerrainViewerControl.
        /// </summary>
        private void ImportVoxelTerrainMenuClicked(object sender, EventArgs e)
        {
            // TODO: Import the file
        }

        /// <summary>
        /// Loads a new 3D model file into the Game Project and displays
        /// it in the editorViewerControl.
        /// </summary>
        private void LoadModel(string fileName)
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
                LoadTexture(fd.FileName);
            }

        }

        /// <summary>
        /// Loads a new a texture from a file into the game project
        /// and displays it in the editorViewerControl
        /// </summary>
        /// <param name="fileName">The texture file to import</param>
        private void LoadTexture(string fileName)
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
    }
}
