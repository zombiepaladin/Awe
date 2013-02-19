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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AweEditor.Datatypes;
using System.Collections.Generic;
using System.Text;
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

        int height = 0;
        int length = 0;
        int width = 0;

        byte[] blockByteArray;

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
            OpenFileDialog fd = new OpenFileDialog();

            fd.InitialDirectory = ContentPath();

            fd.Title = "Import Voxel Terrain";

            fd.Filter = "Decompressed Schematic Files (*.*)|*.*";

            if (fd.ShowDialog() == DialogResult.OK)
            {
                LoadVoxelTerrain(fd.FileName);
            }
        }

        private void LoadVoxelTerrain(string fileName)
        {
            Cursor = Cursors.WaitCursor;

            // Switch to the Terrain tab pane
            tabControl1.SelectedIndex = 3;

            //TODO: populate terrain
            //TODO: fix scaling issue
            
            List<TerrainBlockInstance> blocks = new List<TerrainBlockInstance>();

#region Process file
            FileStream fs = new FileStream(fileName, FileMode.Open);
            BinaryReader bReader = new BinaryReader(fs);

            processTag(blocks, bReader);

            bReader.Close();
            fs.Close();
#endregion

            //Extract from data
            /*
            int y, z, x;
            x = y = z = 0;

            for (int i = 0; i < blockByteArray.Length; i++)
            {
                if (blockByteArray[i] != 0)
                    blocks.Add(new TerrainBlockInstance(x * 0.5f, z * 0.5f, y * 0.5f, BlockType.Stone));

                x++;

                if (x % width == 0)
                {
                    x = 0;
                    z++;
                    if (z % length == 0)
                    {
                        z = 0;
                        y++;
                    }
                }
            }*/
             

#region Load Block Model
            //TODO:move model load into VoxelTerrain
            contentManager.Unload();

            // Tell the ContentBuilder what to build.
            contentBuilder.Clear();
            contentBuilder.Add(ContentPath() + "\\Cats.fbx", "Model", null, "InstancedModelProcessor");

            // Build this new model data.
            string buildError = contentBuilder.Build();

            if (string.IsNullOrEmpty(buildError))
            {
                // If the build succeeded, use the ContentManager to
                // load the temporary .xnb file that we just created.
                terrainViewerControl.voxelModel = contentManager.Load<Model>("Model");
            }
            else
            {
                // If the build failed, display an error message.
                MessageBox.Show(buildError, "Error");
            }
#endregion

            //Populate w/ blocks for testing
            for (int i = 0; i < 32; i++)
                for(int j = 0; j < 32; j++)
                    blocks.Add(new TerrainBlockInstance(i * 0.5f, i * 0.5f, j * 0.5f, BlockType.Stone)); 
            

            VoxelTerrain terrain = new VoxelTerrain(blocks);
            terrainViewerControl.VoxelTerrain = terrain;

            Cursor = Cursors.Arrow;
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

        private void processTag(List<TerrainBlockInstance> blocks, BinaryReader reader) {
            byte _byte = reader.ReadByte();

            switch (_byte)
            {
                case 1:
                    processByte(blocks, reader);
                    break;
                 
                case 2:
                    processShort(blocks, reader);
                    break;

                case 3:
                    processInt(blocks, reader);
                    break;

                case 4:
                    processLong(blocks, reader);
                    break;

                case 7:
                    processByteArray(blocks, reader);
                    break;

                case 8:
                    processString(blocks, reader);
                    break;

                case 9:
                    processList(blocks, reader);
                    break;

                case 10:
                    processCompound(blocks, reader);
                    break;
            }
        }

        private string processName(BinaryReader reader)
        {
            UInt16 nameLength = BitConverter.ToUInt16(getBytes(reader, 2), 0);
            return readString(reader, nameLength);
        }

        private void processCompound(List<TerrainBlockInstance> blocks, BinaryReader reader, bool named = true)
        {
            string name;
            if(named)
                name = processName(reader);

            byte _byte = (byte)reader.PeekChar();
            while (_byte != 0) //process children
            {
                processTag(blocks, reader);
                _byte = (byte)reader.PeekChar();
            }

            reader.ReadChar(); //get rid of 0 tag
                    
        }

        private void processString(List<TerrainBlockInstance> blocks, BinaryReader reader, bool named = true)
        {
            string name;
            if (named)
                name = processName(reader);
            
            UInt16 len = BitConverter.ToUInt16(getBytes(reader, 2), 0);
            string s = readString(reader, len);
        }

        private void processByteArray(List<TerrainBlockInstance> blocks, BinaryReader reader, bool named = true)
        {
            string name = "";
            if (named)
                name = processName(reader);

            int BAPayloadCount = BitConverter.ToInt32(getBytes(reader, 4), 0);

            if (BAPayloadCount == 0)
                return;

            if (name.Equals("Blocks")) //use info if blocks
            {
                blockByteArray = new byte[BAPayloadCount];

                for (int i = 0; i < BAPayloadCount; i++)
                {
                    byte BAPayload = reader.ReadByte();
                    blockByteArray[i] = BAPayload;
                }
            }
            else //skip if not blocks
                for (int i = 0; i < BAPayloadCount; i++)
                    reader.ReadByte();
        }

        private void processList(List<TerrainBlockInstance> blocks, BinaryReader reader, bool named = true)
        {
            string name;
            if (named)
                name = processName(reader);
            
            byte listType = reader.ReadByte();
            int listPayloadCount = BitConverter.ToInt32(getBytes(reader, 4), 0);

            if (listPayloadCount == 0)
                return;

            for (int i = 0; i < listPayloadCount; i++)
            {
                switch (listType)
                {
                    case 10:
                        processCompound(blocks, reader, false);
                        break;
                    //TODO: other cases
                }
            }

            //TODO
        }

        private long processLong(List<TerrainBlockInstance> blocks, BinaryReader reader, bool named = true)
        {
            string name;
            if (named)
                name = processName(reader);
            
            return BitConverter.ToInt64(getBytes(reader, 8), 0);
        }

        private Int32 processInt(List<TerrainBlockInstance> blocks, BinaryReader reader, bool named = true)
        {
            string name = "";
            if (named)
                name = processName(reader);

            int val = BitConverter.ToInt32(getBytes(reader, 4), 0);

            if (name.Equals("Height"))
                height = val;
            else if (name.Equals("Width"))
                width = val;
            else if (name.Equals("Length"))
                length = val;

            return val;
        }

        private short processShort(List<TerrainBlockInstance> blocks, BinaryReader reader, bool named = true)
        {
            string name;
            if (named)
                name = processName(reader);
            
            return BitConverter.ToInt16(getBytes(reader, 2), 0);
        }

        private byte processByte(List<TerrainBlockInstance> blocks, BinaryReader reader, bool named = true)
        {
            string name;
            if (named)
                name = processName(reader);
            
            return reader.ReadByte();
        }

        /// <summary>
        /// Gets array of bytes converted to correct endianness
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private byte[] getBytes(BinaryReader reader, int len)
        {
            byte[] bytes = new byte[len];

            if(BitConverter.IsLittleEndian) //reverse bytes
                for (int i = len - 1; i >= 0; i--)
                    bytes[i] = reader.ReadByte();
            else
                for (int i = 0; i < len; i++)
                    bytes[i] = reader.ReadByte();

            return bytes;
        }

        private string readString(BinaryReader reader, int len)
        {
            string val = "";

            for (int i = 0; i < len; i++)
                val += reader.ReadChar();

            return val;
        }
    }
}
