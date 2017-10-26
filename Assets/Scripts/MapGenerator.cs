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

            // Scale to the Doom grid size. 64x64 pixel grid gives us the correct scale for 1 Unity unit
            Scale = 1f / Grid.x;

            // Dictionaries of lists. We're building by sector so we'll index by sector
            var wallVertices = new Dictionary<int,VertexList>();    // Vertices are per-face because we're tilemapping so we can't share them
            var wallUVs = new Dictionary<int, List<Vector4>>();     // UV0: (uv X,uv Y,tile index (index of wall texture in texture tilesheet),0)
            var wallUV1s = new Dictionary<int, List<Vector4>>();    // UV1: (tile width,tile height (dimensions of wall sprite *within* the tile indexed in uv0),X offset,Y offset (offsets as defined in SideDefs))
            var wallIndices = new Dictionary<int, List<int>>();

            var sectorLines = new Dictionary<int, List<MapLineDef>>();

            for (var i = 0; i < Data.Sectors.Length; i++)
            {
                wallVertices.Add(i, new VertexList());
                wallUVs.Add(i, new List<Vector4>());
                wallUV1s.Add(i, new List<Vector4>());
                wallIndices.Add(i, new List<int>());

                // A list to hold all the lines that belong to this sector
                // i.e. any lines where the left or right sidedef sector equals the sector being examined
                sectorLines.Add(i, new List<MapLineDef>());
            }

            // Generate some walls!
            foreach (var line in Data.LineDefs)
            {
                // Each line always has a right sidedef
                var rightSide = Data.SideDefs[line.Right];
                // A left sidedef of -1 means no left side
                var leftSide = line.Left != -1 ? Data.SideDefs[line.Left] : null;

                var rightSector = Data.Sectors[rightSide.Sector];
                var leftSector = leftSide!=null ? Data.Sectors[leftSide.Sector] : null;

                sectorLines[rightSide.Sector].Add(line);
                if(leftSide!=null)
                    sectorLines[leftSide.Sector].Add(line);
 

                // Calculate the ratio of the lines's length to the Doom grid size (64x64). We use this to set the UVs so that wall textures can be tiled instead of stretching
                var widthRatio = (1f/Grid.x) * Vector2.Distance(new Vector2(Data.Vertexes[line.From].X, Data.Vertexes[line.From].Y), new Vector2(Data.Vertexes[line.To].X, Data.Vertexes[line.To].Y));

                // Full-height walls stretch from the sector's ceiling height (top) to the sector's floor height (bottom)
                // Start with right-side walls
                // A "-" means no texture, so we don't render that side of the wall
                if (!rightSide.FullTexture.StartsWith("-"))
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

                // Upper walls stretch from this sector's ceiling height (top) to the neighbouring (left side) sector's ceiling height (bottom)
                if (!rightSide.UpperTexture.StartsWith("-") && leftSector!=null && !(leftSector.CeilingTexture.StartsWith("F_SKY")))
                {
                    // Dont draw walls with a zero or negative height
                    var heightRatio = (1f / Grid.y) * (rightSector.CeilingHeight - leftSector.CeilingHeight);
                    if (heightRatio > 0f)
                    {
                        var tl = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, rightSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                        var tr = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, rightSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);
                        var bl = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, leftSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                        var br = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, leftSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);
                        wallIndices[rightSide.Sector].AddRange(new[] { tr, br, tl, br, bl, tl });

                        var tileNum = WallTiles.LookupTable[rightSide.UpperTexture].TileNum;
                        var tw = WallTiles.LookupTable[rightSide.UpperTexture].Width;
                        var th = WallTiles.LookupTable[rightSide.UpperTexture].Height;
                        wallUVs[rightSide.Sector].Add(new Vector4(0f, 0f, tileNum, 0f));
                        wallUVs[rightSide.Sector].Add(new Vector4(widthRatio, 0f, tileNum, 0f));
                        wallUVs[rightSide.Sector].Add(new Vector4(0f, heightRatio, tileNum, 0f));
                        wallUVs[rightSide.Sector].Add(new Vector4(widthRatio, heightRatio, tileNum, 0f));

                        for (var i = 0; i < 4; i++) wallUV1s[rightSide.Sector].Add(new Vector4(tw, th, rightSide.XOffset, rightSide.YOffset));
                    }
                }

                // Lower walls stretch from the neighbouring (left side) sector's floor height (top) to the this sector's floor height (bottom)
                if (!rightSide.LowerTexture.StartsWith("-") && leftSector != null)
                {
                    // Dont draw walls with a zero or negative height
                    var heightRatio = (1f / Grid.y) * (leftSector.FloorHeight - rightSector.FloorHeight);
                    if (heightRatio > 0f)
                    {
                        var tl = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, leftSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                        var tr = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, leftSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);
                        var bl = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, rightSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                        var br = wallVertices[rightSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, rightSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);
                        wallIndices[rightSide.Sector].AddRange(new[] { tr, br, tl, br, bl, tl });

                        var tileNum = WallTiles.LookupTable[rightSide.LowerTexture].TileNum;
                        var tw = WallTiles.LookupTable[rightSide.LowerTexture].Width;
                        var th = WallTiles.LookupTable[rightSide.LowerTexture].Height;
                        wallUVs[rightSide.Sector].Add(new Vector4(0f, 0f, tileNum, 0f));
                        wallUVs[rightSide.Sector].Add(new Vector4(widthRatio, 0f, tileNum, 0f));
                        wallUVs[rightSide.Sector].Add(new Vector4(0f, heightRatio, tileNum, 0f));
                        wallUVs[rightSide.Sector].Add(new Vector4(widthRatio, heightRatio, tileNum, 0f));

                        for (var i = 0; i < 4; i++) wallUV1s[rightSide.Sector].Add(new Vector4(tw, th, rightSide.XOffset, rightSide.YOffset));
                    }
                }

                // Now left-side walls, pretty much the same as above but reversed by sector
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
                        var heightRatio = (1f / Grid.y) * (leftSector.CeilingHeight - rightSector.CeilingHeight);
                        if (heightRatio > 0f)
                        {
                            var tl = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, leftSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                            var tr = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, leftSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);
                            var bl = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, rightSector.CeilingHeight, Data.Vertexes[line.From].Y) * Scale);
                            var br = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, rightSector.CeilingHeight, Data.Vertexes[line.To].Y) * Scale);
                            wallIndices[leftSide.Sector].AddRange(new[] { tl, bl, br, tl, br, tr });

                            var tileNum = WallTiles.LookupTable[leftSide.UpperTexture].TileNum;
                            var tw = WallTiles.LookupTable[leftSide.UpperTexture].Width;
                            var th = WallTiles.LookupTable[leftSide.UpperTexture].Height;
                            wallUVs[leftSide.Sector].Add(new Vector4(0f, 0f, tileNum, 0f));
                            wallUVs[leftSide.Sector].Add(new Vector4(widthRatio, 0f, tileNum, 0f));
                            wallUVs[leftSide.Sector].Add(new Vector4(0f, heightRatio, tileNum, 0f));
                            wallUVs[leftSide.Sector].Add(new Vector4(widthRatio, heightRatio, tileNum, 0f));

                            for (var i = 0; i < 4; i++) wallUV1s[leftSide.Sector].Add(new Vector4(tw, th, leftSide.XOffset, leftSide.YOffset));
                        }

                    }
                    if (!leftSide.LowerTexture.StartsWith("-") && leftSector != null)
                    {
                        var heightRatio = (1f / Grid.y) * (rightSector.FloorHeight - leftSector.FloorHeight);
                        if (heightRatio > 0f)
                        {
                            var tl = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, rightSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                            var tr = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, rightSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);
                            var bl = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.From].X, leftSector.FloorHeight, Data.Vertexes[line.From].Y) * Scale);
                            var br = wallVertices[leftSide.Sector].Add(new Vector3(Data.Vertexes[line.To].X, leftSector.FloorHeight, Data.Vertexes[line.To].Y) * Scale);
                            wallIndices[leftSide.Sector].AddRange(new[] { tl, bl, br, tl, br, tr });

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
            }

            


            // Done with walls, now let's start on the floors and ceilings (unofficial Doom docs call them "flats", so we'll do the same)
            // Note that we don't have any data in the WAD file for this, because flats were rendered by raycast
            // So we need to construct our own polys by concave hull triangulation
            // And we need to do this by sector, so let's loop those
            for (var sectorNum = 0; sectorNum < Data.Sectors.Length; sectorNum++)
            {
                var sectorContainer = InstantiateChild(transform, "Sector " + sectorNum);

                // Brightness gives us the amount of light for this sector (yeah, an early version of vertex lighting?)
                // We'll set the Color on the sector's material later according to this brightness
                // Another reason to divide the overall mesh into sectors
                var brightness = (byte)Data.Sectors[sectorNum].Brightness;

                // We've calced the walls, so let's build the meshes for those first
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

                // Now we can start on the flats

                // We don't have enough points to create a flat for this sector
                if (sectorLines[sectorNum].Count < 3) continue;

                //// Get a copy of lines where the left side is this sector (we'll use this later for cutting holes)


                //// Get the sector outline for calculating vertices for the flats
                //// We're going to loop through all the walls that belong to this sector and attempt to join them up
                //// Kind of like Join the Dots, but we need to do it so the vertices are ordered (so we can wind them correctly for triangulation)
                //// So we start on the first line, then find a line where either end connects to the end of that line. And repeat.
                //// Add the connecting vertex of each line to a (hopefully) ordered and closed-loop list
                var sectorVertIndexList = new List<int>();
                var currentLine = sectorLines[sectorNum][0];
                bool workingFrom = true;
                while (true)
                {
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
                                workingFrom = sectorLines[sectorNum][l].To != currentLine.From;
                            currentLine = sectorLines[sectorNum][l];
                            break;
                        }
                    }
                    if (!found) break;


                }
              
                // We have our vertex list, but they're currently just pointers to the vertex index in the map data, so let's actually get the V2s
                var verts2D = new Vector2[sectorVertIndexList.Count];
                for (var v = 0; v < sectorVertIndexList.Count; v++)
                    verts2D[v] = new Vector2(Data.Vertexes[sectorVertIndexList[v]].X, Data.Vertexes[sectorVertIndexList[v]].Y);

                // See if we have a tile index for the flat textures
                var floorTileNum = -1;
                if (FlatTiles.LookupTable.ContainsKey(Data.Sectors[sectorNum].FloorTexture.Replace("\0", ""))) floorTileNum = FlatTiles.LookupTable[Data.Sectors[sectorNum].FloorTexture.Replace("\0", "")].TileNum;
                var ceilingTileNum = -1;
                if (FlatTiles.LookupTable.ContainsKey(Data.Sectors[sectorNum].CeilingTexture.Replace("\0", ""))) ceilingTileNum = FlatTiles.LookupTable[Data.Sectors[sectorNum].CeilingTexture.Replace("\0", "")].TileNum;

                // We don't want to render geometry with sky texture (we want to see through to a potential skybox)
                if (Data.Sectors[sectorNum].FloorTexture.StartsWith("F_SKY")) floorTileNum = -1;
                if (Data.Sectors[sectorNum].CeilingTexture.StartsWith("F_SKY")) ceilingTileNum = -1;

                // Use left-side lines to cut holes in our flats
                var sectorLeftLines = Data.LineDefs.Where(l => l.Left != -1 && Data.SideDefs[l.Left].Sector == sectorNum).ToList();

                var holes = new List<List<int>>();

                var currentHole = 0;
                if (sectorLeftLines.Count > 3)
                {
                    while (true)
                    {
                        if (sectorLeftLines.Count < 4) break;

                        holes.Add(new List<int>());

                        currentLine = sectorLeftLines[0];
                        workingFrom = true;
                        while (true)
                        {
                            if (workingFrom)
                            {
                                if (!holes[currentHole].Contains(currentLine.From))
                                    holes[currentHole].Add(currentLine.From);
                                else break;
                            }
                            else
                            {
                                if (!holes[currentHole].Contains(currentLine.To))
                                    holes[currentHole].Add(currentLine.To);
                                else break;
                            }

                            sectorLeftLines.Remove(currentLine);

                            bool found = false;
                            for (int l = 0; l < sectorLeftLines.Count; l++)
                            {
                                if ((workingFrom && (sectorLeftLines[l].From == currentLine.To || sectorLeftLines[l].To == currentLine.To)) || (!workingFrom && (sectorLeftLines[l].From == currentLine.From || sectorLeftLines[l].To == currentLine.From)))
                                {
                                    found = true;
                                    if (workingFrom)
                                        workingFrom = sectorLeftLines[l].From == currentLine.To;
                                    else
                                        workingFrom = sectorLeftLines[l].To != currentLine.From;
                                    currentLine = sectorLeftLines[l];
                                    break;
                                }
                            }
                            if (!found) break;
                        }

                        currentHole++;
                    }
                }

                


                foreach (var h in holes)
                {
                    var holeVerts2D = new Vector2[h.Count];
                    for (var v = 0; v < h.Count; v++)
                    {
                        holeVerts2D[v] = new Vector2(Data.Vertexes[h[v]].X, Data.Vertexes[h[v]].Y);
                    }

                    //Cutting holes currently not working (based on a simple test of E1M1 sectors 0 and 1)
                    // TODO: Check that all the verts fall inside the sector, not on the edges
                   // verts2D = Triangulator.Triangulator.CutHoleInShape(verts2D, holeVerts2D.Reverse().ToArray());
                }

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
                    flatVertices[i] = new Vector3(floorVerts[i].x * Scale, Data.Sectors[sectorNum].FloorHeight * Scale, floorVerts[i].y * Scale);
                    flatVertices[i + floorVerts.Length] = new Vector3(ceilVerts[i].x * Scale, Data.Sectors[sectorNum].CeilingHeight * Scale, ceilVerts[i].y * Scale);

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
                fm.SetColor("_Color", new Color32(brightness, brightness, brightness, 255));
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
