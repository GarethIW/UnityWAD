using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityWAD
{
    public static class WADReader
    {
        // Deals with reading data from the WAD file
        // See https://github.com/nukeop/TheUnofficialDoomSpecs

        public static WADInfo GetInfo(FileStream wadStream)
        {
            // Root WAD data
            var wi = new WADInfo()
            {
                Type = ReadString(wadStream, 0, 4),
                NumEntries = ReadInt(wadStream, 4, 4),
                DirectoryOffset = ReadInt(wadStream, 8, 4)
            };

            // Get a list of the contents of the WAD
            for(var i=0;i<wi.NumEntries;i++)
            {
                var entryOffset = wi.DirectoryOffset + (i * 16);
                var entry = new WADEntry()
                {
                    ResourceOffset = ReadInt(wadStream, entryOffset, 4),
                    ResourceLength = ReadInt(wadStream, entryOffset + 4, 4),
                    Name = ReadString(wadStream, entryOffset + 8, 8)
                };
                wi.Entries.Add(entry);
                if (!wi.EntryDictionary.ContainsKey(entry.Name.Replace("\0","")))
                {
                    wi.EntryDictionary.Add(entry.Name.Replace("\0",""), entry);
                    //Debug.Log(entry.Name);
                }

            }

            // Wall patches
            // Get patchnames lookup
            var patchesEntry = wi.EntryDictionary["PNAMES"];
            var patchesData = GetResource(wadStream, patchesEntry);
            var numPatches = BitConverter.ToInt32(patchesData, 0);
            wi.PatchNames = new string[numPatches];
            var pos = 4;
            for (var i = 0; i < numPatches; i++)
            {
                wi.PatchNames[i] = Encoding.UTF8.GetString(patchesData, pos, 8).ToUpper().Replace("\0", "");
                pos += 8;
            }

            // Wall textures
            var texEntry = wi.EntryDictionary["TEXTURE1"];
            var texList = GetTextures(wadStream, wi, GetResource(wadStream, texEntry));
            foreach(var t in texList)
            {
                wi.WallTextures.Add(t.Name,t);
            }
            if (wi.EntryDictionary.ContainsKey("TEXTURE2"))
            {
                texEntry = wi.EntryDictionary["TEXTURE2"];
                texList = GetTextures(wadStream, wi, GetResource(wadStream, texEntry));
                foreach (var t in texList)
                    wi.WallTextures.Add(t.Name,t);

            }

            return wi;
        }

        public static List<WallTextureData> GetTextures(FileStream wadStream, WADInfo wadInfo, byte[] data)
        {
            var textures = new List<WallTextureData>();
            var numTexs = BitConverter.ToInt32(data, 0);
            for (var i = 0; i < numTexs; i++)
            {
                var pointer = BitConverter.ToInt32(data, 4 + (i * 4));

                var name = Encoding.UTF8.GetString(data, pointer, 8).ToUpper();
                var w = BitConverter.ToInt16(data, pointer+12);
                var h = BitConverter.ToInt16(data, pointer+14);
                var numPatches = BitConverter.ToInt16(data, pointer + 20);
                var patches = new WallPatch[numPatches];
                for (var p = 0; p < numPatches; p++)
                {
                    var x = BitConverter.ToInt16(data, pointer + 22 + (p * 10));
                    var y = BitConverter.ToInt16(data, pointer + 22 + (p * 10) +2);
                    var pnum = BitConverter.ToInt16(data, pointer + 22 + (p *10)+4);
                    var pname = wadInfo.PatchNames[pnum];
                    var patch = new WallPatch(x,y,pnum,pname);
                    patches[p] = patch;
                }

                textures.Add(new WallTextureData(name, w, h, patches));
            }

            return textures;
        }

        // Gets a list of maps in the WAD
        // At this level, we only care about metadata - so we get the name of the map and pointers to resources containing the map data
        public static List<WADMapEntry> GetMapList(FileStream wadStream, WADInfo wadInfo)
        {
            var maps = new List<WADMapEntry>();

            // Map names are either in the format "ExMy" or "MAPxx" 
            // They also have a zero resource length as they're just markers in the WAD directory
            foreach(var entry in wadInfo.Entries.Where(e=>(e.Name.StartsWith("MAP")||(e.Name[0]=='E' && e.Name[2]=='M')) && e.ResourceLength==0))
            {
                var map = new WADMapEntry()
                {
                    Name = entry.Name
                };

                // The resource entries directly after the map marker are the actual map data
                // Let's find the ones we're interested in and hold pointers to them so we can get to them easily later
                var startIndex = wadInfo.Entries.IndexOf(entry);
                var index = startIndex + 1;
                while(index< startIndex + 10 || index>=wadInfo.Entries.Count-1)
                {
                    switch(wadInfo.Entries[index].Name)
                    {
                        case "THINGS": map.Things = wadInfo.Entries[index]; break;
                        case "LINEDEFS": map.LineDefs = wadInfo.Entries[index]; break;
                        case "SIDEDEFS": map.SideDefs = wadInfo.Entries[index]; break;
                        case "VERTEXES": map.Vertexes = wadInfo.Entries[index]; break;
                        case "SECTORS": map.Sectors = wadInfo.Entries[index]; break;
                    }

                    index++;
                }

                maps.Add(map);
            }

            return maps;
        }

        public static byte[] GetResource(FileStream wadStream, WADEntry entry)
        {
            wadStream.Seek(entry.ResourceOffset, SeekOrigin.Begin);

            var bytes = new byte[entry.ResourceLength];
            wadStream.Read(bytes, 0, entry.ResourceLength);

            return bytes;
        }

        public static Dictionary<string, byte[]> GetResources(FileStream wadStream, List<WADEntry> entries)
        {
            var resDict = new Dictionary<string, byte[]>();

            foreach (var wadEntry in entries)
                resDict.Add(wadEntry.Name, GetResource(wadStream, wadEntry));

            return resDict;
        }

       

        // Returns actual map data when given a map entry (from GetMapList)
        public static MapData GetMapData(FileStream wadStream, WADInfo wadInfo, WADMapEntry mapEntry)
        {
            var mapData = new MapData(wadInfo, mapEntry.Name,
                                      GetResource(wadStream, mapEntry.Things),
                                      GetResource(wadStream, mapEntry.LineDefs), 
                                      GetResource(wadStream, mapEntry.SideDefs), 
                                      GetResource(wadStream, mapEntry.Sectors), 
                                      GetResource(wadStream, mapEntry.Vertexes));

            return mapData;
        }

        

        public static PaletteData GetPalette(FileStream wadStream, int paletteNum)
        {
            var wadInfo = GetInfo(wadStream);

            var palRes = GetResource(wadStream, wadInfo.Entries.First(e => e.Name == "PLAYPAL"));

            var colors = new Color32[256];
            for (int i = 0; i < 256; i++)
            {
                colors[i] = new Color32(palRes[(paletteNum*256)+(i * 3)], palRes[(paletteNum * 256) + ((i*3)+1)], palRes[(paletteNum * 256) + ((i*3)+2)], 255);
            }

            return new PaletteData(colors);
        }

        // Get a picture sprite: basically, anything that is not a floor or ceiling texture
        public static SpriteData GetPictureSprite(FileStream wadStream, WADEntry spriteEntry)
        {
            var res = GetResource(wadStream, spriteEntry);

            return new SpriteData(spriteEntry.Name, SpriteType.Picture, BitConverter.ToInt16(res,0), BitConverter.ToInt16(res, 2), BitConverter.ToInt16(res, 4), BitConverter.ToInt16(res, 6), res);
        }

        public static List<SpriteData> GetPictureSprites(FileStream wadStream, List<WADEntry> spriteEntries)
        {
            var sprites = new List<SpriteData>();
            
            foreach(var e in spriteEntries)
                sprites.Add(GetPictureSprite(wadStream,e));

            return sprites;
        }

        // Get a raw sprite - floors and ceilings mostly
        public static SpriteData GetRawSprite(FileStream wadStream, WADEntry spriteEntry)
        {
            var res = GetResource(wadStream, spriteEntry);

            return new SpriteData(spriteEntry.Name, SpriteType.Raw, 64, 64, 0, 0, res);
        }

        public static List<SpriteData> GetRawSprites(FileStream wadStream, List<WADEntry> spriteEntries)
        {
            var sprites = new List<SpriteData>();

            foreach (var e in spriteEntries)
                sprites.Add(GetRawSprite(wadStream, e));

            return sprites;
        }

        public static Dictionary<string, SpriteData> GetWallPatches(FileStream wadStream, WADInfo wadInfo, WallTextureData textureData)
        {
            var sprites = new Dictionary<string, SpriteData>();

            foreach (var patch in textureData.Patches)
            {
                if(!sprites.ContainsKey(patch.PatchName))
                    sprites.Add(patch.PatchName, GetPictureSprite(wadStream, wadInfo.EntryDictionary[patch.PatchName]));
            }

            return sprites;
        }

        private static int ReadInt(FileStream wadStream, int start, int length)
        {
            wadStream.Seek(start, SeekOrigin.Begin);

            var bytes = new byte[length];
            wadStream.Read(bytes, 0, length);

            return BitConverter.ToInt32(bytes, 0);
        }

        private static string ReadString(FileStream wadStream, int start, int length)
        {
            wadStream.Seek(start, SeekOrigin.Begin);

            var bytes = new byte[length];
            wadStream.Read(bytes, 0, length);

            return Encoding.UTF8.GetString(bytes).Replace("\0", "");
        }
    }
}
