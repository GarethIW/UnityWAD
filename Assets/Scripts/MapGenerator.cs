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
        public Vector2 Grid = new Vector2(64f,64f); // Line length of one map grid unit, used to calculate uv scaling

        public float Scale = 2f;

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
            var localScale = (1f / Grid.x) * Scale;

            // Dictionaries of lists. We're building by sector so we'll index by sector
            var wallVertices = new Dictionary<int,VertexList>();    // Vertices are per-face because we're tilemapping so we can't share them
            var wallUVs = new Dictionary<int, List<Vector4>>();     // UV0: (uv X,uv Y,tile index (index of wall texture in texture tilesheet),0)
            var wallUV1s = new Dictionary<int, List<Vector4>>();    // UV1: (tile width,tile height (dimensions of wall sprite *within* the tile indexed in uv0),X offset,Y offset (offsets as defined in SideDefs))
            var wallIndices = new Dictionary<int, List<int>>();

            for (var i = 0; i < Data.Sectors.Length; i++)
            {
                wallVertices.Add(i, new VertexList());
                wallUVs.Add(i, new List<Vector4>());
                wallUV1s.Add(i, new List<Vector4>());
                wallIndices.Add(i, new List<int>());
            }

            // Classify which sectors are doors/lifts so their geometry can be pulled out of the
            // static mesh into their own movable GameObjects (see the finalisation pass below).
            var movingSectors = Movers.ResolveMovingSectors(Data);
            bool Separated(MoverKind k) => k == MoverKind.Door || k == MoverKind.LockedDoor || k == MoverKind.Lift;

            // Per-mover-sector accumulators. "Face" geometry is wall-type (a door's face / a lift's
            // side skin, DoomWalls material); "Cap" geometry is flat-type (a door's underside ceiling
            // / a lift's floor, DoomFlats material).
            var moverFaceV = new Dictionary<int, VertexList>();
            var moverFaceUV0 = new Dictionary<int, List<Vector4>>();
            var moverFaceUV1 = new Dictionary<int, List<Vector4>>();
            var moverFaceI = new Dictionary<int, List<int>>();
            var moverCapV = new Dictionary<int, List<Vector3>>();
            var moverCapUV = new Dictionary<int, List<Vector4>>();
            var moverCapI = new Dictionary<int, List<int>>();
            void EnsureMover(int s)
            {
                if (moverFaceV.ContainsKey(s)) return;
                moverFaceV[s] = new VertexList(); moverFaceUV0[s] = new List<Vector4>();
                moverFaceUV1[s] = new List<Vector4>(); moverFaceI[s] = new List<int>();
                moverCapV[s] = new List<Vector3>(); moverCapUV[s] = new List<Vector4>();
                moverCapI[s] = new List<int>();
            }

            // Emits one wall quad (four per-face vertices) with DOOM-accurate texel UVs into the
            // supplied target lists.
            //   uv0 = (horizontalTexel, verticalTexel, tileIndex, 0)  - absolute DOOM texels
            //   uv1 = (textureWidth, textureHeight, 0, 0)             - source size within the atlas cell
            // Horizontal texels run from the anchor vertex (v1 for front sides, v2 for back sides)
            // plus the sidedef X offset. Vertical texels are pegged per the DOOM rules:
            //   pegRefIsTop  -> the texture's TOP edge sits at pegRefZ
            //   !pegRefIsTop -> the texture's BOTTOM edge sits at pegRefZ
            // with the sidedef Y offset added on top. The shader tiles with frac(), so these are
            // absolute (un-wrapped) texel coordinates - which is what makes stacked wall sections
            // line up exactly as they do in DOOM.
            void AddWallSection(VertexList vl, List<Vector4> uv0, List<Vector4> uv1, List<int> inds,
                                Vector2 from2D, Vector2 to2D, float topZ, float botZ,
                                string texName, int xOff, int yOff, float pegRefZ, bool pegRefIsTop, bool frontSide)
            {
                if (!WallTiles.LookupTable.ContainsKey(texName)) return; // texture not in atlas -> skip
                var tile = WallTiles.LookupTable[texName];
                int tileNum = tile.TileNum;
                float tw = tile.Width, th = tile.Height;

                float wallLen = Vector2.Distance(from2D, to2D);
                float tuFrom = frontSide ? xOff : xOff + wallLen;
                float tuTo = frontSide ? xOff + wallLen : xOff;

                // World Z of the texture's bottom edge (texel 0), then vertical texels up from it.
                float pegBottomZ = (pegRefIsTop ? pegRefZ - th : pegRefZ) + yOff;
                float tvTop = topZ - pegBottomZ;
                float tvBot = botZ - pegBottomZ;

                int tl = vl.Add(new Vector3(from2D.x, topZ, from2D.y) * localScale);
                int tr = vl.Add(new Vector3(to2D.x, topZ, to2D.y) * localScale);
                int bl = vl.Add(new Vector3(from2D.x, botZ, from2D.y) * localScale);
                int br = vl.Add(new Vector3(to2D.x, botZ, to2D.y) * localScale);

                if (frontSide) inds.AddRange(new[] { tr, br, tl, br, bl, tl });
                else inds.AddRange(new[] { tl, bl, br, tl, br, tr });

                uv0.Add(new Vector4(tuFrom, tvTop, tileNum, 0f));
                uv0.Add(new Vector4(tuTo, tvTop, tileNum, 0f));
                uv0.Add(new Vector4(tuFrom, tvBot, tileNum, 0f));
                uv0.Add(new Vector4(tuTo, tvBot, tileNum, 0f));
                for (int i = 0; i < 4; i++) uv1.Add(new Vector4(tw, th, 0f, 0f));
            }

            // Appends a floor/ceiling surface (triangulated flat) into the supplied lists at the
            // given height. faceUp reverses the winding so a floor's normal points up (+Y).
            void AddFlatSurface(List<Vector3> verts, List<Vector4> uvs, List<int> inds,
                                FlatGeometry.SectorFlat flat, float height, int tileNum, bool faceUp)
            {
                if (tileNum == -1) return;
                foreach (var region in flat.Regions)
                {
                    int b = verts.Count;
                    foreach (var p in region.Verts)
                    {
                        verts.Add(new Vector3(p.x, height, p.y) * localScale);
                        uvs.Add(new Vector4(0, 0, tileNum, 0));
                    }
                    for (int i = 0; i < region.Tris.Length; i += 3)
                    {
                        if (faceUp) { inds.Add(b + region.Tris[i + 2]); inds.Add(b + region.Tris[i + 1]); inds.Add(b + region.Tris[i]); }
                        else { inds.Add(b + region.Tris[i]); inds.Add(b + region.Tris[i + 1]); inds.Add(b + region.Tris[i + 2]); }
                    }
                }
            }

            // Routes a wall face to the static sector mesh - or, when it is the skin of an adjacent
            // door (an upper wall) or lift (a lower wall), into that mover's own geometry so it can
            // be separated out and moved.
            void EmitWall(int ownSector, int neighbourSector, char part, Vector2 from2D, Vector2 to2D,
                          float topZ, float botZ, string tex, int xOff, int yOff, float pegRef,
                          bool pegRefIsTop, bool frontSide)
            {
                int moverS = -1;
                if (neighbourSector >= 0 && movingSectors.TryGetValue(neighbourSector, out var nk))
                {
                    if (part == 'U' && (nk == MoverKind.Door || nk == MoverKind.LockedDoor)) moverS = neighbourSector;
                    else if (part == 'L' && nk == MoverKind.Lift) moverS = neighbourSector;
                }

                if (moverS >= 0)
                {
                    EnsureMover(moverS);
                    AddWallSection(moverFaceV[moverS], moverFaceUV0[moverS], moverFaceUV1[moverS], moverFaceI[moverS],
                                   from2D, to2D, topZ, botZ, tex, xOff, yOff, pegRef, pegRefIsTop, frontSide);
                }
                else
                {
                    AddWallSection(wallVertices[ownSector], wallUVs[ownSector], wallUV1s[ownSector], wallIndices[ownSector],
                                   from2D, to2D, topZ, botZ, tex, xOff, yOff, pegRef, pegRefIsTop, frontSide);
                }
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

                int rightNeighbour = leftSide != null ? leftSide.Sector : -1;
                int leftNeighbour = rightSide.Sector;

                var from2D = new Vector2(Data.Vertexes[line.From].X, Data.Vertexes[line.From].Y);
                var to2D = new Vector2(Data.Vertexes[line.To].X, Data.Vertexes[line.To].Y);

                // DOOM linedef flags: bit 3 = upper unpegged (ML_DONTPEGTOP),
                // bit 4 = lower unpegged (ML_DONTPEGBOTTOM). These pick where each wall texture is
                // pegged vertically and were previously ignored entirely - the single biggest reason
                // wall textures never matched DOOM.
                bool pegTop = line.Attributes[3];
                bool pegBottom = line.Attributes[4];

                // --- Right (front) side ---
                // Full wall (one-sided line): ceiling down to floor.
                if (!rightSide.FullTexture.StartsWith("-"))
                {
                    float topZ = rightSector.CeilingHeight, botZ = rightSector.FloorHeight;
                    // Middle texture: default pegs the top to the ceiling; lower-unpegged pegs the
                    // bottom to the floor.
                    float pegRef = pegBottom ? botZ : topZ;
                    EmitWall(rightSide.Sector, rightNeighbour, 'F', from2D, to2D, topZ, botZ, rightSide.FullTexture,
                             rightSide.XOffset, rightSide.YOffset, pegRef, !pegBottom, true);
                }
                // Upper wall: this ceiling down to the neighbour's (lower) ceiling.
                if (!rightSide.UpperTexture.StartsWith("-") && leftSector != null && !leftSector.CeilingTexture.StartsWith("F_SKY"))
                {
                    float topZ = rightSector.CeilingHeight, botZ = leftSector.CeilingHeight;
                    if (topZ - botZ > 0f)
                    {
                        // Default pegs the bottom to the back ceiling; upper-unpegged pegs the top
                        // to this ceiling.
                        float pegRef = pegTop ? topZ : botZ;
                        EmitWall(rightSide.Sector, rightNeighbour, 'U', from2D, to2D, topZ, botZ, rightSide.UpperTexture,
                                 rightSide.XOffset, rightSide.YOffset, pegRef, pegTop, true);
                    }
                }
                // Lower wall: neighbour's (higher) floor down to this floor.
                if (!rightSide.LowerTexture.StartsWith("-") && leftSector != null)
                {
                    float topZ = leftSector.FloorHeight, botZ = rightSector.FloorHeight;
                    if (topZ - botZ > 0f)
                    {
                        // Default pegs the top to the back floor; lower-unpegged pegs the top to
                        // this sector's ceiling (so it aligns with the wall above, e.g. lifts).
                        float pegRef = pegBottom ? (float)rightSector.CeilingHeight : topZ;
                        EmitWall(rightSide.Sector, rightNeighbour, 'L', from2D, to2D, topZ, botZ, rightSide.LowerTexture,
                                 rightSide.XOffset, rightSide.YOffset, pegRef, true, true);
                    }
                }

                // --- Left (back) side --- (same rules, sectors swapped, reversed winding)
                if (leftSide != null)
                {
                    if (!leftSide.FullTexture.StartsWith("-"))
                    {
                        float topZ = leftSector.CeilingHeight, botZ = leftSector.FloorHeight;
                        float pegRef = pegBottom ? botZ : topZ;
                        EmitWall(leftSide.Sector, leftNeighbour, 'F', from2D, to2D, topZ, botZ, leftSide.FullTexture,
                                 leftSide.XOffset, leftSide.YOffset, pegRef, !pegBottom, false);
                    }
                    if (!leftSide.UpperTexture.StartsWith("-") && !rightSector.CeilingTexture.StartsWith("F_SKY"))
                    {
                        float topZ = leftSector.CeilingHeight, botZ = rightSector.CeilingHeight;
                        if (topZ - botZ > 0f)
                        {
                            float pegRef = pegTop ? topZ : botZ;
                            EmitWall(leftSide.Sector, leftNeighbour, 'U', from2D, to2D, topZ, botZ, leftSide.UpperTexture,
                                     leftSide.XOffset, leftSide.YOffset, pegRef, pegTop, false);
                        }
                    }
                    if (!leftSide.LowerTexture.StartsWith("-"))
                    {
                        float topZ = rightSector.FloorHeight, botZ = leftSector.FloorHeight;
                        if (topZ - botZ > 0f)
                        {
                            float pegRef = pegBottom ? (float)leftSector.CeilingHeight : topZ;
                            EmitWall(leftSide.Sector, leftNeighbour, 'L', from2D, to2D, topZ, botZ, leftSide.LowerTexture,
                                     leftSide.XOffset, leftSide.YOffset, pegRef, true, false);
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

                // See if we have a tile index for the flat textures
                var floorTileNum = -1;
                if (FlatTiles.LookupTable.ContainsKey(Data.Sectors[sectorNum].FloorTexture.Replace("\0", ""))) floorTileNum = FlatTiles.LookupTable[Data.Sectors[sectorNum].FloorTexture.Replace("\0", "")].TileNum;
                var ceilingTileNum = -1;
                if (FlatTiles.LookupTable.ContainsKey(Data.Sectors[sectorNum].CeilingTexture.Replace("\0", ""))) ceilingTileNum = FlatTiles.LookupTable[Data.Sectors[sectorNum].CeilingTexture.Replace("\0", "")].TileNum;

                // We don't want to render geometry with sky texture (we want to see through to a potential skybox)
                if (Data.Sectors[sectorNum].FloorTexture.StartsWith("F_SKY")) floorTileNum = -1;
                if (Data.Sectors[sectorNum].CeilingTexture.StartsWith("F_SKY")) ceilingTileNum = -1;

                // Rebuild the floor/ceiling polygons for this sector - tracing the boundary loops,
                // classifying outer boundaries vs holes (islands), and triangulating with the holes
                // cut out. See FlatGeometry for the details.
                var flat = FlatGeometry.Build(Data, sectorNum);
                if (flat.Outers.Count == 0) continue; // nothing enclosed -> no flat to build

                var floorH = Data.Sectors[sectorNum].FloorHeight;
                var ceilH = Data.Sectors[sectorNum].CeilingHeight;

                movingSectors.TryGetValue(sectorNum, out var sectorKind);
                bool sectorIsDoor = sectorKind == MoverKind.Door || sectorKind == MoverKind.LockedDoor;
                bool sectorIsLift = sectorKind == MoverKind.Lift;

                var flatVertices = new List<Vector3>();
                var flatUVs = new List<Vector4>();
                var flatIndices = new List<int>();

                // Floor moves with a lift (its top surface); otherwise it stays in the static mesh.
                if (sectorIsLift)
                {
                    EnsureMover(sectorNum);
                    AddFlatSurface(moverCapV[sectorNum], moverCapUV[sectorNum], moverCapI[sectorNum], flat, floorH, floorTileNum, true);
                }
                else
                {
                    AddFlatSurface(flatVertices, flatUVs, flatIndices, flat, floorH, floorTileNum, true);
                }

                // Ceiling moves with a door (its underside); otherwise it stays in the static mesh.
                if (sectorIsDoor)
                {
                    EnsureMover(sectorNum);
                    AddFlatSurface(moverCapV[sectorNum], moverCapUV[sectorNum], moverCapI[sectorNum], flat, ceilH, ceilingTileNum, false);
                }
                else
                {
                    AddFlatSurface(flatVertices, flatUVs, flatIndices, flat, ceilH, ceilingTileNum, false);
                }

                if (flatVertices.Count > 0)
                {
                    var flatsMesh = new Mesh();
                    if (flatVertices.Count > 65000) flatsMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    flatsMesh.SetVertices(flatVertices);
                    flatsMesh.SetIndices(flatIndices.ToArray(), MeshTopology.Triangles, 0);
                    flatsMesh.SetUVs(0, flatUVs);
                    flatsMesh.RecalculateNormals();
                    flatsMesh.RecalculateBounds();

                    var floor = InstantiateChild(sectorContainer.transform, "Flats");
                    floor.GetComponent<MeshFilter>().mesh = flatsMesh;
                    var fm = Instantiate(FlatsMaterial);
                    fm.SetColor("_Color", new Color32(brightness, brightness, brightness, 255));
                    floor.GetComponent<MeshRenderer>().materials = new[] { fm };
                }
            }

            // Builds a mesh child (offset so its vertices are local to the mover's pivot) and gives
            // it the given base material tinted by sector brightness.
            void BuildMoverChild(Transform parent, string name, List<Vector3> verts, List<Vector4> uv0,
                                 List<Vector4> uv1, List<int> inds, Vector3 offset, Material baseMat, byte brightness)
            {
                var vs = new Vector3[verts.Count];
                for (int i = 0; i < verts.Count; i++) vs[i] = verts[i] - offset;

                var mesh = new Mesh();
                if (vs.Length > 65000) mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                mesh.vertices = vs;
                mesh.SetIndices(inds.ToArray(), MeshTopology.Triangles, 0);
                mesh.SetUVs(0, uv0);
                if (uv1 != null) mesh.SetUVs(1, uv1);
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                var go = InstantiateChild(parent, name);
                go.GetComponent<MeshFilter>().mesh = mesh;
                var mat = Instantiate(baseMat);
                mat.SetColor("_Color", new Color32(brightness, brightness, brightness, 255));
                go.GetComponent<MeshRenderer>().materials = new[] { mat };
            }

            // Finalise: build the separated door/lift GameObjects. Each is positioned at its own
            // pivot (footprint centre, at the sector floor) so it can later be animated simply by
            // translating its Transform - see DoomMover.OpenOffset. No controller/animation is added.
            Transform moversRoot = null;
            foreach (var kv in movingSectors)
            {
                int s = kv.Key;
                var mk = kv.Value;
                if (!Separated(mk) || !moverFaceV.ContainsKey(s)) continue;

                var faceVerts = moverFaceV[s].Vertices;
                var capVerts = moverCapV[s];
                bool hasFace = faceVerts.Count > 0;
                bool hasCap = capVerts.Count > 0;
                if (!hasFace && !hasCap) continue;

                float minX = float.MaxValue, maxX = float.MinValue, minZ = float.MaxValue, maxZ = float.MinValue;
                void Extend(List<Vector3> vs)
                {
                    foreach (var v in vs)
                    {
                        if (v.x < minX) minX = v.x;
                        if (v.x > maxX) maxX = v.x;
                        if (v.z < minZ) minZ = v.z;
                        if (v.z > maxZ) maxZ = v.z;
                    }
                }
                Extend(faceVerts); Extend(capVerts);
                var pivot = new Vector3((minX + maxX) * 0.5f, Data.Sectors[s].FloorHeight * localScale, (minZ + maxZ) * 0.5f);

                if (moversRoot == null)
                {
                    var rootGO = new GameObject("Movers");
                    rootGO.transform.SetParent(transform);
                    rootGO.transform.localPosition = Vector3.zero;
                    moversRoot = rootGO.transform;
                }

                var moverBrightness = (byte)Data.Sectors[s].Brightness;
                var moverGO = new GameObject((mk == MoverKind.Lift ? "Lift " : "Door ") + s);
                moverGO.transform.SetParent(moversRoot);
                moverGO.transform.localPosition = pivot;

                if (hasFace)
                    BuildMoverChild(moverGO.transform, "Faces", faceVerts, moverFaceUV0[s], moverFaceUV1[s], moverFaceI[s], pivot, WallsMaterial, moverBrightness);
                if (hasCap)
                    BuildMoverChild(moverGO.transform, "Cap", capVerts, moverCapUV[s], null, moverCapI[s], pivot, FlatsMaterial, moverBrightness);

                var mover = moverGO.AddComponent<DoomMover>();
                mover.Kind = mk;
                mover.SectorIndex = s;
                if (mk == MoverKind.Lift)
                {
                    mover.ClosedHeight = Data.Sectors[s].FloorHeight;
                    mover.OpenHeight = Movers.LowestNeighbourFloor(Data, s, Data.Sectors[s].FloorHeight);
                }
                else
                {
                    mover.ClosedHeight = Data.Sectors[s].CeilingHeight;
                    mover.OpenHeight = Movers.LowestNeighbourCeiling(Data, s, Data.Sectors[s].CeilingHeight) - 4;
                }
                mover.OpenOffset = new Vector3(0f, (mover.OpenHeight - mover.ClosedHeight) * localScale, 0f);
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
