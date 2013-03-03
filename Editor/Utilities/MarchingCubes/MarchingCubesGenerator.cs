using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework;


namespace AweEditor.Utilities.MarchingCubes
{
	class MarchingCubesGenerator
	{
		private int WORLD_WIDTH;
		private int WORLD_LENGTH;
		private int WORLD_HEIGHT;

        public string MeshName { get; set; }

		public MarchingCubesGenerator()
			: this(512, 256, 512, "Default")
		{
		}

		public MarchingCubesGenerator(int x, int y, int z, string meshName)
		{
			WORLD_WIDTH = x;
			WORLD_HEIGHT = y;
			WORLD_LENGTH = z;
            MeshName = meshName;
		}

		public MeshContent March(List<BlockData> blockList, bool simple)
		{
            MeshBuilder mBuilder = MeshBuilder.StartMesh(MeshName);
			byte[,] slice1 = GetSlice(blockList, 0, simple);
			byte[,] slice2 = GetSlice(blockList, 1, simple);
			byte[,] slice3 = GetSlice(blockList, 2, simple);
			byte[,] slice4 = GetSlice(blockList, 3, simple);
			byte[,] index;
			Tuple<byte, byte[]>[,] tIndex;

			for (int z = 0; z < WORLD_LENGTH-1; z++)
			{
				if (simple)
				{
					index = ProcessSlices(slice1, slice2);
                    for (int x = 0; x < WORLD_WIDTH - 1; x++)
                    {
                        for (int y = 0; y < WORLD_HEIGHT - 1; y++)
                        {
                            ProcessIndex(mBuilder, index[x, y], x, y, z);
                        }
                    }
				}
				else
				{
					tIndex = ProcessTexturedSlices(slice1, slice2);
					AddToTexturedMesh(tIndex, z);
				}
				
				slice1 = slice2;
				slice2 = slice3;
				slice3 = slice4;
				slice4 = GetSlice(blockList, z + 4, simple);
			}

            return mBuilder.FinishMesh();
		}

		private byte[,] GetSlice(List<BlockData> blockList, int z, bool simple)
		{
			if (z > WORLD_LENGTH)
				return null;

			byte[,] slice = new byte[WORLD_WIDTH, WORLD_HEIGHT];
			List<BlockData> list = blockList.FindAll(x => x.z == z);
			
			if (simple)
			{
				list.ForEach(block => slice[block.x, block.y] = 1);
			}
			else
			{
				list.ForEach(block => slice[block.x, block.y] = block.Type);
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

		private void AddToMesh(MeshBuilder mBuilder, byte[,] meshIndicies,int z)
		{
			byte[,] indicies = meshIndicies;

			for (int x = 0; x < WORLD_WIDTH - 1; x++)
			{
				for (int y = 0; y < WORLD_HEIGHT - 1; y++)
				{
					ProcessIndex(mBuilder, indicies[x, y], x, y, z);
				}
			}
		}

        /// <summary>
        /// Adds triangle verticies to the MeshBuilder based on the index given.
        /// </summary>
        /// <param name="mBuilder"></param>
        /// <param name="index"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
		private void ProcessIndex(MeshBuilder mBuilder, byte index, int x, int y, int z)
		{
			switch (index)
			{
                //v8v7v6v5 v4v3v2v1
				#region 0-99
				#region 0-9
				case (0):
					break;
				case(1):
					break;
				case(2):
					break;
				case(3):
					break;
				case(4):
					break;
				case(5):
					break;
				case(6):
					break;
				case(7):
					break;
				case(8):
					break;
				case(9):
					break;
				#endregion;
				#region 10-19
				case (10):
					break;
				case (11):
					break;
				case (12):
					break;
				case (13):
					break;
				case (14):
					break;
				case (15):
					break;
				case (16):
					break;
				case (17):
					break;
				case (18):
					break;
				case (19):
					break;
				#endregion;
				#region 20-29
				case (20):
					break;
				case (21):
					break;
				case (22):
					break;
				case (23):
					break;
				case (24):
					break;
				case (25):
					break;
				case (26):
					break;
				case (27):
					break;
				case (28):
					break;
				case (29):
					break;
				#endregion;
				#region 30-39
				case (30):
					break;
				case (31):
					break;
				case (32):
					break;
				case (33):
					break;
				case (34):
					break;
				case (35):
					break;
				case (36):
					break;
				case (37):
					break;
				case (38):
					break;
				case (39):
					break;
				#endregion;
				#region 40-49
				case (40):
					break;
				case (41):
					break;
				case (42):
					break;
				case (43):
					break;
				case (44):
					break;
				case (45):
					break;
				case (46):
					break;
				case (47):
					break;
				case (48):
					break;
				case (49):
					break;
				#endregion;
				#region 50-59
				case (50):
					break;
				case (51):
					break;
				case (52):
					break;
				case (53):
					break;
				case (54):
					break;
				case (55):
					break;
				case (56):
					break;
				case (57):
					break;
				case (58):
					break;
				case (59):
					break;
				#endregion;
				#region 60-69
				case (60):
					break;
				case (61):
					break;
				case (62):
					break;
				case (63):
					break;
				case (64):
					break;
				case (65):
					break;
				case (66):
					break;
				case (67):
					break;
				case (68):
					break;
				case (69):
					break;
				#endregion;		
				#region 70-79
				case (70):
					break;
				case (71):
					break;
				case (72):
					break;
				case (73):
					break;
				case (74):
					break;
				case (75):
					break;
				case (76):
					break;
				case (77):
					break;
				case (78):
					break;
				case (79):
					break;
				#endregion;
				#region 80-89
				case (80):
					break;
				case (81):
					break;
				case (82):
					break;
				case (83):
					break;
				case (84):
					break;
				case (85):
					break;
				case (86):
					break;
				case (87):
					break;
				case (88):
					break;
				case (89):
					break;
				#endregion;
				#region 90-99
				case (90):
					break;
				case (91):
					break;
				case (92):
					break;
				case (93):
					break;
				case (94):
					break;
				case (95):
					break;
				case (96):
					break;
				case (97):
					break;
				case (98):
					break;
				case (99):
					break;
				#endregion;
				#endregion
				#region 100-199
				#region 100-109
				case (100):
					break;
				case (101):
					break;
				case (102):
					break;
				case (103):
					break;
				case (104):
					break;
				case (105):
					break;
				case (106):
					break;
				case (107):
					break;
				case (108):
					break;
				case (109):
					break;
				#endregion;
				#region 110-119
				case (110):
					break;
				case (111):
					break;
				case (112):
					break;
				case (113):
					break;
				case (114):
					break;
				case (115):
					break;
				case (116):
					break;
				case (117):
					break;
				case (118):
					break;
				case (119):
					break;
				#endregion;
				#region 120-129
				case (120):
					break;
				case (121):
					break;
				case (122):
					break;
				case (123):
					break;
				case (124):
					break;
				case (125):
					break;
				case (126):
					break;
				case (127):
					break;
				case (128):
					break;
				case (129):
					break;
				#endregion;
				#region 130-139
				case (130):
					break;
				case (131):
					break;
				case (132):
					break;
				case (133):
					break;
				case (134):
					break;
				case (135):
					break;
				case (136):
					break;
				case (137):
					break;
				case (138):
					break;
				case (139):
					break;
				#endregion;
				#region 140-149
				case (140):
					break;
				case (141):
					break;
				case (142):
					break;
				case (143):
					break;
				case (144):
					break;
				case (145):
					break;
				case (146):
					break;
				case (147):
					break;
				case (148):
					break;
				case (149):
					break;
				#endregion;
				#region 150-159
				case (150):
					break;
				case (151):
					break;
				case (152):
					break;
				case (153):
					break;
				case (154):
					break;
				case (155):
					break;
				case (156):
					break;
				case (157):
					break;
				case (158):
					break;
				case (159):
					break;
				#endregion;
				#region 160-169
				case (160):
					break;
				case (161):
					break;
				case (162):
					break;
				case (163):
					break;
				case (164):
					break;
				case (165):
					break;
				case (166):
					break;
				case (167):
					break;
				case (168):
					break;
				case (169):
					break;
				#endregion;
				#region 170-179
				case (170):
					break;
				case (171):
					break;
				case (172):
					break;
				case (173):
					break;
				case (174):
					break;
				case (175):
					break;
				case (176):
					break;
				case (177):
					break;
				case (178):
					break;
				case (179):
					break;
				#endregion;
				#region 180-189
				case (180):
					break;
				case (181):
					break;
				case (182):
					break;
				case (183):
					break;
				case (184):
					break;
				case (185):
					break;
				case (186):
					break;
				case (187):
					break;
				case (188):
					break;
				case (189):
					break;
				#endregion;
				#region 190-199
				case (190):
					break;
				case (191):
					break;
				case (192):
					break;
				case (193):
					break;
				case (194):
					break;
				case (195):
					break;
				case (196):
					break;
				case (197):
					break;
				case (198):
					break;
				case (199):
					break;
				#endregion;
				#endregion
				#region 200-255
				#region 200-209
				case (200):
					break;
				case (201):
					break;
				case (202):
					break;
				case (203):
					break;
				case (204):
					break;
				case (205):
					break;
				case (206):
					break;
				case (207):
					break;
				case (208):
					break;
				case (209):
					break;
				#endregion;
				#region 210-219
				case (210):
					break;
				case (211):
					break;
				case (212):
					break;
				case (213):
					break;
				case (214):
					break;
				case (215):
					break;
				case (216):
					break;
				case (217):
					break;
				case (218):
					break;
				case (219):
					break;
				#endregion;
				#region 220-229
				case (220):
					break;
				case (221):
					break;
				case (222):
					break;
				case (223):
					break;
				case (224):
					break;
				case (225):
					break;
				case (226):
					break;
				case (227):
					break;
				case (228):
					break;
				case (229):
					break;
				#endregion;
				#region 230-239
				case (230):
					break;
				case (231):
					break;
				case (232):
					break;
				case (233):
					break;
				case (234):
					break;
				case (235):
					break;
				case (236):
					break;
				case (237):
					break;
				case (238):
					break;
				case (239):
					break;
				#endregion;
				#region 240-249
				case (240):
					break;
				case (241):
					break;
				case (242):
					break;
				case (243):
					break;
				case (244):
					break;
				case (245):
					break;
				case (246):
					break;
				case (247):
					break;
				case (248):
					break;
				case (249):
					break;
				#endregion;
				#region 250-255
				case (250): //1111 1010
					break;
				case (251): //1111 1011
					break;
				case (252): //1111 1100
					break;
				case (253): //1111 1101
					break;
				case (254): //1111 1110
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(GetVertex(1, x, y, z)));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(GetVertex(5, x, y, z)));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(GetVertex(4, x, y, z)));
                    break;
				case (255): //1111 1111
                    //Do nothing.
					break;
				#endregion
				#endregion
				default:
					break;
			}
		}

        private Vector3 GetVertex(int vertex, int x, int y, int z)
        {
            switch (vertex)
            {
                case 1:
                    return new Vector3(2 * x + 1, 2 * y, 2 * z);
                case 2:
                    return new Vector3(2 * x + 2, 2 * y + 1, 2 * z);
                case 3:
                    return new Vector3(2 * x + 1, 2 * y + 2, 2 * z);
                case 4:
                    return new Vector3(2 * x, 2 * y + 1, 2 * z);
                case 5:
                    return new Vector3(2 * x, 2 * y, 2 * z + 1);
                case 6:
                    return new Vector3(2 * x + 2, 2 * y, 2 * z + 1);
                case 7:
                    return new Vector3(2 * x + 2, 2 * y + 2, 2 * z + 1);
                case 8:
                    return new Vector3(2 * x, 2 * y + 2, 2 * z + 1);
                case 9:
                    return new Vector3(2 * x + 1, 2 * y, 2 * z + 2);
                case 10:
                    return new Vector3(2 * x + 2, 2 * y + 1, 2 * z + 2);
                case 11:
                    return new Vector3(2 * x + 1, 2 * y + 2, 2 * z + 2);
                case 12:
                    return new Vector3(2 * x, 2 * y + 2, 2 * z + 2);
                default:
                    throw new Exception("Unknown vertex");
            }
        }

		private Tuple<byte,byte[]>[,] ProcessTexturedSlices(byte[,] slice1, byte[,] slice2)
		{
			byte index;
			Tuple<byte, byte[]>[,] cubes = new Tuple<byte, byte[]>[WORLD_WIDTH - 1, WORLD_HEIGHT - 1];
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
						type[0] = slice1[x, y];
						v1 = (type[0] != 0) ? (byte)1 : (byte)0;

						type[1] = slice1[x + 1, y];
						v2 = (type[1] != 0) ? (byte)1 : (byte)0;

						type[2] = slice1[x + 1, y + 1];
						v3 = (type[2] != 0) ? (byte)1 : (byte)0;

						type[3] = slice1[x, y + 1];
						v4 = (type[3] != 0) ? (byte)1 : (byte)0;

						type[4] = slice2[x, y];
						v5 = (type[4] != 0) ? (byte)1 : (byte)0;

						type[5] = slice2[x + 1, y];
						v6 = (type[5] != 0) ? (byte)1 : (byte)0;

						type[6] = slice2[x + 1, y + 1];
						v7 = (type[6] != 0) ? (byte)1 : (byte)0;

						type[7] = slice2[x, y + 1];
						v8 = (type[7] != 0) ? (byte)1 : (byte)0;
					}
					index = (byte)((v1) | (v2 << 1) | (v3 << 2) | (v4 << 3) | (v5 << 4) | (v6 << 5) | (v7 << 6) | (v8 << 7));
					cubes[x, y] = new Tuple<byte, byte[]>(index, type);
				}
			}

			return cubes;
		}

		private void AddToTexturedMesh(Tuple<byte, byte[]>[,] tIndex, int z)
		{

			throw new NotImplementedException();
		}
	}

	class BlockData
	{
		private int _x, _y, _z;
		private byte _type;

		public int x
		{
			get
			{
				return _x;
			}
		}

		public int y
		{
			get
			{
				return _y;
			}
		}

		public int z
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
