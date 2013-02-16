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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO.Compression;
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

        /// <summary>
        /// Constructs the main form.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            contentBuilder = new ContentBuilder();

            contentManager = new ContentManager(modelViewerControl.Services,
                                                contentBuilder.OutputDirectory);

            /// Automatically bring up the "Load Model" dialog when we are first shown.
            ///this.Shown += OpenMenuClicked;
        }


        /// <summary>
        /// Event handler for the Exit menu option.
        /// </summary>
        void ExitMenuClicked(object sender, EventArgs e)
        {
            Close();
        }


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
                LoadModel(fileDialog.FileName);
            }
        }

        private static string ContentPath()
        {
            // Default to the directory which contains our content files.
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string relativePath = Path.Combine(assemblyLocation, "../../../../Content");
            string contentPath = Path.GetFullPath(relativePath);
            return contentPath;
        }

        /// <summary>
        /// Loads a new minecraft terrain file into the TerrainViewerControl.
        /// </summary>
        private void ImportVoxelTerrainMenuClicked(object sender, EventArgs e)
        {
            MinecraftChunk[] mc = new MinecraftChunk[1024];

            int[] chunkLocation = new int[1024];

            int[] chunkSize = new int[1024];

            for (int i = 0; i < 1024; i++)
            {
                mc[i] = null;
                chunkLocation[i] = -1;
                chunkSize[i] = -1;
            }
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = ContentPath();

            fileDialog.Title = "Load MinecraftFile";

            fileDialog.InitialDirectory = "%appdata%";

            fileDialog.Filter = "Region Files (*.mca)|*.mca|" +
                                "Level Files (*.dat)|*.dat|" +
                                "All Files (*.*)|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream f = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read);
                f.Flush();
                byte[] bi= new byte[(int)f.Length];
                f.Read(bi,0,(int)f.Length);
                byte[] biH = new byte[8192];
                byte[] biE = new byte[bi.Length - 8192];
                int j = 0;
                int k = 0;
                for (int i = 0; i < 8192; i++)
                {
                    biH[i] = bi[j];
                    j++;
                }
                int localX = 0 >> 5;
                int localZ = 0 >> 5;
                for (int i = 0; i < biE.Length; i++)
                {
                    biE[i] = bi[j];
                    j++;
                }
                j = 0;
                while (k < 4096)
                {
                    chunkLocation[j] = biH[k] << 16 | biH[k + 1] << 8 | biH[k + 2];
                    chunkSize[j] = biH[k + 3];
                    k += 4;
                    j++;
                }
                j = 0;
                k = 0;
                
                while (k < 1024)
                {
                    if (chunkSize[k] == 0)
                    {
                        k++;
                    }
                    else
                    {
                        j = chunkLocation[k];
                        int cL = biE[j] << 24 | biE[j + 1] << 16 | biE[j + 2] << 8 | biE[j + 3];
                    }
                }
                f.Close();
        
            }

        }


        /// <summary>
        /// Loads a new 3D model file into the ModelViewerControl.
        /// </summary>
        void LoadModel(string fileName)
        {
            Cursor = Cursors.WaitCursor;

            // Switch to the Model tab pane
            tabControl1.SelectedIndex = 1;

            // Unload any existing model.
            modelViewerControl.Model = null;
            contentManager.Unload();

            // Tell the ContentBuilder what to build.
            contentBuilder.Clear();
            contentBuilder.Add(fileName, "Model", null, "ModelProcessor");

            // Build this new model data.
            string buildError = contentBuilder.Build();

            if (string.IsNullOrEmpty(buildError))
            {
                // If the build succeeded, use the ContentManager to
                // load the temporary .xnb file that we just created.
                modelViewerControl.Model = contentManager.Load<Model>("Model");
            }
            else
            {
                // If the build failed, display an error message.
                MessageBox.Show(buildError, "Error");
            }

            Cursor = Cursors.Arrow;
        }

        private void ImportImageClicked(object sender, EventArgs e)
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

        protected void LoadTexture(string fileName)
        {
            Cursor = Cursors.WaitCursor;

            // Switch to the Texture tab pane
            tabControl1.SelectedIndex = 5;

            // Unload any existing texture.
            textureViewerControl.Texture = null;
            contentManager.Unload();

            // Tell the ContentBuilder what to build.
            contentBuilder.Clear();
            contentBuilder.Add(fileName, "Texture", null, "TextureProcessor");

            // Build this new texture data.
            string buildError = contentBuilder.Build();

            if (string.IsNullOrEmpty(buildError))
            {
                // If the build succeeded, use the ContentManager to
                // load the temporary .xnb file that we just created.
                textureViewerControl.Texture = contentManager.Load<Texture2D>("Texture");
            }
            else
            {
                // If the build failed, display an error message.
                MessageBox.Show(buildError, "Error");
            }


            Cursor = Cursors.Arrow;
        }
    }
}
