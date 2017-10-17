using System;
using System.Collections.Generic;
using System.Text;

namespace UnityWAD
{
    public class MapData
    {
        public string Name;
        public MapLineDef[] LineDefs;
        public MapSideDef[] SideDefs;
        public MapSector[] Sectors;
        public MapVertex[] Vertexes;

        public MapData(string name, byte[] things, byte[] lineDefData, byte[] sideDefData, byte[] sectorData, byte[] vertexData)
        {
            Name = name;

            //TODO : Things
            LineDefs = new MapLineDef[lineDefData.Length / 14];
            SideDefs = new MapSideDef[sideDefData.Length / 30];
            Sectors = new MapSector[sectorData.Length / 26];
            Vertexes = new MapVertex[vertexData.Length / 4];

            var pos = 0;
            for (var i = 0; i < LineDefs.Length; i++)
            {
                LineDefs[i] = new MapLineDef(BitConverter.ToInt16(lineDefData, pos),
                                             BitConverter.ToInt16(lineDefData, pos + 2),
                                             BitConverter.ToInt16(lineDefData, pos + 4),
                                             BitConverter.ToInt16(lineDefData, pos + 6),
                                             BitConverter.ToInt16(lineDefData, pos + 8),
                                             BitConverter.ToInt16(lineDefData, pos + 10),
                                             BitConverter.ToInt16(lineDefData, pos + 12));
                pos += 14;
            }

            pos = 0;
            for (var i = 0; i < SideDefs.Length; i++)
            {
                SideDefs[i] = new MapSideDef(BitConverter.ToInt16(sideDefData, pos),
                                             BitConverter.ToInt16(sideDefData, pos + 2),
                                             Encoding.UTF8.GetString(sideDefData, pos + 4, 8),
                                             Encoding.UTF8.GetString(sideDefData, pos + 12, 8),
                                             Encoding.UTF8.GetString(sideDefData, pos + 20, 8),
                                             BitConverter.ToInt16(sideDefData, pos + 28));
                pos += 30;
            }

            pos = 0;
            for (var i = 0; i < Sectors.Length; i++)
            {
                Sectors[i] = new MapSector(BitConverter.ToInt16(sectorData, pos),
                                           BitConverter.ToInt16(sectorData, pos + 2),
                                           Encoding.UTF8.GetString(sectorData, pos + 4, 8),
                                           Encoding.UTF8.GetString(sectorData, pos + 12, 8),
                                           BitConverter.ToInt16(sectorData, pos + 20),
                                           BitConverter.ToInt16(sectorData, pos + 22),
                                           BitConverter.ToInt16(sectorData, pos + 24));
                pos += 26;
            }

            pos = 0;
            for (var i = 0; i < Vertexes.Length; i++)
            {
                Vertexes[i] = new MapVertex(BitConverter.ToInt16(vertexData, pos),
                                            BitConverter.ToInt16(vertexData, pos + 2));
                pos += 4;
            }
        }
    }
}
