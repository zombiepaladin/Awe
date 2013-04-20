using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

//Temporary
using System.Windows.Forms;
using AweEditor.Utilities;

namespace AweEditor
{
    public enum TagType : byte
    {
        TAG_End = 0,
        TAG_Byte = 1,
        TAG_Short = 2,
        TAG_Int = 3,
        TAG_Long = 4,
        TAG_Float = 5,
        TAG_Double = 6,
        TAG_Byte_Array = 7,
        TAG_String = 8,
        TAG_List = 9,
        TAG_Compound = 10,
        TAG_Int_Array = 11
    }

    public enum FileType : byte
    {
        FILE_Schematic,
        FILE_MCA,
        FILE_MCR,
    }

    /// <summary>
    /// Represents a single nbt tag, may contain others as children
    /// </summary>
    public class NamedBinaryTag
    {
        /// <summary>
        /// The type of tag that this is, can only be set by contstructor
        /// </summary>
        public TagType Type { get { return _type; } }

        /// <summary>
        /// The assigned name of this tag, may be empty
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Payload that this tag has
        /// </summary>
        public object Payload { get; set; }


        /// <summary>
        /// Maintains only getter attributes of this property
        /// </summary>
        private TagType _type;



        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">The type of tag this will be</param>
        public NamedBinaryTag(TagType type)
        {
            this._type = type;
            this.Name = "None";
            this.Payload = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">The type of tag this will be</param>
        /// <param name="payload">The payload of this tag</param>
        public NamedBinaryTag(TagType type, object payload)
        {
            this._type = type;
            this.Name = "None";
            this.Payload = payload;
        }

        /// <summary>
        /// Will automatically make an empty TAG_Compound with no children
        /// </summary>
        public NamedBinaryTag()
        {
            this._type = TagType.TAG_Compound;
            this.Name = "None";
            this.Payload = new List<NamedBinaryTag>();
        }


        //Getters for the various tags that can be found
        public byte GetByte() { return (byte)Payload; }
        public short GetShort() { return (short)Payload; }
        public int GetInt() { return (int)Payload; }
        public long GetLong() { return (long)Payload; }
        public float GetFloat() { return (float)Payload; }
        public double GetDouble() { return (double)Payload; }
        public byte[] GetByteArray() { return (byte[])Payload; }
        public string GetString() { return (string)Payload; }
        public List<NamedBinaryTag> GetList() { return (List<NamedBinaryTag>)Payload; }
        public List<NamedBinaryTag> GetCompound() { return (List<NamedBinaryTag>)Payload; }
        public int[] GetIntArray() { return (int[])Payload; }
    }

    public struct Chunk
    {
        public Vector2 Position;
        public NamedBinaryTag ByteArray;

        public Chunk(Vector2 pos, NamedBinaryTag nbt)
        {
            Position = pos;
            ByteArray = nbt;
        }
    }

    public static class VoxelTerrainImporter
    {
        //Stores the type of the file the importer is currently importing
        private static FileType fileType;

        //TODO: Delete
        private static TextBox status = null;
        private static short statusIndention = 0;
        public static void SetStatus(TextBox tb)
        {
            status = tb;
        }
        public static void UpdateStatus(string message)
        {
            string prepend = "|";
            for (int i = 0; i < statusIndention * 2; i++)
            {
                prepend += "_";
            }

            Debug.WriteLine("{0}>{1}", prepend, message);
        }

        private static string ByteString(byte b)
        {
            return Convert.ToString(b, 2).PadLeft(8, '0');
        }

        private enum CompressionType
        {
            GZip = 0,
            Zlib = 1
        }

        private struct ChunkInformation
        {
            public int Offset;
            public short Size;
            public int ByteLength;
            public CompressionType Compression;
        }

        /// <summary>
        /// A method of correcting odd endian problems
        /// </summary>
        /// <param name="x">the value to correct</param>
        /// <returns>the corrected value</returns>
        private static short CorrectEndian(short x)
        {
            return IPAddress.NetworkToHostOrder(x);
        }
        /// <summary>
        /// A method of correcting odd endian problems
        /// </summary>
        /// <param name="x">the value to correct</param>
        /// <returns>the corrected value</returns>
        private static int CorrectEndian(int x)
        {
            return IPAddress.NetworkToHostOrder(x);
        }
        /// <summary>
        /// A method of correcting odd endian problems
        /// </summary>
        /// <param name="x">the value to correct</param>
        /// <returns>the corrected value</returns>
        private static long CorrectEndian(long x)
        {
            return IPAddress.NetworkToHostOrder(x);
        }

        public static List<BlockData> GenerateBlocks(List<Chunk> chunkList, bool ignoreAir = true)
        {
            List<BlockData> blocks = new List<BlockData>();

            short width;

            if (fileType == FileType.FILE_MCR)
                width = 128;
            else
                width = 256;

            short length = 16;

            foreach (Chunk chunk in chunkList)
            {
                if (fileType == FileType.FILE_MCR)
                {
                    byte[] blockArray = chunk.ByteArray.GetByteArray();

                    if (blockArray == null)
                    {
                        //TODO:show error reading file
                        return new List<BlockData>();
                    }

                    makeHollow(length, width, 16, blockArray); //can only use this for mcr right now, mca doesnt use same array

                    //variables used in loop
                    short y, z, x;
                    x = y = z = 0;

                    BlockData blockData = new BlockData();

                    for (int i = 0; i < blockArray.Length; i++)
                    {
                        if (!ignoreAir || blockArray[i] != 0)
                        {
                            blockData.x = y + ((int)chunk.Position.X * 16); //Swapping axis so that the chunk appears right side up
                            blockData.y = x;
                            blockData.z = z + ((int)chunk.Position.Y * 16); //Working to add the chunk position to the block array
                            blockData.type = blockArray[i];

                            if (blockData.y > 30)
                                blocks.Add(blockData);

                        }

                        //simulate 3D array
                        x++;
                        if (x == width)
                        {
                            x = 0;
                            z++;
                            if (z == length)
                            {
                                z = 0;
                                y++;
                            }
                        }

                    }
                }
                else
                {
                    byte[, ,] blockArray = (byte[, ,])chunk.ByteArray.Payload;

                    makeHollow(length, width, 16, blockArray);

                    BlockData blockData = new BlockData();
                    for (int x = 0; x < 16; x++)
                        for (int y = 0; y < 256; y++)
                            for (int z = 0; z < 16; z++)
                            {
                                if (blockArray[x, y, z] != 0)
                                {
                                    blockData.x = x + ((int)chunk.Position.X * 16); //Swapping axis so that the chunk appears right side up
                                    blockData.y = y;
                                    blockData.z = z + ((int)chunk.Position.Y * 16); //Working to add the chunk position to the block array
                                    blockData.type = blockArray[x, y, z];

                                    blocks.Add(blockData);
                                }
                            }
                }
            }

            return blocks;
        }

        private static void makeHollow(int length, int width, int height, byte[] blockArray)
        {
            //TODO: consider transparent blocks when implemented

            if (blockArray == null)
                return;

            byte[, ,] blocks = unflattenBlockArray(length, width, height, blockArray);

            //1 to edge - 1 because edges are never surrounded
            for (int y = 1; y < height - 1; y++)
                for (int z = 1; z < length - 1; z++)
                    for (int x = 1; x < width - 1; x++)
                    {
                        if (blocks[y - 1, z, x] != 0 && blocks[y + 1, z, x] != 0 && //Check y
                            blocks[y, z - 1, x] != 0 && blocks[y, z + 1, x] != 0 && //Check z
                            blocks[y, z, x - 1] != 0 && blocks[y, z, x + 1] != 0) //Check x
                        {
                            blockArray[y * (length * width) + z * (width) + x] = 0;
                        }
                    }
        }

        //used for 3d arrays
        private static void makeHollow(int xMax, int yMax, int zMax, byte[, ,] blockArray)
        {
            //TODO: consider transparent blocks when implemented

            if (blockArray == null)
                return;

            for (int x = 1; x < xMax - 1; x++)
                for (int y = 1; y < yMax - 1; y++)
                    for (int z = 1; z < zMax - 1; z++)
                    {
                        if (blockArray[x - 1, y, z] != 0 && blockArray[x + 1, y, z] != 0 &&
                           blockArray[x, y - 1, z] != 0 && blockArray[x, y + 1, z] != 0 &&
                           blockArray[x, y, z - 1] != 0 && blockArray[x, y, z + 1] != 0)
                            blockArray[x, y, z] = 0;
                    }
        }

        private static byte[, ,] unflattenBlockArray(int length, int width, int height, byte[] blockArray)
        {
            byte[, ,] unflattenedBlockArray = new byte[height, length, width];

            short y, z, x;
            x = y = z = 0;

            BlockData blockData = new BlockData();

            for (int i = 0; i < blockArray.Length; i++)
            {
                if (blockArray[i] != 0) //ignore air blocks
                {
                    blockData.x = x;
                    blockData.y = y;
                    blockData.z = z;

                    unflattenedBlockArray[y, z, x] = blockArray[i];
                    //blocks.Add(new TerrainBlockInstance(x * 0.5f, y * 0.5f, z * 0.5f, BlockType.Stone)); //TODO: fix hardcoded scaling
                }

                //simulate 3D array
                x++;
                if (x == width)
                {
                    x = 0;
                    z++;
                    if (z == length)
                    {
                        z = 0;
                        y++; //y is leftmost index so it won't need to cycle
                    }
                }
            }

            return unflattenedBlockArray;
        }

        public static List<BlockData> ImportBlockData(string filePath)
        {
            List<BlockData> blocks;
            string extension = Path.GetExtension(filePath).ToLower();

            switch (extension)
            {
                case ".schematic":
                    fileType = FileType.FILE_Schematic;
                    SchematicProcessor schematicProcessor = new SchematicProcessor(filePath);
                    blocks = schematicProcessor.generateBlockData();
                    break;

                case ".mcr":
                    fileType = FileType.FILE_MCR;
                    List<Chunk> chunkList = VoxelTerrainImporter.LoadTerrain(filePath);
                    blocks = VoxelTerrainImporter.GenerateBlocks(chunkList);
                    break;
                case ".mca":
                    fileType = FileType.FILE_MCA;
                    chunkList = VoxelTerrainImporter.LoadTerrain(filePath);
                    blocks = VoxelTerrainImporter.GenerateBlocks(chunkList);
                    break;

                default:
                    MessageBox.Show(String.Format("The {0} format is not accepted - Aborting", extension));
                    blocks = null;
                    break;
            }

            return blocks;
        }

        /// <summary>
        /// Loads in the terrain if possible
        /// </summary>
        /// <param name="filepath">the path to the file to import</param>
        private static List<Chunk> LoadTerrain(string filePath)
        {
            return ParseRegionFile(filePath);
        }

        private static List<Chunk> ParseRegionFile(string filePath)
        {
            ChunkInformation[] chunkInfo = new ChunkInformation[1024]; //There is exactly 1024 chunks in a region, 32x32
            byte[] readBlock = new byte[4096];
            short currentIndex = 0;

            string outputString = "";

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            fs.Read(readBlock, 0, 4096);
            fs.Close();

            for (int i = 0; i < 1024; i++)
            {
                string[] test = new string[4];
                test[0] = ByteString(readBlock[currentIndex]);
                test[1] = ByteString(readBlock[currentIndex + 1]);
                test[2] = ByteString(readBlock[currentIndex + 2]);
                test[3] = ByteString(readBlock[currentIndex + 3]);

                //Reading in first 3 bytes as an integer
                chunkInfo[i].Offset = (readBlock[currentIndex] << 16) + (readBlock[currentIndex + 1] << 8) + readBlock[currentIndex + 2];
                currentIndex += 3; // Advancing the read marker past the first 3 bytes

                chunkInfo[i].Size = readBlock[currentIndex]; //Reading in the last byte
                currentIndex++; // Advancing the read marker by one byte

                if ((chunkInfo[i].Offset + chunkInfo[i].Size) != 0)
                    //outputString += "Chunk " + i + ": " + chunkInfo[i].Offset + " [" + chunkInfo[i].Size + "]\t" + test[0] + "  " + test[1] + "  " + test[2] + "  " + test[3] + Environment.NewLine;
                    outputString += String.Format("Chunk {0,4}:{1,4} [{2,1}]{3,10}{4,10}{5,10}{6,10}", i, chunkInfo[i].Offset, chunkInfo[i].Size, test[0], test[1], test[2], test[3]) + Environment.NewLine;
            }

            return ParseChunks(filePath, chunkInfo);
        }

        private static List<Chunk> ParseChunks(string filePath, ChunkInformation[] chunkInfo)
        {
            int offsetIndex, currentIndex;
            byte[] readBlock, compressedBlock, decompressedBlock;

            List<Chunk> chunkList = new List<Chunk>();
            //VoxelTerrain terrain = new VoxelTerrain();

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            for (int i = 0; i < chunkInfo.Length; i++)
            {
                //We don't care if the chunk doesn't exist, as is indicated by a 0 for offset and length
                if (chunkInfo[i].Offset == 0 && chunkInfo[i].Size == 0)
                    continue;

                //Calculating offset, which is set in 4096 byte sections
                offsetIndex = chunkInfo[i].Offset * 4096;
                currentIndex = 0;

                // We know the size of our chunk data, which is also in 4096 byte sections
                readBlock = new byte[chunkInfo[i].Size * 4096];

                //Now we can populate our block
                fs.Seek(offsetIndex, SeekOrigin.Begin);
                fs.Read(readBlock, 0, chunkInfo[i].Size * 4096);

                //Now that we have our chunk data read in, we can parse it

                //First is the length in bytes of the data
                chunkInfo[i].ByteLength = CorrectEndian(BitConverter.ToInt32(readBlock, currentIndex));

                //That was 4 bytes of data, so we can push our index in further
                currentIndex += 4;

                //We should probably set up the size of our compressed array as well
                compressedBlock = new byte[chunkInfo[i].ByteLength - 1];

                //The next byte should be our compression type
                chunkInfo[i].Compression = (CompressionType)readBlock[currentIndex];

                //Only 1 byte forward this time
                currentIndex++;

                if (chunkInfo[i].Compression == CompressionType.GZip)
                {
                    //Now we can just grab our compressed chunk data
                    Array.Copy(readBlock, currentIndex, compressedBlock, 0, chunkInfo[i].ByteLength - 1);

                    //and of course decompress it
                    decompressedBlock = DecompressGZip(compressedBlock);
                }
                else
                {
                    //Here we're snagging the compressed chunk data
                    //Since we're decompressing with Deflate, rather than Zlib, we need to cut off the 
                    //header tags. To do this we remove the first 2 (index + 2) and last 4 bytes (length - 4)
                    Array.Copy(readBlock, currentIndex + 2, compressedBlock, 0, chunkInfo[i].ByteLength - 1 - 4);

                    //Now we can finally decompress
                    decompressedBlock = DecompressZLib(compressedBlock);
                }

                //now the decompressed data and the chunk Info are ready to port out to an NBT parser
                if (chunkInfo[i].Offset != 0 && chunkInfo[i].Size != 0)
                {
                    NamedBinaryTag topLevel = new NamedBinaryTag();

                    GetTagList(decompressedBlock, 0, topLevel);

                    if (fileType == FileType.FILE_MCR)
                    {
                        NamedBinaryTag xPos = FindTag("xPos", topLevel);
                        NamedBinaryTag zPos = FindTag("zPos", topLevel);
                        NamedBinaryTag Blocks = FindTag("Blocks", topLevel);

                        if (xPos == null || zPos == null || Blocks == null)
                            throw new NullReferenceException("One or more tags not found");
                        else
                        {
                            chunkList.Add(new Chunk(new Vector2((float)xPos.GetInt(), (float)zPos.GetInt()), Blocks));
                        }
                    }
                    else
                    {
                        NamedBinaryTag xPos = FindTag("xPos", topLevel);
                        NamedBinaryTag zPos = FindTag("zPos", topLevel);
                        NamedBinaryTag Blocks;
                        NamedBinaryTag sectionsTag = FindTag("Sections", topLevel); //this finds a tag, but the value is always 0 even though nbt viewer shows 0,1,2,etc.

                        if (xPos == null || zPos == null || sectionsTag == null)
                            throw new NullReferenceException("One or more tags not found");
                        else
                        {
                            List<NamedBinaryTag> sections = sectionsTag.GetList();
                            byte[] sectionBlockArray = new byte[16 * 16 * 16];
                            byte[, ,] chunkBlockArray = new byte[16, 256, 16]; //start by populating with [x,y,z] since its easier to understand
                            int yOffset;

                            foreach (NamedBinaryTag section in sections)
                            {
                                yOffset = FindTag("Y", section).GetByte() * 16;
                                sectionBlockArray = FindTag("Blocks", section).GetByteArray();

                                for (int y = 0; y < 16; y++)
                                    for (int z = 0; z < 16; z++)
                                        for (int x = 0; x < 16; x++)
                                            chunkBlockArray[x, y + yOffset, z] = sectionBlockArray[(y * 16 + z) * 16 + x];
                            }

                            //chunkBlockArray contains all blocks from its sections in [x,y,z] coordinates/locations
                            //now convert to a flattened array ordered XZY so that it matches the mcr style that this importer is set up for

                            /*
                            byte[] flattenedChunkBlockArray = new byte[16 * 16 * 256];

                            for (int y = 0; y < 256; y++)
                                for (int z = 0; z < 16; z++)
                                    for (int x = 0; x < 16; x++)
                                        flattenedChunkBlockArray[(x * 16 + z) * 16 + y] = chunkBlockArray[x, y, z];

                            //horribly inefficient conversion complete, now load it like an mcr file.
                            */

                            //cheat and just send 3d array, process differently in generateBlocks
                            Blocks = new NamedBinaryTag(TagType.TAG_Byte_Array, chunkBlockArray);
                            chunkList.Add(new Chunk(new Vector2((float)xPos.GetInt(), (float)zPos.GetInt()), Blocks));

                        }
                    }
                }
            }

            fs.Close();
            UpdateStatus("Done!");

            return chunkList;
        }

        //Code copied from http://www.dotnetperls.com/decompress
        /// <summary>
        /// Decompresses a gzipped file, what most .nbt and .schematic files are
        /// </summary>
        /// <param name="gzip">the array of bytes of the compressed file</param>
        /// <returns>an uncompressed array of bytes representing the original file</returns>
        private static byte[] DecompressGZip(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);

                    return memory.ToArray();
                }
            }
        }

        private static byte[] DecompressZLib(byte[] zlib)
        {
            using (DeflateStream stream = new DeflateStream(new MemoryStream(zlib), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    } while (count > 0);

                    return memory.ToArray();
                }
            }
        }

        //WARNING: Imminent Recursion
        /// <summary>
        /// Prints out a list of all tags in the file, those without a name
        /// will have the name "None"
        /// </summary>
        /// <param name="nbtChunk">The nbt formatted binary file</param>
        /// <param name="currentIndex">the current position</param>
        /// <param name="parentStruct">Non-null only when an immediate child of a list or compound, 
        /// allows a compilation of sub tags to be made in a structure similar to a tree or 
        /// directory</param>
        /// <param name="parsingList">Indicates whether we currently parsing specifically a list or not;
        /// necessary to avoid checking names that don't exist</param>
        /// <returns>the position ended upon</returns>
        private static int GetTagList(byte[] nbtChunk, int currentIndex, NamedBinaryTag parentStruct = null, bool parsingList = false, TagType listIDTag = TagType.TAG_End)
        {
            //Our new tag, but we'll need its tag type first
            NamedBinaryTag newNBT;

            //Let's read in the tag where we're at
            TagType currentTag;

            if (!parsingList)
            {
                //Reading the current tag
                currentTag = (TagType)nbtChunk[currentIndex];

                //And of course we'll increment our currentIndex
                //We don't want to do this if we're in a list
                //because we're not actually reading the data
                currentIndex++;
            }
            else
                currentTag = listIDTag;

            //Now we can initialize our new tag
            newNBT = new NamedBinaryTag(currentTag);

            /**************************/
            /*  Searching For a Name  */
            /**************************/

            //Lets first check for a name, since most tags have one
            //We'll need to make sure its not an End or that we're in a List first
            if (currentTag != TagType.TAG_End && !parsingList)
            {
                //Then we need to find the name length
                short nameLength = CorrectEndian(BitConverter.ToInt16(nbtChunk, currentIndex));
                currentIndex += 2; //We read 2 bytes, so lets take care of it before we forget

                //Not outta the woods yet, gotta see if the name is actually there
                if (nameLength > 0)
                {
                    //Alright, so we've got a name, now we can decode it and get outta dere
                    newNBT.Name = Encoding.UTF8.GetString(nbtChunk, currentIndex, nameLength);

                    //And can't forget to keep on crawling
                    currentIndex += nameLength;
                }
            }

            /**************************/
            /*       Parsing Tag      */
            /**************************/

            //Next we need to figure out what to do
            //So let's have a look at that tag
            switch (currentTag)
            {
                // ID Type = Payload

                // 0 TAG_End = None [No name]
                case TagType.TAG_End:

                    //Not too much to worry about here, just need
                    // to move on back up the recursive chain
                    currentIndex++;

                    break;

                // 1 TAG_Byte = 1 byte signed
                case TagType.TAG_Byte:

                    //Pretty straightforward, just a byte
                    newNBT.Payload = nbtChunk[currentIndex];

                    //And crawling along
                    currentIndex++;

                    break;

                // 2 TAG_Short = 2 bytes, signed, big endian
                case TagType.TAG_Short:

                    //Similar as before
                    newNBT.Payload = CorrectEndian(BitConverter.ToInt16(nbtChunk, currentIndex));

                    //Still moving along
                    currentIndex += 2;

                    break;

                // 3 TAG_Int = 4 bytes, signed, big endian
                case TagType.TAG_Int:

                    //Same old drill
                    newNBT.Payload = CorrectEndian(BitConverter.ToInt32(nbtChunk, currentIndex));

                    //Crawling along
                    currentIndex += 4;

                    break;

                // 4 TAG_Long = 8 bytes, signed, big endian
                case TagType.TAG_Long:

                    //Getting a little bigger
                    newNBT.Payload = CorrectEndian(BitConverter.ToInt64(nbtChunk, currentIndex));

                    //8 more bytes down the road
                    currentIndex += 8;

                    break;

                // 5 TAG_Float = 4 bytes, signed, big endian
                case TagType.TAG_Float:

                    //TODO: Check the endianness of this
                    //Floats now, a bit different though, had to make a separate array, thanks
                    //to this system being little-endian
                    newNBT.Payload = BitConverter.ToSingle(new byte[4]
                                                                    {
                                                                        nbtChunk[currentIndex],
                                                                        nbtChunk[currentIndex + 1],
                                                                        nbtChunk[currentIndex + 2],
                                                                        nbtChunk[currentIndex + 3]
                                                                    }, 0);
                    //Still counting here though
                    currentIndex += 4;

                    break;

                // 6 TAG_Double = 8 bytes, signed, big endian
                case TagType.TAG_Double:

                    //We will have to do things the same here as for TAG_Float
                    newNBT.Payload = BitConverter.ToDouble(new byte[8]
                                                                    {
                                                                        nbtChunk[currentIndex],
                                                                        nbtChunk[currentIndex + 1],
                                                                        nbtChunk[currentIndex + 2],
                                                                        nbtChunk[currentIndex + 3],
                                                                        nbtChunk[currentIndex + 4],
                                                                        nbtChunk[currentIndex + 5],
                                                                        nbtChunk[currentIndex + 6],
                                                                        nbtChunk[currentIndex + 7]
                                                                    }, 0);

                    //Can't forget this wonderful counting
                    currentIndex += 8;
                    break;

                // 7 TAG_Byte_Array = [Int] size for size # of [Byte] payloads
                case TagType.TAG_Byte_Array:
                    {
                        //Alright, switching things up a bit (every. pun. intended.)

                        //First we need the size of the array, it'll be in the next 4 bytes
                        int size = CorrectEndian(BitConverter.ToInt32(nbtChunk, currentIndex));

                        //Tick
                        currentIndex += 4;

                        //Lets first make sure that our Payload can contain the bytes
                        newNBT.Payload = new byte[size];

                        //Now we can apply it and get all of that glorious information
                        Array.Copy(nbtChunk, currentIndex, (byte[])newNBT.Payload, 0, size);

                        //And finally we can increment our count
                        currentIndex += size;

                        break;
                    }

                // 8 TAG_String = [Short] size for size # of [UTF-8] characters
                case TagType.TAG_String:
                    {
                        //So lets start with the size
                        int size = CorrectEndian(BitConverter.ToInt16(nbtChunk, currentIndex));

                        //Increment
                        currentIndex += 2;

                        //Then we can populate our payload
                        newNBT.Payload = Encoding.UTF8.GetString(nbtChunk, currentIndex, size);

                        //And increment yet again
                        currentIndex += size;

                        break;
                    }

                // 9 TAG_List = [Byte] for tagID, [Int] for size, size # of payloads of type tagID. [contains unnamed tags]
                case TagType.TAG_List:
                    {
                        //First we need to know the kind of data we'll be looking at
                        TagType tagID = (TagType)nbtChunk[currentIndex];

                        //Then we'll do some bookeeping
                        currentIndex++;

                        //Next we need our size
                        int size = CorrectEndian(BitConverter.ToInt32(nbtChunk, currentIndex));

                        //And Increment once again
                        currentIndex += 4;

                        //It's probably a good idea here to initialize our payload
                        newNBT.Payload = new List<NamedBinaryTag>();

                        //Finally we need to actually parse the remaining tags
                        //And so we come across our first tree-like structure
                        for (int i = 0; i < size; i++)
                        {
                            currentIndex = GetTagList(nbtChunk, currentIndex, newNBT, true, tagID);
                        }

                        //That should take care of things and auto-increment our index due
                        //to the magic that is recursion

                        break;
                    }

                // 10 TAG_Compound = Any number of fully formed named binary tags, terminates with a TAG_End
                case TagType.TAG_Compound:
                    {
                        //First we'll make the variable we'll need for our loop
                        bool foundEnd = false;

                        //Since the Coumpounds has an indeterminant payload size
                        //we run the risk of hitting an infinite loop so we'll use a 
                        //sanity check variable
                        int infiniteGuard = 0;

                        //Next we'll want to be able to peek at the tag that we're on
                        //after each iteration, so we need keep track of it
                        TagType tagID;

                        //It's probably a good idea here to initialize our payload
                        newNBT.Payload = new List<NamedBinaryTag>();

                        //Finally we're ready for our loop, nothing complicated, we just need to be careful
                        while (!foundEnd)
                        {
                            //First we peek at our current position
                            tagID = (TagType)nbtChunk[currentIndex];

                            //And then do some more sanity checks
                            if (tagID < Enum.GetValues(typeof(TagType)).Cast<TagType>().Min() ||
                                tagID > Enum.GetValues(typeof(TagType)).Cast<TagType>().Max()) throw new IndexOutOfRangeException("Non-Existent tag detected");

                            if ((TagType)tagID == TagType.TAG_End)
                            {
                                foundEnd = true;
                                //Since we don't actually parse this tag, we need to increment past it
                                currentIndex++;
                                break;
                            }

                            //Then we can actually start diving into the recursion
                            currentIndex = GetTagList(nbtChunk, currentIndex, newNBT);

                            //TODO:Change the Status route, throw an error
                            //Here we need to check our guard (and increment it)
                            if (infiniteGuard++ > int.MaxValue - 4) { UpdateStatus("Woah! Infinite Loop Dected, pulling out"); break; }
                        }

                        break;
                    }

                // 11 TAG_Int_Array = [Int] size for size # of [Int] payloads
                case TagType.TAG_Int_Array:
                    {
                        //And back to something simple with a size:
                        int size = CorrectEndian(BitConverter.ToInt32(nbtChunk, currentIndex));

                        //The Incrementing
                        currentIndex += 4;

                        //The making sure the payload can hold it
                        newNBT.Payload = new int[size];

                        //The temp array that we'll be populating...oh wait that's new
                        int[] tempArray = new int[size];

                        //The Populating
                        for (int i = 0; i < size; i++)
                        {
                            //With the Converting
                            tempArray[i] = CorrectEndian(BitConverter.ToInt32(nbtChunk, currentIndex));

                            //And more Incrementing
                            currentIndex += 4;
                        }

                        //The Copying
                        tempArray.CopyTo((Array)newNBT.Payload, 0);


                        break;
                    }

                default:
                    throw new IndexOutOfRangeException("A non-existent tag was found");
            }

            //Now we need to handle the actual payload of the tree tags
            if (parentStruct != null)
            {
                //First we need a temp list
                List<NamedBinaryTag> tempList = (List<NamedBinaryTag>)parentStruct.Payload;
                tempList.Add(newNBT);
            }

            //Now that we're done here, we can let everyone else know where we stopped
            return currentIndex;
        }

        private static NamedBinaryTag FindTag(string tagName, NamedBinaryTag currentTag)
        {
            switch (currentTag.Type)
            {
                // ID Type = Payload

                // 0 TAG_End = None [No name]
                case TagType.TAG_End:
                    return null;

                // 1 TAG_Byte = 1 byte signed
                case TagType.TAG_Byte:
                // 2 TAG_Short = 2 bytes, signed, big endian
                case TagType.TAG_Short:
                // 3 TAG_Int = 4 bytes, signed, big endian
                case TagType.TAG_Int:
                // 4 TAG_Long = 8 bytes, signed, big endian
                case TagType.TAG_Long:
                // 5 TAG_Float = 4 bytes, signed, big endian
                case TagType.TAG_Float:
                // 6 TAG_Double = 8 bytes, signed, big endian
                case TagType.TAG_Double:
                // 7 TAG_Byte_Array = [Int] size for size # of [Byte] payloads
                case TagType.TAG_Byte_Array:
                // 8 TAG_String = [Short] size for size # of [UTF-8] characters
                case TagType.TAG_String:
                // 11 TAG_Int_Array = [Int] size for size # of [Int] payloads
                case TagType.TAG_Int_Array:
                    if (currentTag.Name == tagName)
                        return currentTag;
                    else
                        return null;


                // 9 TAG_List = [Byte] for tagID, [Int] for size, size # of payloads of type tagID. [contains unnamed tags]
                case TagType.TAG_List:
                // 10 TAG_Compound = Any number of fully formed named binary tags, terminates with a TAG_End
                case TagType.TAG_Compound:
                    {
                        if (currentTag.Name == tagName)
                            return currentTag;

                        List<NamedBinaryTag> children = (List<NamedBinaryTag>)currentTag.Payload;

                        NamedBinaryTag result = null;

                        foreach (NamedBinaryTag nbt in children)
                        {
                            result = FindTag(tagName, nbt);
                            if (result != null) return result;
                        }

                        return null;
                    }

                default:
                    throw new IndexOutOfRangeException("A non-existent tag was found");
            }
        }
    }
}
