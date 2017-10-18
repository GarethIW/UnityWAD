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

        public Material FloorMaterial;
        public Material CeilingMaterial;
        public Material WallsMaterial;


        public void GenerateMesh()
        {
            if (Data == null) return;

            var wallVertices = new VertexList();
            var wallIndices = new List<int>();

            wallVertices.Mode = VertexList.VertexListMode.PerFace;

            foreach(var line in Data.LineDefs)
            {
                var rightSide = Data.SideDefs[line.Right];
                var leftSide = line.Left != -1 ? Data.SideDefs[line.Left] : null;

                var rightSector = Data.Sectors[rightSide.Sector];
                var leftSector = leftSide!=null ? Data.Sectors[leftSide.Sector] : null;

                if(!rightSide.FullTexture.StartsWith("-"))
                {
                    var tl = wallVertices.Add(new Vector3(Data.Vertexes[line.From].X, rightSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                    var tr = wallVertices.Add(new Vector3(Data.Vertexes[line.To].X, rightSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);
                    var bl = wallVertices.Add(new Vector3(Data.Vertexes[line.From].X, rightSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                    var br = wallVertices.Add(new Vector3(Data.Vertexes[line.To].X, rightSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);

                    wallIndices.AddRange(new[] { tr,br,tl,br,bl,tl });
                }
                if (!rightSide.UpperTexture.StartsWith("-") && leftSector!=null)
                {
                    var tl = wallVertices.Add(new Vector3(Data.Vertexes[line.From].X, rightSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                    var tr = wallVertices.Add(new Vector3(Data.Vertexes[line.To].X, rightSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);
                    var bl = wallVertices.Add(new Vector3(Data.Vertexes[line.From].X, leftSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                    var br = wallVertices.Add(new Vector3(Data.Vertexes[line.To].X, leftSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);

                    wallIndices.AddRange(new[] { tr, br, tl, br, bl, tl });
                }
                if (!rightSide.LowerTexture.StartsWith("-") && leftSector != null)
                {
                    var tl = wallVertices.Add(new Vector3(Data.Vertexes[line.From].X, leftSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                    var tr = wallVertices.Add(new Vector3(Data.Vertexes[line.To].X, leftSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);
                    var bl = wallVertices.Add(new Vector3(Data.Vertexes[line.From].X, rightSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                    var br = wallVertices.Add(new Vector3(Data.Vertexes[line.To].X, rightSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);

                    wallIndices.AddRange(new[] { tr, br, tl, br, bl, tl });
                }

                if (leftSide != null)
                {
                    if (!leftSide.FullTexture.StartsWith("-"))
                    {
                        var tl = wallVertices.Add(new Vector3(Data.Vertexes[line.From].X, leftSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                        var tr = wallVertices.Add(new Vector3(Data.Vertexes[line.To].X, leftSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);
                        var bl = wallVertices.Add(new Vector3(Data.Vertexes[line.From].X, leftSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                        var br = wallVertices.Add(new Vector3(Data.Vertexes[line.To].X, leftSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);

                        wallIndices.AddRange(new[] { tl,bl,br,tl,br,tr });
                    }
                    if (!leftSide.UpperTexture.StartsWith("-") && leftSector != null)
                    {
                        var tl = wallVertices.Add(new Vector3(Data.Vertexes[line.From].X, leftSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                        var tr = wallVertices.Add(new Vector3(Data.Vertexes[line.To].X, leftSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);
                        var bl = wallVertices.Add(new Vector3(Data.Vertexes[line.From].X, rightSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                        var br = wallVertices.Add(new Vector3(Data.Vertexes[line.To].X, rightSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);

                        wallIndices.AddRange(new[] { tl, bl, br, tl, br, tr });
                    }
                    if (!leftSide.LowerTexture.StartsWith("-") && leftSector != null)
                    {
                        var tl = wallVertices.Add(new Vector3(Data.Vertexes[line.From].X, rightSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                        var tr = wallVertices.Add(new Vector3(Data.Vertexes[line.To].X, rightSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);
                        var bl = wallVertices.Add(new Vector3(Data.Vertexes[line.From].X, leftSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                        var br = wallVertices.Add(new Vector3(Data.Vertexes[line.To].X, leftSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);

                        wallIndices.AddRange(new[] { tl, bl, br, tl, br, tr });
                    }
                }
            }

            var wallsMesh = new Mesh
            {
                vertices = wallVertices.ToArray(),
            };
            wallsMesh.SetIndices(wallIndices.ToArray(), MeshTopology.Triangles, 0);
            wallsMesh.RecalculateNormals();
            wallsMesh.RecalculateBounds();

            var walls = InstantiateChild("Walls");
            walls.GetComponent<MeshFilter>().mesh = wallsMesh;
            walls.GetComponent<MeshRenderer>().materials = new[] { WallsMaterial };
        }

        //public void GenerateMesh()
        //{
        //    if (Data == null) return;

        //    // We're going to break the map into Sectors, with a child object for each
        //    // So start by looping through Sectors
        //    // Note that all the map data is referenced by array index, so we need to loop by index rather than foreaching
        //    for (var sector = 0; sector < Data.Sectors.Length; sector++)
        //    {
        //        var sectorVertIndexList = new List<int>();
        //        var vertLookup = new Dictionary<int, int>();
        //        var sectorLines = new List<MapLineDef>();

        //        // Sectors are made up of SideDefs, so we need to loop through sides that belong to this sector
        //        for (var side = 0; side < Data.SideDefs.Length; side++)
        //        {
        //            if (Data.SideDefs[side].Sector == sector)
        //            {
        //                // Sides have a corresponding line, so let's get the line for this side
        //                for (var line = 0; line < Data.LineDefs.Length; line++)
        //                {
        //                    if (Data.LineDefs[line].Right == side)
        //                    {
        //                        // This side is for the Right side of the line
        //                        sectorLines.Add(Data.LineDefs[line]);
                                
        //                    }

        //                    if (Data.LineDefs[line].Left == side)
        //                    {
        //                        //sectorLines.Add(Data.LineDefs[line]);
        //                        // This side is for the Left side of the line
        //                    }
        //                }
        //            }
        //        }

        //        if (sectorLines.Count == 0) continue;

        //        // Get the sector outline for calculating vertices and floor & ceiling
        //        var currentLine = sectorLines[0];
        //        while (true)
        //        {
        //            if (!sectorVertIndexList.Contains(currentLine.From))             
        //                sectorVertIndexList.Add(currentLine.From);
        //            else break;

        //            bool found = false;
        //            for (int i = 0; i < sectorLines.Count; i++)
        //            {
        //                if (i!=sectorLines.IndexOf(currentLine) && sectorLines[i].From == currentLine.To)
        //                {
        //                    found = true;
                            
        //                    currentLine = sectorLines[i];
        //                    break;
        //                }
        //            }
        //            if (!found) break;
        //        }

        //        // Get the vertices from the map data and adjust the scale
        //        // Result in verts2D being a flattened, clockwise-ordered list of verts for our sector
        //        var verts2D = new Vector2[sectorVertIndexList.Count];
        //        for(var v =0;v<sectorVertIndexList.Count;v++)
        //        {
        //            var vert = new Vector2(Data.Vertexes[sectorVertIndexList[v]].X * Scale, Data.Vertexes[sectorVertIndexList[v]].Y*Scale);
        //            verts2D[v] = vert;

        //            vertLookup.Add(sectorVertIndexList[v], v);
        //        }

        //        // We need our verts to be anti-clockwise wound in order to triangulate them
        //        verts2D = verts2D.Reverse().ToArray();

        //        // Create Unity-space floor and ceiling verts
        //        Vector3[] vertices = new Vector3[verts2D.Length * 2];
        //        for (int i = 0; i < verts2D.Length; i++)
        //        {
        //            vertices[i] = new Vector3(verts2D[i].x, Data.Sectors[sector].FloorHeight * Scale, verts2D[i].y);
        //            vertices[i+verts2D.Length]  = new Vector3(verts2D[i].x, Data.Sectors[sector].CeilingHeight * Scale, verts2D[i].y);
        //        }

        //        // Triangulate floor
        //        var floorIndices = EarClipper.Triangulate(verts2D).ToArray();

        //        // Triangulate ceiling
        //        var ceilingIndices = EarClipper.Triangulate(verts2D).Reverse().ToArray();
        //        for (int i = 0; i < ceilingIndices.Length; i++)
        //            ceilingIndices[i] += verts2D.Length;

        //        // Triangulate walls
        //        List<int> wallIndices = new List<int>();
        //        foreach(var l in sectorLines)
        //        {
        //            var right = Data.SideDefs[l.Right];
        //            var bl = vertLookup[l.From];
        //            var br = vertLookup[l.To];
        //            var tl = vertLookup[l.From]+verts2D.Length;
        //            var tr = vertLookup[l.To]+verts2D.Length;
        //            wallIndices.AddRange(new[] { tl,bl,br,tl,br,tr });

        //            if (l.Left > -1)
        //            {
        //                var left = Data.SideDefs[l.Left];
        //                bl = vertLookup[l.From];
        //                br = vertLookup[l.To];
        //                tl = vertLookup[l.From] + verts2D.Length;
        //                tr = vertLookup[l.To] + verts2D.Length;
        //                wallIndices.AddRange(new[] { tr,br,tl,br,bl,tl });
        //            }
        //        }

        //        // Set the mesh: Three submeshes make up floor, ceiling, and walls
        //        var sectorMesh = new Mesh
        //        {
        //            subMeshCount = 3,
        //            vertices = vertices,
        //        };
        //        sectorMesh.SetIndices(floorIndices, MeshTopology.Triangles, 0);
        //        sectorMesh.SetIndices(ceilingIndices, MeshTopology.Triangles, 1);
        //        sectorMesh.SetIndices(wallIndices.ToArray(), MeshTopology.Triangles, 2);

        //        sectorMesh.RecalculateNormals();
        //        sectorMesh.RecalculateBounds();

        //        var newSector = InstantiateSector(sector);
        //        newSector.GetComponent<MeshFilter>().mesh = sectorMesh;
        //        newSector.GetComponent<MeshRenderer>().materials = new [] { FloorMaterial, CeilingMaterial, WallsMaterial };
        //    }

        //}

        GameObject InstantiateChild(string name)
        {
            var newChild = new GameObject();
            newChild.name = name;
            newChild.AddComponent<MeshRenderer>();
            newChild.AddComponent<MeshFilter>();
            newChild.transform.SetParent(transform);
            newChild.transform.localPosition = Vector3.zero;
            return newChild;
        }
    }
}
