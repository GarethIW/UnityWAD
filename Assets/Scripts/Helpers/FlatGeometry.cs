using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityWAD
{
    // Reconstructs the floor/ceiling polygons ("flats") for a sector from its linedefs.
    //
    // DOOM stores no polygon data for flats - the original engine flood-filled them via BSP.
    // So we rebuild them: gather every linedef that bounds the sector, trace them into closed
    // loops, work out which loops are the outer boundary and which are holes (islands), then
    // triangulate with the holes cut out.
    //
    // This replaces the old approach (single-loop outline walk + "left sidedef == hole" guess +
    // the disabled CutHoleInShape), which never handled holes and mis-triangulated collinear-
    // heavy geometry. Verified against every sector of DOOM.WAD and DOOM2.WAD.
    public static class FlatGeometry
    {
        public class Loop
        {
            public List<int> VertIdx = new List<int>();  // map vertex indices, in order
            public List<Vector2> Pts = new List<Vector2>();
            public float SignedArea;                       // >0 CCW (outer), <0 CW (hole)
            public bool IsHole { get { return SignedArea < 0f; } }
        }

        public class SectorFlat
        {
            public List<Loop> Outers = new List<Loop>();
            public List<Loop> Holes = new List<Loop>();
            // One triangulated region per outer loop: (vertices in map-space, triangle indices).
            public List<Region> Regions = new List<Region>();
        }

        public struct Region
        {
            public Vector2[] Verts;
            public int[] Tris;
            public Region(Vector2[] v, int[] t) { Verts = v; Tris = t; }
        }

        public static float SignedArea(IList<Vector2> p)
        {
            double a = 0;
            for (int i = 0; i < p.Count; i++)
            {
                Vector2 c = p[i], n = p[(i + 1) % p.Count];
                a += (double)c.x * n.y - (double)n.x * c.y;
            }
            return (float)(a * 0.5);
        }

        static bool PointInPoly(Vector2 pt, IList<Vector2> poly)
        {
            bool inside = false;
            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                if (((poly[i].y > pt.y) != (poly[j].y > pt.y)) &&
                    (pt.x < (poly[j].x - poly[i].x) * (pt.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x))
                    inside = !inside;
            }
            return inside;
        }

        // Trace all boundary loops of a sector using directed half-edges.
        // For each linedef with the sector on exactly one side, emit a directed edge oriented so
        // the sector interior lies on the LEFT of travel. Then each vertex has a well-defined
        // outgoing edge, so tracing is unambiguous - even where loops meet at a shared vertex.
        public static List<Loop> TraceLoops(MapData data, int sectorNum)
        {
            var outgoing = new Dictionary<int, List<int>>();
            Action<int, int> addEdge = (a, b) =>
            {
                List<int> l;
                if (!outgoing.TryGetValue(a, out l)) { l = new List<int>(); outgoing[a] = l; }
                l.Add(b);
            };

            foreach (var ln in data.LineDefs)
            {
                int rs = ln.Right != -1 ? data.SideDefs[ln.Right].Sector : -1;
                int ls = ln.Left != -1 ? data.SideDefs[ln.Left].Sector : -1;
                bool right = rs == sectorNum;
                bool left = ls == sectorNum;
                if (right == left) continue; // neither side, or both (self-referencing) -> not a boundary

                // DOOM: the right sidedef's sector sits on the RIGHT of From->To. Interior-on-left
                // therefore means: right-side sector -> travel To->From; left-side sector -> From->To.
                if (right) addEdge(ln.To, ln.From);
                else addEdge(ln.From, ln.To);
            }

            Func<int, Vector2> P = vi => new Vector2(data.Vertexes[vi].X, data.Vertexes[vi].Y);

            var loops = new List<Loop>();
            while (outgoing.Count > 0)
            {
                int start = outgoing.Keys.First();
                var loop = new Loop();
                int cur = start;
                int prev = -1;
                int guard = 0, maxGuard = data.LineDefs.Length * 4 + 16;
                while (true)
                {
                    List<int> nexts;
                    if (!outgoing.TryGetValue(cur, out nexts) || nexts.Count == 0) break;

                    int pick;
                    if (prev == -1 || nexts.Count == 1)
                    {
                        pick = 0;
                    }
                    else
                    {
                        // Pinch point: several boundary edges leave this vertex. Keep the interior
                        // on the left by taking the outgoing edge that is the most clockwise turn
                        // from the reversed incoming edge (standard planar face traversal). This
                        // traces a pinched boundary as ONE weakly-simple ring rather than splitting
                        // it into two loops that share a vertex (which would drop a hole).
                        Vector2 backDir = P(prev) - P(cur);
                        float backAng = Mathf.Atan2(backDir.y, backDir.x);
                        pick = 0; float best = float.MaxValue;
                        for (int k = 0; k < nexts.Count; k++)
                        {
                            Vector2 outDir = P(nexts[k]) - P(cur);
                            float a = backAng - Mathf.Atan2(outDir.y, outDir.x); // clockwise sweep
                            while (a <= 0) a += Mathf.PI * 2f;
                            while (a > Mathf.PI * 2f) a -= Mathf.PI * 2f;
                            if (a < best) { best = a; pick = k; }
                        }
                    }

                    int nxt = nexts[pick];
                    nexts.RemoveAt(pick);
                    if (nexts.Count == 0) outgoing.Remove(cur);

                    loop.VertIdx.Add(cur);
                    prev = cur;
                    cur = nxt;
                    if (cur == start) break;
                    if (++guard > maxGuard) break; // safety
                }
                if (loop.VertIdx.Count >= 3)
                {
                    foreach (int vi in loop.VertIdx) loop.Pts.Add(P(vi));
                    loop.SignedArea = SignedArea(loop.Pts);
                    loops.Add(loop);
                }
            }
            return loops;
        }

        public static SectorFlat Build(MapData data, int sectorNum)
        {
            var flat = new SectorFlat();
            var loops = TraceLoops(data, sectorNum);
            foreach (var lp in loops)
                if (lp.IsHole) flat.Holes.Add(lp); else flat.Outers.Add(lp);

            // Assign each hole to the smallest-area outer loop that contains it (handles a sector
            // made of several disjoint filled regions, each possibly with its own islands).
            var holesByOuter = new Dictionary<Loop, List<Loop>>();
            foreach (var o in flat.Outers) holesByOuter[o] = new List<Loop>();
            foreach (var h in flat.Holes)
            {
                Vector2 probe = h.Pts[0];
                Loop best = null; float bestArea = float.MaxValue;
                foreach (var o in flat.Outers)
                {
                    if (PointInPoly(probe, o.Pts))
                    {
                        float ar = Mathf.Abs(o.SignedArea);
                        if (ar < bestArea) { bestArea = ar; best = o; }
                    }
                }
                if (best != null) holesByOuter[best].Add(h);
            }

            foreach (var o in flat.Outers)
            {
                // Earcut input: outer ring followed by each hole ring, recording where each hole
                // starts. Earcut re-normalises winding, so passing our loops as-is is fine.
                var pts = new List<Vector2>(o.Pts);
                var holeStarts = new List<int>();
                foreach (var h in holesByOuter[o])
                {
                    holeStarts.Add(pts.Count);
                    pts.AddRange(h.Pts);
                }
                var idx = Earcut.Triangulate(pts, holeStarts);
                flat.Regions.Add(new Region(pts.ToArray(), idx.ToArray()));
            }

            return flat;
        }
    }
}
