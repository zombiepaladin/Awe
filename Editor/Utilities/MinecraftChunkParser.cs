using System;
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
                int i = 3;
                int nameLength = 0;
                string Name = "";
                int payload = 0;
                while (i < mc.decryptedChunck.Length)
                {
                    byte[] buffer = new byte[1024];
                    MemoryStream ms = new MemoryStream(mc.decryptedChunck, false);
                    ms.Flush();
                    ms.Seek(i, SeekOrigin.Begin);
                    ms.Read(buffer, 0, 1);
                    tagID = buffer[0];
                    ms.Read(buffer, 0, 2);
                    nameLength = buffer[0] + buffer[1];
                    ms.Read(buffer, 0, nameLength);
                    Name = System.Text.Encoding.UTF8.GetString(buffer);
                    Name = Name.Substring(0, nameLength);

                    if (Name == "Level")
                    {
                        ms.Read(buffer, 0, 2);
                        payload = buffer[0] + buffer[1];
                        i += nameLength + 3;
                    }
                    if (Name == "Entities")
                    {
                        ms.Read(buffer, 0, 5);
                        payload = buffer[1] << 24 | buffer[2] << 16 | buffer[3] << 8 | buffer[4];
                        i += nameLength + payload + 5 + 3;

                    }
                    if (Name == "Biomes")
                    {
                        ms.Read(buffer, 0, 4);
                        payload = buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3];
                        ms.Read(buffer, 0, 256);
                        Array.Copy(buffer, Biomes, 256);
                        i += nameLength + 256 + 4 + 3;
                    }
                    if (Name == "LastUpdate")
                    {
                        ms.Read(buffer, 0, 8);
                        LastUpdate = buffer[0] << 56 | buffer[1] << 48 | buffer[2] << 40 | buffer[3] << 32 | buffer[4] << 24 | buffer[5] << 16 | buffer[6] << 8 | buffer[7];
                        i += nameLength + 3 + 8;
                    }
                    if (Name == "xPos")
                    {
                        ms.Read(buffer, 0, 4);
                        xPos = buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3];
                        i += nameLength + 4 + 3;
                    }
                    if (Name == "zPos")
                    {
                        ms.Read(buffer, 0, 4);
                        zPos = buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3];
                        i += nameLength + 4 + 3;
                    }
                    if (Name == "TileEntities")
                    {
                        ms.Read(buffer, 0, 5);
                        payload = buffer[1] << 24 | buffer[2] << 16 | buffer[3] << 8 | buffer[4];
                        if (payload != 0)
                        {
                            int j = 0;
                            int f = 0;
                            int tID = 0;
                            int tNameL = 0;
                            string tName = "";
                            int tP = 0;
                            i += nameLength + payload + 6;
                            while (f < payload)
                            {
                                byte[] tBuff = new byte[1024];
                                ms.Seek(i + j, SeekOrigin.Begin);
                                ms.Read(tBuff, 0, 1);
                                j++;
                                tID = tBuff[0];
                                ms.Read(tBuff, 0, 2);
                                j += 2;
                                tNameL = tBuff[1];
                                ms.Read(tBuff, 0, tNameL);
                                j += tNameL;
                                tName = System.Text.Encoding.UTF8.GetString(tBuff);
                                tName = tName.Substring(0, tNameL);
                                if(tName == "id"){
                                    ms.Read(tBuff, 0, 2);
                                    j += 2;
                                    tNameL = tBuff[1];
                                    ms.Read(tBuff, 0, tNameL);
                                    j += tNameL;
                                    tName = System.Text.Encoding.UTF8.GetString(tBuff);
                                    tName = tName.Substring(0, tNameL);
                                    if (tName == "MobSpawner")
                                    {

                                        ms.Read(tBuff, 0, 1);
                                        j++;
                                        tID = tBuff[0];
                                        ms.Read(tBuff, 0, 2);
                                        j += 2;
                                        tNameL = tBuff[0] << 8 | tBuff[1];
                                        ms.Read(tBuff, 0, tNameL);
                                        tName = System.Text.Encoding.UTF8.GetString(tBuff);
                                        tName = tName.Substring(0, tNameL);
                                        j += tNameL;
                                        if (tName == "MinSpawnDelay")
                                        {
                                            j += 2;
                                        }
                                        ms.Seek(i + j, SeekOrigin.Begin);
                                        ms.Read(tBuff, 0, 1);
                                        j++;
                                        tID = tBuff[0];
                                        ms.Read(tBuff, 0, 2);
                                        j += 2;
                                        tNameL = tBuff[0] << 8 | tBuff[1];
                                        ms.Read(tBuff, 0, tNameL);
                                        tName = System.Text.Encoding.UTF8.GetString(tBuff);
                                        tName = tName.Substring(0, tNameL);
                                        j += tNameL;
                                        if (tName == "RequiredPlayerRange")
                                        {
                                            j += 2;
                                        }
                                        ms.Seek(i + j, SeekOrigin.Begin);
                                        ms.Read(tBuff, 0, 1);
                                        j++;
                                        tID = tBuff[0];
                                        ms.Read(tBuff, 0, 2);
                                        j += 2;
                                        tNameL = tBuff[0] << 8 | tBuff[1];
                                        ms.Read(tBuff, 0, tNameL);
                                        tName = System.Text.Encoding.UTF8.GetString(tBuff);
                                        tName = tName.Substring(0, tNameL);
                                        j += tNameL;
                                        if (tName == "Delay")
                                        {
                                            j += 2;
                                        }
                                        ms.Seek(i + j, SeekOrigin.Begin);
                                        ms.Read(tBuff, 0, 1);
                                        j++;
                                        tID = tBuff[0];
                                        ms.Read(tBuff, 0, 2);
                                        j += 2;
                                        tNameL = tBuff[0] << 8 | tBuff[1];
                                        ms.Read(tBuff, 0, tNameL);
                                        tName = System.Text.Encoding.UTF8.GetString(tBuff);
                                        tName = tName.Substring(0, tNameL);
                                        j += tNameL;
                                        if (tName == "MaxNearbyEntities")
                                        {
                                            j += 2;
                                        }
                                        ms.Seek(i + j, SeekOrigin.Begin);
                                        ms.Read(tBuff, 0, 1);
                                        j++;
                                        tID = tBuff[0];
                                        ms.Read(tBuff, 0, 2);
                                        j += 2;
                                        tNameL = tBuff[0] << 8 | tBuff[1];
                                        ms.Read(tBuff, 0, tNameL);
                                        tName = System.Text.Encoding.UTF8.GetString(tBuff);
                                        tName = tName.Substring(0, tNameL);
                                        j += tNameL;
                                        if (tName == "MaxSpawnDelay")
                                        {
                                            j += 2;
                                        }
                                        ms.Seek(i + j, SeekOrigin.Begin);
                                        ms.Read(tBuff, 0, 1);
                                        j++;
                                        tID = tBuff[0];
                                        ms.Read(tBuff, 0, 2);
                                        j += 2;
                                        tNameL = tBuff[0] << 8 | tBuff[1];
                                        ms.Read(tBuff, 0, tNameL);
                                        tName = System.Text.Encoding.UTF8.GetString(tBuff);
                                        tName = tName.Substring(0, tNameL);
                                        j += tNameL;
                                        if (tName == "SpawnRange")
                                        {
                                            j += 2;
                                        }
                                        ms.Seek(i + j, SeekOrigin.Begin);
                                        ms.Read(tBuff, 0, 1);
                                        j++;
                                        tID = tBuff[0];
                                        ms.Read(tBuff, 0, 2);
                                        j += 2;
                                        tNameL = tBuff[0] << 8 | tBuff[1];
                                        ms.Read(tBuff, 0, tNameL);
                                        tName = System.Text.Encoding.UTF8.GetString(tBuff);
                                        tName = tName.Substring(0, tNameL);
                                        j += tNameL;
                                        if (tName == "SpawnCount")
                                        {
                                            j += 2;
                                        }
                                        ms.Seek(i + j, SeekOrigin.Begin);
                                        ms.Read(tBuff, 0, 1);
                                        j++;
                                        tID = tBuff[0];
                                        ms.Read(tBuff, 0, 2);
                                        j += 2;
                                        tNameL = tBuff[0] << 8 | tBuff[1];
                                        ms.Read(tBuff, 0, tNameL);
                                        tName = System.Text.Encoding.UTF8.GetString(tBuff);
                                        tName = tName.Substring(0, tNameL);
                                        j += tNameL;
                                        if (tName == "z")
                                        {
                                            j += 4;
                                        }
                                        ms.Seek(i + j, SeekOrigin.Begin);
                                        ms.Read(tBuff, 0, 1);
                                        j++;
                                        tID = tBuff[0];
                                        ms.Read(tBuff, 0, 2);
                                        j += 2;
                                        tNameL = tBuff[0] << 8 | tBuff[1];
                                        ms.Read(tBuff, 0, tNameL);
                                        tName = System.Text.Encoding.UTF8.GetString(tBuff);
                                        tName = tName.Substring(0, tNameL);
                                        j += tNameL;
                                        if (tName == "EntityId")
                                        {
                                            j = EntityLoop(j, ms);
                                            j++;
                                        }
                                        f++;
                                    }

                                }
                                if (tName == "Items")
                                {
                                    j += 5;
                                    ms.Read(tBuff, 0, 5);
                                    //ID part
                                    ms.Read(tBuff, 0, 1);
                                    tID = tBuff[0];
                                    j++;
                                    ms.Read(tBuff, 0, 2);
                                    j += 2;
                                    tNameL = tBuff[1];
                                    ms.Read(tBuff, 0, tNameL);
                                    tName = System.Text.Encoding.UTF8.GetString(tBuff);
                                    tName = tName.Substring(0, tNameL);
                                    j += tNameL;
                                    j += 2;
                                    ms.Read(tBuff, 0, 2);

                                    ms.Read(tBuff, 0, 1);
                                    tID = tBuff[0];
                                    j++;
                                    ms.Read(tBuff, 0, 2);
                                    j += 2;
                                    tNameL = tBuff[1];
                                    ms.Read(tBuff, 0, tNameL);
                                    tName = System.Text.Encoding.UTF8.GetString(tBuff);
                                    tName = tName.Substring(0, tNameL);
                                    j += tNameL;

                                }
                                
                            }
                            i += j;
                        }
                        else
                        {
                            i += nameLength + payload + 8;
                        }
                    }
                    if (Name == "TerrainPopulated")
                    {
                        ms.Read(buffer, 0, 1);
                        TerrainPopulated = Convert.ToBoolean(buffer[0]);
                        i += nameLength + 3 + 1;
                    }
                    if (Name == "HeightMap")
                    {
                        ms.Read(buffer, 0, 4);
                        payload = Convert.ToInt32(buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3]);
                        for (int j = 0; j < 256; j++)
                        {
                            ms.Read(buffer, 0, 4);
                            HeightMap[j] = buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3];
                        }
                        i += nameLength + 3 + payload * 4 + 4;
                    }
                    if (Name == "Sections")
                    {
                        SubChunk sc = new SubChunk();
                        ms.Read(buffer, 0, 5);
                        payload = buffer[1] << 24 | buffer[2] << 16 | buffer[3] << 8 | buffer[4];
                        int j = 0;

                        int scID = 0;
                        int scPayload = 0;
                        int scNameLength = 0;
                        String scName = "";
                        i += nameLength + 3 + 5;
                        while (j < payload)
                        {
                            byte[] scBuffer = new byte[4096];
                            ms.Seek(i + j, SeekOrigin.Begin);
                            ms.Read(scBuffer, 0, 1);
                            scID = scBuffer[0];
                            ms.Read(scBuffer, 0, 2);
                            scNameLength = scBuffer[1];
                            ms.Read(scBuffer, 0, scNameLength);
                            scName = System.Text.Encoding.UTF8.GetString(scBuffer);
                            scName = scName.Substring(0, scNameLength);
                            if (scName == "Data")
                            {
                                ms.Read(scBuffer, 0, 4);
                                scPayload = scBuffer[0] << 24 | scBuffer[1] << 16 | scBuffer[2] << 8 | scBuffer[0];
                                ms.Read(scBuffer, 0, 2048);
                                Array.Copy(scBuffer, sc.Data, sc.Data.Length);
                                j += scPayload + 3 + 4;
                            }
                            if (scName == "Blocks")
                            {
                                i += 0;
                            }
                            if (scName == "Y")
                            {
                                i += 0;
                            }
                            if (scName == "BlockLight")
                            {
                                i += 0;
                            }
                            if (scName == "SkyLight")
                            {
                                i += 0;
                            }
                        }
                        Sections.Add(sc);
                        i += j;
                    }
                    if (Name == "TileTicks")
                    {
                        i += 0;
                    }

                    ms.Close();
                }
            }
        }
        int EntityLoop(int i, MemoryStream ms)
        {
            byte[] tBuff = new byte[1000];
            int tNameL = 0;
            string tName = "";
            int tID = 0;
            while (true)
            {
                //Namepart
                ms.Read(tBuff, 0, 2);
                i += 2;
                tNameL = tBuff[0] << 8 | tBuff[1];
                ms.Read(tBuff, 0, tNameL);
                tName = System.Text.Encoding.UTF8.GetString(tBuff);
                tName = tName.Substring(0, tNameL);
                i += tNameL;
                //y part
                ms.Read(tBuff, 0, 1);
                i++;
                tID = tBuff[0];
                ms.Read(tBuff, 0, 2);
                i += 2;
                tNameL = tBuff[0] << 8 | tBuff[1];
                ms.Read(tBuff, 0, tNameL);
                tName = System.Text.Encoding.UTF8.GetString(tBuff);
                tName = tName.Substring(0, tNameL);
                i += tNameL;
                i += 4;
                //x part
                ms.Read(tBuff, 0, 4);
                ms.Read(tBuff, 0, 1);
                i++;
                tID = tBuff[0];
                ms.Read(tBuff, 0, 2);
                i += 2;
                tNameL = tBuff[0] << 8 | tBuff[1];
                ms.Read(tBuff, 0, tNameL);
                tName = System.Text.Encoding.UTF8.GetString(tBuff);
                tName = tName.Substring(0, tNameL);
                i += tNameL;
                i += 4;
                //Items part
                break;

            }
            return i;
        }
    }
}
