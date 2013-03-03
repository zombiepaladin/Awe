using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AweEditor.Utilities.MarchingCubes
{
	class MarchingCubesGenerator
	{
		private static int WORLD_WIDTH;
		private static int WORLD_LENGTH;
		private static int WORLD_HEIGHT;

		public MarchingCubesGenerator()
			: this(512, 256, 512)
		{
		}

		public MarchingCubesGenerator(int x, int y, int z)
		{
			WORLD_WIDTH = x;
			WORLD_HEIGHT = y;
			WORLD_LENGTH = z;
		}

		public void March(List<BlockData> blockList, bool simple)
		{
			byte[,] slice1 = GetSlice(blockList, 0, simple);
			byte[,] slice2 = GetSlice(blockList, 1, simple);
			byte[,] slice3 = GetSlice(blockList, 2, simple);
			byte[,] slice4 = GetSlice(blockList, 3, simple);
			byte[,] index;
			Tuple<byte, byte>[,] tIndex;

			for (int z = 0; z < WORLD_LENGTH-1; z++)
			{
				if (simple)
				{
					index = ProcessSlices(slice1, slice2);
					AddToMesh(index, z);
				}
				else
				{
				}
				
				slice1 = slice2;
				slice2 = slice3;
				slice3 = slice4;
				slice4 = GetSlice(blockList, z + 4, simple);
			}
		}

		private byte[,] GetSlice(List<BlockData> blockList, int z, bool simple)
		{
			if (z > WORLD_LENGTH)
				return null;

			byte[,] slice = new byte[WORLD_WIDTH, WORLD_HEIGHT];
			List<BlockData> list = blockList.FindAll(x => x.Z == z);
			
			if (simple)
			{
				list.ForEach(block => slice[block.X, block.Y] = 1);
			}
			else
			{
				list.ForEach(block => slice[block.X, block.Y] = block.Type);
			}

			return slice;
		}

		private byte[,] ProcessSlices(byte[,] slice1, byte[,] slice2)
		{
			byte[,] index = new byte[WORLD_WIDTH - 1, WORLD_HEIGHT - 1];
			byte v1,v2,v3,v4,v5,v6,v7,v8;

			for (int y = 0; y < WORLD_HEIGHT - 1; y++)
			{
				for (int x = 0; x < WORLD_WIDTH - 1; x++)
				{
					if (slice1 == null)
					{
						v1 = 0;
						v2 = 0;
						v3 = 0;
						v4 = 0;
						v5 = slice2[x, y];
						v6 = slice2[x + 1, y];
						v7 = slice2[x + 1, y + 1];
						v8 = slice2[x, y + 1];
					}
					else if (slice2 ==null)
					{
						v1 = slice1[x, y];
						v2 = slice1[x + 1, y];
						v3 = slice1[x + 1, y + 1];
						v4 = slice1[x, y + 1];
						v5 = 0;
						v6 = 0;
						v7 = 0;
						v8 = 0;
					}
					else
					{
						v1 = slice1[x, y];
						v2 = slice1[x + 1, y];
						v3 = slice1[x + 1, y + 1];
						v4 = slice1[x, y + 1];
						v5 = slice2[x, y];
						v6 = slice2[x + 1, y];
						v7 = slice2[x + 1, y + 1];
						v8 = slice2[x, y + 1];
					}
					index[x, y] = (byte)((v1) | (v2 << 1) | (v3 << 2) | (v4 << 3) | (v5 << 4) | (v6 << 5) | (v7 << 6) | (v8 << 7));
				}
			}
			return index;
		}

		private void AddToMesh(byte[,] meshIndicies,int z)
		{
			byte[,] indicies = meshIndicies;
			
			throw new NotImplementedException();
		}

		private Tuple<byte,byte[]>[,] ProcessTexturedSlices(byte[,] slice1, byte[,] slice2)
		{
			byte index;
			Tuple<byte, byte[]>[,] cubes = new Tuple<byte,byte[]>[WORLD_WIDTH - 1, WORLD_HEIGHT - 1] 
			byte v1, v2, v3, v4, v5, v6, v7, v8;
			byte[] type;
			

			for (int y = 0; y < WORLD_HEIGHT - 1; y++)
			{
				for (int x = 0; x < WORLD_WIDTH - 1; x++)
				{
					type = new byte[8];
					if (slice1 == null)
					{			
						type[0] = 0;
						v1 = 0;
					
						type[1] = 0;
						v2 = 0;

						type[2] = 0;
						v3 = 0;

						type[3] = 0;
						v4 = 0;

						type[4] = slice2[x, y];
						v5 = (type[4] != 0) ? (byte)1 : (byte)0;

						type[5] = slice2[x + 1, y];
						v6 = (type[5] != 0) ? (byte)1 : (byte)0;

						type[6] = slice2[x + 1, y + 1];
						v7 = (type[6] != 0) ? (byte)1 : (byte)0;

						type[7] = slice2[x, y + 1];
						v8 = (type[7] != 0) ? (byte)1 : (byte)0;
					}
					else if (slice2 == null)
					{
						type[0] = slice1[x, y];
						v1 =  (type[0] != 0) ? (byte)1 : (byte)0;

						type[1] = slice1[x+1,y];
						v2 = (type[1] != 0) ? (byte)1 : (byte)0;

						type[2] = slice1[x+1,y+1];
						v3 =  (type[2] != 0) ? (byte)1 : (byte)0;

						type[3] =  slice1[x, y + 1];
						v4 = (type[3] != 0) ? (byte)1 : (byte)0;

						type[4]=0;
						v5 = 0;

						type[5]=0;
						v6 = 0;

						type[6]=0;
						v7 = 0;

						type[7]=0;
						v8 = 0;
					}
					else
					{
						v1 = (byte)(0x1 & slice1[x, y]);
						v2 = (byte)(0x1 & slice1[x + 1, y]);
						v3 = (byte)(0x1 & slice1[x + 1, y + 1]);
						v4 = (byte)(0x1 & slice1[x, y + 1]);
						v5 = (byte)(0x1 & slice2[x, y]);
						v6 = (byte)(0x1 & slice2[x + 1, y]);
						v7 = (byte)(0x1 & slice2[x + 1, y + 1]);
						v8 = (byte)(0x1 & slice2[x, y + 1]);
					}
					index = (byte)((v1) | (v2 << 1) | (v3 << 2) | (v4 << 3) | (v5 << 4) | (v6 << 5) | (v7 << 6) | (v8 << 7));
				}
			}
			

			throw new NotImplementedException();
		}

		private void AddToTexturedMesh(Tuple<byte, byte>[,] tIndex, int z)
		{

			throw new NotImplementedException();
		}
	}

	class BlockData
	{
		private int _x, _y, _z;
		private byte _type;

		public int X
		{
			get
			{
				return _x;
			}
		}

		public int Y
		{
			get
			{
				return _y;
			}
		}

		public int Z
		{
			get
			{
				return _z;
			}
		}

		public byte Type
		{
			get
			{
				return _type;
			}
		}
	}
}
