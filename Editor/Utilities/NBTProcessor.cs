using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AweEditor.Datatypes;

namespace AweEditor.Utilities
{
    public class NBTProcessor
    {
        //Dimensions of the block array for the imported file
        private short width, length, height;

        //Store block types in a flattened 3D array, block coords are [Y][Z][X] - need to unflatten or simulate 3D array indicies based on width/length/height to find XYZ
        private byte[] blockArray;

        //For reading uncompressed schematics
        private BinaryReader reader;

        /// <summary>
        /// Currently only supports UNCOMPRESSED schematic files (Just unzip the .schematic and import the contained file)
        /// </summary>
        /// <param name="fileName"></param>
        public NBTProcessor()
        {

        }

        /// <summary>
        /// Processes the given file for 
        /// </summary>
        public void process(byte[] byteArray)
        {
            reader = new BinaryReader(new MemoryStream(byteArray));
            processTag();
            reader.Close();
        }

        private byte[, ,] unflattenBlockArray()
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

        public List<BlockData> createTerrain()
        {
            if (blockArray == null)
                return new List<BlockData>();

            List<BlockData> blocks = new List<BlockData>(); //TODO: optimize

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
                    blockData.type = blockArray[i];

                    blocks.Add(blockData);
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

            return blocks;
        }
        
        #region Tag Processing Methods

        //optional named parameter indicates whether or not the tag is followed by the standard name bytes, use false when parsing tags with no names

        private void processTag()
        {
            byte _byte;

            try
            {
                _byte = reader.ReadByte();
            }
            catch (Exception)
            {
                return;
            }

            switch (_byte)
            {
                case 1:
                    processByte();
                    break;

                case 2:
                    processShort();
                    break;

                case 3:
                    processInt();
                    break;

                case 4:
                    processLong();
                    break;
                    
                case 5:
                    processFloat();
                    break;

                case 6:
                    processDouble();
                    break;

                case 7:
                    processByteArray();
                    break;

                case 8:
                    processString();
                    break;

                case 9:
                    processList();
                    break;

                case 10:
                    processCompound();
                    break;

                case 11:
                    throw new NotImplementedException();
                    break;
            }
        }

        private string processName()
        {
            UInt16 nameLength = BitConverter.ToUInt16(getBytes(2), 0);
            return readString(nameLength);
        }

        private float processFloat(bool named = true)
        {
            string name;
            if (named)
                name = processName();

            return BitConverter.ToSingle(getBytes(4), 0);
        }

        private void processCompound(bool named = true)
        {
            string name;
            if (named)
                name = processName();

            byte _byte = (byte)reader.PeekChar();
            while (_byte != 0) //process children
            {
                processTag();
                _byte = (byte)reader.PeekChar();
            }

            reader.ReadChar(); //get rid of compound end tag (00)
            processTag(); //continue where we left off
        }

        private void processString(bool named = true)
        {
            string name;
            if (named)
                name = processName();

            UInt16 len = BitConverter.ToUInt16(getBytes(2), 0);
            string s = readString(len);
        }

        private void processByteArray(bool named = true)
        {
            string name = "";
            if (named)
                name = processName();

            int BAPayloadCount = BitConverter.ToInt32(getBytes(4), 0);

            if (BAPayloadCount == 0)
                return;

            if (name.Equals("Blocks")) //when we find the block array, store the values
            {
                blockArray = new byte[BAPayloadCount];

                for (int i = 0; i < BAPayloadCount; i++)
                {
                    byte BAPayload = reader.ReadByte();
                    blockArray[i] = BAPayload;
                }
            }
            else //read through and do nothing if not blocks
                for (int i = 0; i < BAPayloadCount; i++)
                    reader.ReadByte();
        }

        private void processList(bool named = true)
        {
            string name;
            if (named)
                name = processName();

            byte listType = reader.ReadByte();
            int listPayloadCount = BitConverter.ToInt32(getBytes(4), 0);

            if (listPayloadCount == 0)
                return;

            for (int i = 0; i < listPayloadCount; i++)
            {
                switch (listType)
                {
                    case 5:
                        processFloat(false);
                        break;
                    case 6:
                        processDouble(false);
                        break;
                    case 10:
                        processCompound(false);
                        break;
                    default:
                        throw new NotImplementedException();
                    
                    //TODO: other cases
                }
            }

            //TODO
        }

        private double processDouble(bool named = true)
        {
            string name;
            if (named)
                name = processName();

            return BitConverter.ToDouble(getBytes(8), 0);
        }
        private long processLong(bool named = true)
        {
            string name;
            if (named)
                name = processName();

            return BitConverter.ToInt64(getBytes(8), 0);
        }

        private Int32 processInt(bool named = true)
        {
            string name = "";
            if (named)
                name = processName();

            int val = BitConverter.ToInt32(getBytes(4), 0);

            return val;
        }

        private short processShort(bool named = true)
        {
            string name = "";
            if (named)
                name = processName();

            short val = BitConverter.ToInt16(getBytes(2), 0);

            //store w/h/l when we come across them
            if (name.Equals("Height"))
                height = val;
            else if (name.Equals("Width"))
                width = val;
            else if (name.Equals("Length"))
                length = val;

            return val;
        }

        private byte processByte(bool named = true)
        {
            string name;
            if (named)
                name = processName();

            return reader.ReadByte();
        }

        /// <summary>
        /// Gets array of bytes converted to correct endianness
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private byte[] getBytes(int len)
        {
            byte[] bytes = new byte[len];

            if (BitConverter.IsLittleEndian) //reverse bytes
                for (int i = len - 1; i >= 0; i--)
                    bytes[i] = reader.ReadByte();
            else
                for (int i = 0; i < len; i++)
                    bytes[i] = reader.ReadByte();

            return bytes;
        }

        private string readString(int len)
        {
            string val = "";

            for (int i = 0; i < len; i++)
                val += reader.ReadChar();

            return val;
        }

        #endregion
    }
}
