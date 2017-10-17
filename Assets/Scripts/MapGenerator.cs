using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityWAD;

namespace UnityWAD
{
    public class MapGenerator : MonoBehaviour
    {
        public MapData Data;
        public float Scale = 0.015f;

        public void GenerateMesh()
        {
            if (Data == null) return;

            // We're going to break the map into Sectors, with a child object for each
            // So start by looping through Sectors
            // Note that all the map data is referenced by array index, so we need to loop by index rather than foreaching
            for (var sector = 0; sector < Data.Sectors.Length; sector++)
            {
               

                var vertIndexList = new List<int>();
                var sectorLines = new List<MapLineDef>();

                // Sectors are made up of SideDefs, so we need to loop through sides that belong to this sector
                for (var side = 0; side < Data.SideDefs.Length; side++)
                {
                    if (Data.SideDefs[side].Sector == sector)
                    {
                        // Sides have a corresponding line, so let's get the line for this side
                        for (var line = 0; line < Data.LineDefs.Length; line++)
                        {
                            if (Data.LineDefs[line].Right == side)
                            {
                                // This side is for the Right side of the line
                                sectorLines.Add(Data.LineDefs[line]);
                                
                            }

                            if (Data.LineDefs[line].Left == side)
                            {
                                sectorLines.Add(Data.LineDefs[line]);
                                // This side is for the Left side of the line
                            }
                        }
                    }
                }

                if (sectorLines.Count == 0) continue;


                var currentLine = sectorLines[0];
                while (true)
                {
                    //var vertCount = vertIndexList.Count;

                    //if (vertIndexList.Count == vertCount) break;
                    if (!vertIndexList.Contains(currentLine.From)) vertIndexList.Add(currentLine.From);
                    else break;

                    bool found = false;
                    for (int i = 0; i < sectorLines.Count; i++)
                    {
                        if (i!=sectorLines.IndexOf(currentLine) && sectorLines[i].From == currentLine.To)
                        {
                            found = true;
                            
                            currentLine = sectorLines[i];
                            break;
                        }
                    }
                    if (!found) break;
                }

                

                var verts2D = new Vector2[vertIndexList.Count];
                for(var v =0;v<vertIndexList.Count;v++)
                {
                    var vert = new Vector2(Data.Vertexes[vertIndexList[v]].X * Scale, Data.Vertexes[vertIndexList[v]].Y*Scale);
                    verts2D[v] = vert;
                }

                verts2D = verts2D.Reverse().ToArray();

                var indices = EarClipper.Triangulate(verts2D);
                //var triangulator = new Triangulator(verts2D);
                //var indices = triangulator.Triangulate();

                //indices = indices.Reverse().ToArray();

                Vector3[] vertices = new Vector3[verts2D.Length];
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i] = new Vector3(verts2D[i].x, 0f, verts2D[i].y);

                var m = new Mesh
                {
                    vertices = vertices,
                    triangles = indices
                };
                m.RecalculateNormals();
                m.RecalculateBounds();

                //var m = HullMesher.BuildPolygon(verts3D);
                var newSector = InstantiateSector(sector);
                newSector.GetComponent<MeshFilter>().mesh = m;
            }

        }

        GameObject InstantiateSector(int num)
        {
            var newSector = new GameObject();
            newSector.name = "Sector " + num;
            newSector.AddComponent<MeshRenderer>();
            newSector.AddComponent<MeshFilter>();
            newSector.transform.SetParent(transform);
            newSector.transform.localPosition = Vector3.zero;
            return newSector;
        }
    }
}
