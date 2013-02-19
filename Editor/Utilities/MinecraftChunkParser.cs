﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AweEditor
{
    class MinecraftChunkParser
    {
        MinecraftChunk mc;
        int xPos;
        int zPos;
        long LastUpdate;
        bool TerrainPopulated;
        byte[] Biomes = new byte[256];
        int[] HeightMap = new int[256];
        List<SubChunk> Sections = new List<SubChunk>();
        List<MinecraftChunk> Entities = new List<MinecraftChunk>();
        List<MinecraftChunk> TileEntities = new List<MinecraftChunk>();
        List<MinecraftChunk> TileTicks = new List<MinecraftChunk>();
        public MinecraftChunkParser(MinecraftChunk m)
        {
            mc = m;
        }
        public void Parse()
        {
            if (mc != null)
            {
                int tagID = 0;
                int nameLength = 0;
                string Name = "";
                int payload = 0;
                int xPosLoc = 0;
                int zPosLoc = 0;
                int sectionLoc = 0;
                byte[] buff = new byte[4096];
                MemoryStream ms = new MemoryStream(mc.decryptedChunck, false);
                for (int i = 0; i < mc.decryptedChunck.Length; i++)
                {
                    if (mc.decryptedChunck[i] == 120 && (i+1 <mc.decryptedChunck.Length && mc.decryptedChunck[i+1] == 80))
                    {
                        xPosLoc = i;
                    }
                    if (mc.decryptedChunck[i] == 122 && (i+1 <mc.decryptedChunck.Length && mc.decryptedChunck[i+1] == 80))
                    {
                        zPosLoc = i;
                    }
                    if (mc.decryptedChunck[i] == 83 && (i+1 <mc.decryptedChunck.Length && mc.decryptedChunck[i+1] == 101))
                    {
                        sectionLoc = i;
                    }
                }
                if (xPosLoc != 0)
                {
                    xPosLoc -= 3;
                    ms.Seek(xPosLoc, SeekOrigin.Begin);
                    ms.Read(buff, 0, 1);
                    tagID = buff[0];
                    ms.Read(buff, 0, 2);
                    nameLength = buff[1];
                    ms.Read(buff, 0, nameLength);
                    Name = System.Text.Encoding.UTF8.GetString(buff);
                    Name = Name.Substring(0, nameLength);
                    ms.Read(buff, 0, 4);
                    xPos = buff[0] << 24 | buff[1] << 16 | buff[2] << 8 | buff[3];
                }
                if (zPosLoc != 0)
                {
                    zPosLoc -= 3;
                    ms.Seek(xPosLoc, SeekOrigin.Begin);
                    ms.Read(buff, 0, 1);
                    tagID = buff[0];
                    ms.Read(buff, 0, 2);
                    nameLength = buff[1];
                    ms.Read(buff, 0, nameLength);
                    Name = System.Text.Encoding.UTF8.GetString(buff);
                    Name = Name.Substring(0, nameLength);
                    ms.Read(buff, 0, 4);
                    zPos = buff[0] << 24 | buff[1] << 16 | buff[2] << 8 | buff[3];
                }
                if (sectionLoc != 0)
                {
                    sectionLoc -= 3;
                    SubChunk sc = new SubChunk();
                    ms.Seek(sectionLoc, SeekOrigin.Begin);
                    ms.Read(buff, 0, 1);
                    tagID = buff[0];
                    ms.Read(buff, 0, 2);
                    nameLength = buff[1];
                    ms.Read(buff, 0, nameLength);
                    Name = System.Text.Encoding.UTF8.GetString(buff);
                    Name = Name.Substring(0, nameLength);
                    ms.Read(buff, 0, 5);
                    payload = buff[1] << 24 | buff[2] << 16 | buff[3] << 8 | buff[4];
                    int i = 0;
                    long j = ms.Position;
                    while (i < payload && j < ms.Length)
                    {
                        ms.Seek(j, SeekOrigin.Begin);
                        ms.Read(buff, 0, 1);
                        tagID = buff[0];
                        ms.Read(buff, 0, 2);
                        nameLength = buff[1];
                        ms.Read(buff, 0, nameLength);
                        Name = System.Text.Encoding.UTF8.GetString(buff);
                        Name = Name.Substring(0, nameLength);
                        if (Name == "Data")
                        {
                            ms.Read(buff, 0, 4);
                            ms.Read(buff, 0, 2048);
                            Array.Copy(buff, sc.Data, 2048);
                            j = ms.Position;
                            i++;
                        }
                        if (Name == "SkyLight")
                        {
                            ms.Read(buff, 0, 4);
                            ms.Read(buff, 0, 2048);
                            Array.Copy(buff, sc.SkyLight, 2048);
                            j = ms.Position;
                            i++;
                        }
                        if (Name == "Blocks")
                        {
                            ms.Read(buff, 0, 4);
                            ms.Read(buff, 0, 4096);
                            Array.Copy(buff, sc.Blocks, 4096);
                            j = ms.Position;
                            i++;
                        }
                        if(Name == "Y")
                        {
                            ms.Read(buff, 0, 4);
                            sc.Y = buff[0] << 24 | buff[1] << 16 | buff[2] << 8 | buff[3];
                            j = ms.Position;
                            i++;
                        }
                        if (Name == "BlockLight")
                        {
                            ms.Read(buff, 0, 4);
                            ms.Read(buff, 0, 2048);
                            Array.Copy(buff, sc.BlockLight, 2048);
                            j = ms.Position;
                            i++;
                        }
                        if (Name.Contains("\0"))
                        {
                            break;
                            i++;
                        }
                    }
                }
            }
        }
    }
}
