using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AweEditor.Datatypes;
using System.IO.Compression;

namespace AweEditor.Utilities
{

    public class SchematicProcessor
    {
        //Dimensions
        private short width, length, height;

        /// <summary>
        /// Height in blocks
        /// </summary>
        public short Height
        {
            get { return height; }
        }

        /// <summary>
        /// Length in blocks
        /// </summary>
        public short Length
        {
            get { return length; }
        }

        /// <summary>
        /// Width in blocks
        /// </summary>
        public short Width
        {
            get { return width; }
        }

        //Store block types in a flattened 3D array, block coords are [Y][Z][X] - need to unflatten or simulate 3D array indicies based on width/length/height to find XYZ
        private byte[] blockArray;

        /// <summary>
        /// The raw byte data for the block types.
        /// </summary>
        public byte[] BlockArray
        {
            get { return blockArray; }
        }

        //Used to read byte data
        private BinaryReader reader;

        /// <summary>
        /// Processes the given schematic file.
        /// </summary>
        /// <param name="fileName"></param>
        public SchematicProcessor(string fileName) : this(File.ReadAllBytes(fileName)){
            //converts file to byte array and passes it to the byte array constructor
        }

        /// <summary>
        /// Processes byte array containing the data of a schematic file.
        /// </summary>
        /// <param name="byteArray"></param>
        public SchematicProcessor(byte[] byteArray)
        {
            byteArray = decompressIfNeeded(byteArray);

            reader = new BinaryReader(new MemoryStream(byteArray));

            processTag();
        }

        private byte[] decompressIfNeeded(byte[] byteArray)
        {
            if (byteArray.Length < 3)
                throw new Exception("Invalid byte array sent to decompressIfNeeded");

            //GZip starts with 1F 8B 08
            if (byteArray[0] == 0x1F && byteArray[1] == 0x8B && byteArray[2] == 0x08) //does endianness matter here?
            {
                // Create a GZIP stream with decompression mode.
                // ... Then create a buffer and write into while reading from the GZIP stream.
                using (GZipStream stream = new GZipStream(new MemoryStream(byteArray), CompressionMode.Decompress))
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
            else
                return byteArray;
        }

        /// <summary>
        /// Creates a List of BlockData structs from the schematic.
        /// Set parameter to false to keep air blocks.
        /// </summary>
        /// <param name="ignoreAir"></param>
        /// <returns></returns>
        public List<BlockData> generateBlockData(bool ignoreAir = true)
        {
            if (blockArray == null)
            {
                //TODO:show error reading file
                return new List<BlockData>();
            }

            List<BlockData> blocks = new List<BlockData>();

            //variables used in loop
            short y, z, x;
            x = y = z = 0;
            BlockData blockData = new BlockData();
            for (int i = 0; i < blockArray.Length; i++)
            {
                if (!ignoreAir || blockArray[i] != 0)
                {
                    blockData.x = x;
                    blockData.y = y;
                    blockData.z = z;
                    blockData.type = blockArray[i];

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

            return blocks;
        }
        
        #region Tag Processing Methods

        private void processTag()
        {
            byte _byte;

            try
            {
                _byte = reader.ReadByte();
            }
            catch (EndOfStreamException)
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
                    processIntArray();
                    break;
            }
        }

        private string processName()
        {
            UInt16 nameLength = BitConverter.ToUInt16(getBytes(2), 0);
            return readString(nameLength);
        }

        //optional named parameter indicates whether or not the tag is followed by the standard name bytes, use false when parsing tags with no names
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

        private void processIntArray(bool named = true)
        {
            string name = "";
            if (named)
                name = processName();

            int BAPayloadCount = processInt(false);

            if (BAPayloadCount == 0)
                return;

            for (int i = 0; i < BAPayloadCount; i++)
                processInt(false);
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

            /* Not sure if endianness needs to be checked
            if (BitConverter.IsLittleEndian) //reverse bytes
                for (int i = len - 1; i >= 0; i--)
                    bytes[i] = reader.ReadByte();
            else
                for (int i = 0; i < len; i++)
                    bytes[i] = reader.ReadByte();

             */

            for (int i = len - 1; i >= 0; i--)
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
