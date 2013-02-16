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
            int deflateMax = 1024 * 64;
            MinecraftChunk[] mc = new MinecraftChunk[1024];
            int mcNum = 0;
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
                int localX = 0, localZ = 0;
                int offset = 0;
                int sectorNumber = 0;
                int chunkL = 0;
                char[] delm = { '\\', '.' };
                string[] fn = fileDialog.FileName.Split(delm);
                for (int i = 0; i < fn.Length; i++)
                {
                    if (fn[i] == "r")
                    {
                        localX = Convert.ToInt32(fn[i+1]);
                        localZ = Convert.ToInt32(fn[i+2]);
                        break;
                    }
                }
                FileStream f = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read);
                f.Flush();
                byte[] hedder = new byte[8 * 1024];
                f.Read(hedder, 0, 8 * 1024);
                int chunkStart = 4 * ((localX & 31) + (localZ & 31) * 32);
                while(chunkStart < 4096){
                    f.Flush();
                    byte[] buffer = new byte[5];
                    f.Seek(chunkStart, SeekOrigin.Begin);
                    f.Read(buffer, 0, 4);
                    sectorNumber = buffer[3];
                    offset = buffer[0] << 16 | buffer[1] << 8 | buffer[2];
                    chunkStart += 4;
                    if (offset != 0)
                    {
                        f.Seek(4096 * offset, SeekOrigin.Begin);
                        f.Read(buffer, 0, 5);
                        chunkL = buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3];
                        byte[] chunk = new byte[chunkL - 1];
                        f.Read(chunk, 0, chunkL - 1);
                        int j = 0;
                        byte[] chunkD = new byte[chunk.Length-6];
                        for (int i = 0; i < chunk.Length - 4; i++)
                        {
                            if (i != 0)
                            {
                                if (i != 1)
                                {
                                    chunkD[j] = chunk[i];
                                    j++;
                                }
                            }
                        }
                        DeflateStream dfs = new DeflateStream(new MemoryStream(chunkD), CompressionMode.Decompress);
                        byte[] dChunk = new byte[chunkD.Length];
                        dfs.Flush();
                        dfs.Read(dChunk, 0, chunkD.Length);
                        mc[mcNum] = new MinecraftChunk(dChunk);
                        mcNum++;
                        dfs.Close();
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
