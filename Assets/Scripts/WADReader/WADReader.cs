using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WADReader
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
                wi.Entries.Add(new WADEntry()
                {
                    ResourceOffset = ReadInt(wadStream, entryOffset, 4),
                    ResourceLength = ReadInt(wadStream, entryOffset + 4, 4),
                    Name = ReadString(wadStream, entryOffset + 8, 8)
                });
            }

            return wi;
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

        // Returns actual map data when given a map entry (from GetMapList)
        public static MapData GetMapData(FileStream wadStream, WADMapEntry mapEntry)
        {
            var mapData = new MapData(mapEntry.Name,
                                      GetResource(wadStream, mapEntry.Things),
                                      GetResource(wadStream, mapEntry.LineDefs), 
                                      GetResource(wadStream, mapEntry.SideDefs), 
                                      GetResource(wadStream, mapEntry.Sectors), 
                                      GetResource(wadStream, mapEntry.Vertexes));

            return mapData;
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
