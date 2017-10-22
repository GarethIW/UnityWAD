using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityWAD;
using Triangulator;

namespace UnityWAD
{
    public class MapGenerator : MonoBehaviour
    {
        public MapData Data;
        public Vector2 Grid = new Vector2(64f,64f); // Line length of one map grid unit, used to calculate uv scaling

        public float Scale = 0.015625f;

        public TileSheet WallTiles;
        public TileSheet FlatTiles;

        public Material WallsMaterial;
        public Material FlatsMaterial;


        public void GenerateMesh()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            if (Data == null) return;

            Scale = 1f / Grid.x;

            var wallVertices = new Dictionary<int,VertexList>();
            var wallUVs = new Dictionary<int, List<Vector4>>();
            var wallUV1s = new Dictionary<int, List<Vector4>>();
            var wallIndices = new Dictionary<int, List<int>>();

            var sectorLines = new Dictionary<int, List<MapLineDef>>();


            for (var i = 0; i < Data.Sectors.Length; i++)
            {
                wallVertices.Add(i, new VertexList());
                wallUVs.Add(i, new List<Vector4>());
                wallUV1s.Add(i, new List<Vector4>());
                wallIndices.Add(i, new List<int>());

                sectorLines.Add(i, new List<MapLineDef>());
            }


            foreach (var line in Data.LineDefs)
            {
                var rightSide = Data.SideDefs[line.Right];
                var leftSide = line.Left != -1 ? Data.SideDefs[line.Left] : null;

                var rightSector = Data.Sectors[rightSide.Sector];
                var leftSector = leftSide!=null ? Data.Sectors[leftSide.Sector] : null;

                sectorLines[rightSide.Sector].Add(line);
                if(leftSide!=null)
                    sectorLines[leftSide.Sector].Add(line);

                var widthRatio = (1f/Grid.x) * Vector2.Distance(new Vector2(Data.Vertexes[line.From].X, Data.Vertexes[line.From].Y), new Vector2(Data.Vertexes[line.To].X, Data.Vertexes[line.To].Y));

                if(!rightSide.FullTexture.StartsWith("-"))
                {
                    var tl = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, rightSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                    var tr = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, rightSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);
                    var bl = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, rightSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                    var br = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, rightSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);
                    wallIndices[rightSide.Sector].AddRange(new[] { tr,br,tl,br,bl,tl });

                    var heightRatio = (1f / Grid.y) * (rightSector.CeilingHeight - rightSector.FloorHeight);
                    var tileNum = WallTiles.LookupTable[rightSide.FullTexture].TileNum;
                    var tw = WallTiles.LookupTable[rightSide.FullTexture].Width;
                    var th = WallTiles.LookupTable[rightSide.FullTexture].Height;
                    wallUVs[rightSide.Sector].Add(new Vector4(0f, 0f, tileNum, 0f));
                    wallUVs[rightSide.Sector].Add(new Vector4(widthRatio, 0f, tileNum, 0f));
                    wallUVs[rightSide.Sector].Add(new Vector4(0f, heightRatio, tileNum, 0f));
                    wallUVs[rightSide.Sector].Add(new Vector4(widthRatio, heightRatio, tileNum, 0f));

                    for (var i = 0; i < 4; i++) wallUV1s[rightSide.Sector].Add(new Vector4(tw,th,rightSide.XOffset,rightSide.YOffset));
                }
                if (!rightSide.UpperTexture.StartsWith("-") && leftSector!=null)
                {
                    var tl = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, rightSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                    var tr = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, rightSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);
                    var bl = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, leftSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                    var br = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, leftSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);
                    wallIndices[rightSide.Sector].AddRange(new[] { tr, br, tl, br, bl, tl });

                    var heightRatio = (1f / Grid.y) * (rightSector.CeilingHeight - leftSector.CeilingHeight);
                    var tileNum = WallTiles.LookupTable[rightSide.UpperTexture].TileNum;
                    var tw = WallTiles.LookupTable[rightSide.UpperTexture].Width;
                    var th = WallTiles.LookupTable[rightSide.UpperTexture].Height;
                    wallUVs[rightSide.Sector].Add(new Vector4(0f, 0f, tileNum, 0f));
                    wallUVs[rightSide.Sector].Add(new Vector4(widthRatio, 0f, tileNum, 0f));
                    wallUVs[rightSide.Sector].Add(new Vector4(0f, heightRatio, tileNum, 0f));
                    wallUVs[rightSide.Sector].Add(new Vector4(widthRatio, heightRatio, tileNum, 0f));

                    for (var i = 0; i < 4; i++) wallUV1s[rightSide.Sector].Add(new Vector4(tw, th, rightSide.XOffset, rightSide.YOffset));
                }
                if (!rightSide.LowerTexture.StartsWith("-") && leftSector != null)
                {
                    var tl = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, leftSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                    var tr = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, leftSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);
                    var bl = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, rightSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                    var br = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, rightSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);
                    wallIndices[rightSide.Sector].AddRange(new[] { tr, br, tl, br, bl, tl });

                    var heightRatio = (1f / Grid.y) * (leftSector.FloorHeight - rightSector.FloorHeight);
                    var tileNum = WallTiles.LookupTable[rightSide.LowerTexture].TileNum;
                    var tw = WallTiles.LookupTable[rightSide.LowerTexture].Width;
                    var th = WallTiles.LookupTable[rightSide.LowerTexture].Height;
                    wallUVs[rightSide.Sector].Add(new Vector4(0f, 0f, tileNum, 0f));
                    wallUVs[rightSide.Sector].Add(new Vector4(widthRatio, 0f, tileNum, 0f));
                    wallUVs[rightSide.Sector].Add(new Vector4(0f, heightRatio, tileNum, 0f));
                    wallUVs[rightSide.Sector].Add(new Vector4(widthRatio, heightRatio, tileNum, 0f));

                    for (var i = 0; i < 4; i++) wallUV1s[rightSide.Sector].Add(new Vector4(tw, th, rightSide.XOffset, rightSide.YOffset));
                }

                if (leftSide != null)
                {
                    if (!leftSide.FullTexture.StartsWith("-"))
                    {
                        var tl = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, leftSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                        var tr = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, leftSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);
                        var bl = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, leftSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                        var br = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, leftSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);
                        wallIndices[leftSide.Sector].AddRange(new[] { tl, bl, br, tl, br, tr });

                        var heightRatio = (1f / Grid.y) * (leftSector.CeilingHeight - leftSector.FloorHeight);
                        var tileNum = WallTiles.LookupTable[leftSide.FullTexture].TileNum;
                        var tw = WallTiles.LookupTable[leftSide.FullTexture].Width;
                        var th = WallTiles.LookupTable[leftSide.FullTexture].Height;
                        wallUVs[leftSide.Sector].Add(new Vector4(0f, 0f, tileNum, 0f));
                        wallUVs[leftSide.Sector].Add(new Vector4(widthRatio, 0f, tileNum, 0f));
                        wallUVs[leftSide.Sector].Add(new Vector4(0f, heightRatio, tileNum, 0f));
                        wallUVs[leftSide.Sector].Add(new Vector4(widthRatio, heightRatio, tileNum, 0f));

                        for (var i = 0; i < 4; i++) wallUV1s[leftSide.Sector].Add(new Vector4(tw, th, leftSide.XOffset, leftSide.YOffset));
                    }
                    if (!leftSide.UpperTexture.StartsWith("-") && leftSector != null)
                    {
                        var tl = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, leftSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                        var tr = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, leftSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);
                        var bl = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, rightSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                        var br = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, rightSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);
                        wallIndices[leftSide.Sector].AddRange(new[] { tl, bl, br, tl, br, tr });

                        var heightRatio = (1f / Grid.y) * (leftSector.CeilingHeight - rightSector.CeilingHeight);
                        var tileNum = WallTiles.LookupTable[leftSide.UpperTexture].TileNum;
                        var tw = WallTiles.LookupTable[leftSide.UpperTexture].Width;
                        var th = WallTiles.LookupTable[leftSide.UpperTexture].Height;
                        wallUVs[leftSide.Sector].Add(new Vector4(0f, 0f, tileNum, 0f));
                        wallUVs[leftSide.Sector].Add(new Vector4(widthRatio, 0f, tileNum, 0f));
                        wallUVs[leftSide.Sector].Add(new Vector4(0f, heightRatio, tileNum, 0f));
                        wallUVs[leftSide.Sector].Add(new Vector4(widthRatio, heightRatio, tileNum, 0f));

                        for (var i = 0; i < 4; i++) wallUV1s[leftSide.Sector].Add(new Vector4(tw, th, leftSide.XOffset, leftSide.YOffset));

                    }
                    if (!leftSide.LowerTexture.StartsWith("-") && leftSector != null)
                    {
                        var tl = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, rightSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                        var tr = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, rightSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);
                        var bl = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, leftSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                        var br = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, leftSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);
                        wallIndices[leftSide.Sector].AddRange(new[] { tl, bl, br, tl, br, tr });

                        var heightRatio = (1f / Grid.y) * (rightSector.FloorHeight - leftSector.FloorHeight);
                        var tileNum = WallTiles.LookupTable[leftSide.LowerTexture].TileNum;
                        var tw = WallTiles.LookupTable[leftSide.LowerTexture].Width;
                        var th = WallTiles.LookupTable[leftSide.LowerTexture].Height;
                        wallUVs[leftSide.Sector].Add(new Vector4(0f, 0f, tileNum, 0f));
                        wallUVs[leftSide.Sector].Add(new Vector4(widthRatio, 0f, tileNum, 0f));
                        wallUVs[leftSide.Sector].Add(new Vector4(0f, heightRatio, tileNum, 0f));
                        wallUVs[leftSide.Sector].Add(new Vector4(widthRatio, heightRatio, tileNum, 0f));

                        for (var i = 0; i < 4; i++) wallUV1s[leftSide.Sector].Add(new Vector4(tw, th, leftSide.XOffset, leftSide.YOffset));

                    }
                }
            }

            for (var sectorNum = 0; sectorNum < Data.Sectors.Length; sectorNum++)
            {
                var sectorContainer = InstantiateChild(transform, "Sector " + sectorNum);

                var brightness = (byte)Data.Sectors[sectorNum].Brightness;

                var wallsMesh = new Mesh
                {
                    vertices = wallVertices[sectorNum].ToArray(),
                };
                wallsMesh.SetIndices(wallIndices[sectorNum].ToArray(), MeshTopology.Triangles, 0);
                wallsMesh.SetUVs(0, wallUVs[sectorNum]);
                wallsMesh.SetUVs(1, wallUV1s[sectorNum]);
                wallsMesh.RecalculateNormals();
                wallsMesh.RecalculateBounds();

                var walls = InstantiateChild(sectorContainer.transform, "Walls");
                walls.GetComponent<MeshFilter>().mesh = wallsMesh;
                var wm = Instantiate(WallsMaterial);
                wm.SetColor("_Color", new Color32(brightness, brightness, brightness, 255));
                walls.GetComponent<MeshRenderer>().materials = new[] { wm };

                // We don't have enough points to create floor/ceiling
                if (sectorLines[sectorNum].Count < 3) continue;

                // Get the sector outline for calculating vertices and floor & ceiling
                var sectorVertIndexList = new List<int>();
                var currentLine = sectorLines[sectorNum][0];
                bool workingFrom = true;
                while (true)
                {
                    //if (sectorNum == 47)
                    //{
                    //    Debug.Log("Need=" + (workingFrom ? currentLine.To : currentLine.From));
                    //    foreach (var sl in sectorLines[sectorNum])
                    //    {
                    //        Debug.Log((sl==currentLine&&workingFrom?"*":"") + "F:" + sl.From + "(" + Data.Vertexes[sl.From].ToString() + ") T:" + sl.To + "(" + Data.Vertexes[sl.To].ToString() + ")"+ (sl == currentLine && !workingFrom ? "*" : ""));
                    //    }
                    //}

                    if (workingFrom)
                    {
                        if (!sectorVertIndexList.Contains(currentLine.From))
                            sectorVertIndexList.Add(currentLine.From);
                        else break;
                    }
                    else
                    {
                        if (!sectorVertIndexList.Contains(currentLine.To))
                            sectorVertIndexList.Add(currentLine.To);
                        else break;
                    }

                    //if (sectorNum == 47)
                    //{
                    //    Debug.Log(sectorVertIndexList.Count);
                    //    foreach (var sl in sectorVertIndexList)
                    //    {

                    //        Debug.Log(Data.Vertexes[sl].ToString());
                    //    }
                    //}

                    sectorLines[sectorNum].Remove(currentLine);

                    bool found = false;
                    for (int l = 0; l < sectorLines[sectorNum].Count; l++)
                    {
                        if ((workingFrom && (sectorLines[sectorNum][l].From == currentLine.To || sectorLines[sectorNum][l].To == currentLine.To)) || (!workingFrom && (sectorLines[sectorNum][l].From == currentLine.From || sectorLines[sectorNum][l].To == currentLine.From)))
                        {
                            found = true;
                            if (workingFrom)
                                workingFrom = sectorLines[sectorNum][l].From == currentLine.To;
                            else
                                workingFrom = !(sectorLines[sectorNum][l].To == currentLine.From);
                            currentLine = sectorLines[sectorNum][l];
                            break;
                        }
                    }
                    if (!found) break;

                    
                }
                if (sectorNum == 47)
                        Debug.Log(sectorVertIndexList.Count);

                if (sectorNum == 47)
                    foreach (var sl in sectorVertIndexList)
                    {
                        Debug.Log(Data.Vertexes[sl].ToString());
                    }

                var verts2D = new Vector2[sectorVertIndexList.Count];
                for (var v = 0; v < sectorVertIndexList.Count; v++)
                    verts2D[v] = new Vector2(Data.Vertexes[sectorVertIndexList[v]].X, Data.Vertexes[sectorVertIndexList[v]].Y);

                // See if we have a tile for the floor and ceiling textures
                var floorTileNum = -1;
                if (FlatTiles.LookupTable.ContainsKey(Data.Sectors[sectorNum].FloorTexture.Replace("\0",""))) floorTileNum = FlatTiles.LookupTable[Data.Sectors[sectorNum].FloorTexture.Replace("\0", "")].TileNum;
                var ceilingTileNum = -1;
                if (FlatTiles.LookupTable.ContainsKey(Data.Sectors[sectorNum].CeilingTexture.Replace("\0",""))) ceilingTileNum = FlatTiles.LookupTable[Data.Sectors[sectorNum].CeilingTexture.Replace("\0", "")].TileNum;

                // We don't want to render geometry with sky texture
                if (Data.Sectors[sectorNum].FloorTexture.StartsWith("F_SKY")) floorTileNum = -1;
                if (Data.Sectors[sectorNum].CeilingTexture.StartsWith("F_SKY")) ceilingTileNum = -1;

                // Triangulate floor
                int[] floorIndices;
                Vector2[] floorVerts;
                Triangulator.Triangulator.Triangulate(verts2D, WindingOrder.Clockwise, out floorVerts, out floorIndices);

                // Triangulate ceiling (reverse winding)
                int[] ceilIndices;
                Vector2[] ceilVerts;
                Triangulator.Triangulator.Triangulate(verts2D, WindingOrder.CounterClockwise, out ceilVerts, out ceilIndices);

                // Pack floor and ceiling into one mesh
                // But we only want to render if we have a texture tile, so exclude indices if we don't
                var totalInds = (floorTileNum != -1 ? floorIndices.Length : 0) + (ceilingTileNum != -1 ? ceilIndices.Length : 0);
                var flatIndices = new int[totalInds];
                var pos = 0;
                if (floorTileNum != -1)
                {
                    Array.Copy(floorIndices, flatIndices, floorIndices.Length);
                    pos += floorIndices.Length;
                }
                if (ceilingTileNum != -1)
                {
                    for (int i = 0; i < ceilIndices.Length; i++) ceilIndices[i] += floorVerts.Length;
                    Array.Copy(ceilIndices, 0, flatIndices, pos, ceilIndices.Length);
                }

                Vector3[] flatVertices = new Vector3[floorVerts.Length * 2];
                Vector4[] flatUVs = new Vector4[floorVerts.Length * 2];
                
                for (int i = 0; i < floorVerts.Length; i++)
                {
                    flatVertices[i] = new Vector3(floorVerts[i].x*Scale, Data.Sectors[sectorNum].FloorHeight * Scale, floorVerts[i].y*Scale);
                    flatVertices[i + floorVerts.Length] = new Vector3(ceilVerts[i].x *Scale, Data.Sectors[sectorNum].CeilingHeight * Scale, ceilVerts[i].y*Scale);

                    flatUVs[i] = new Vector4(0, 0, floorTileNum, 0);
                    flatUVs[i + floorVerts.Length] = new Vector4(0, 0, ceilingTileNum, 0);
                }

                var flatsMesh = new Mesh
                {
                    vertices = flatVertices,
                };
                flatsMesh.SetIndices(flatIndices, MeshTopology.Triangles, 0);
                flatsMesh.SetUVs(0, flatUVs.ToList());
                flatsMesh.RecalculateNormals();
                flatsMesh.RecalculateBounds();

                var floor = InstantiateChild(sectorContainer.transform, "Flats");
                floor.GetComponent<MeshFilter>().mesh = flatsMesh;
                var fm = Instantiate(FlatsMaterial);
                fm.SetColor("_Color", new Color32(brightness,brightness,brightness,255));
                floor.GetComponent<MeshRenderer>().materials = new[] { fm };
            }
        }

        GameObject InstantiateChild(Transform parent, string name)
        {
            var newChild = new GameObject();
            newChild.name = name;
            newChild.AddComponent<MeshRenderer>();
            newChild.AddComponent<MeshFilter>();
            newChild.transform.SetParent(parent);
            newChild.transform.localPosition = Vector3.zero;
            return newChild;
        }
    }
}
