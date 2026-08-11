using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityWAD
{
    // Compact C# port of mapbox/earcut (ISC licence). Triangulates a polygon with holes.
    // Input: a flat list of vertices, plus the start index of each hole. The outer ring
    // occupies [0, firstHoleStart). Robust to collinear runs and the coincident vertices
    // that hole bridges create - which is exactly what defeated the old ear clipper.
    // z-order hashing from the original is omitted; DOOM sectors are small enough not to need it.
    public static class Earcut
    {
        class Node
        {
            public int i;           // vertex index in the source list
            public float x, y;
            public Node prev, next;
            public bool steiner;
            public Node(int i, float x, float y) { this.i = i; this.x = x; this.y = y; }
        }

        // Returns triangle indices into `verts`.
        public static List<int> Triangulate(IList<Vector2> verts, IList<int> holeIndices)
        {
            int n = verts.Count;
            bool hasHoles = holeIndices != null && holeIndices.Count > 0;
            int outerLen = hasHoles ? holeIndices[0] : n;

            var triangles = new List<int>();
            Node outerNode = LinkedList(verts, 0, outerLen, true);
            if (outerNode == null || outerNode.next == outerNode.prev) return triangles;

            if (hasHoles) outerNode = EliminateHoles(verts, holeIndices, outerNode, n);

            EarcutLinked(outerNode, triangles, 0);
            return triangles;
        }

        // Build a circular doubly linked list from a ring of vertices, forcing a winding direction.
        static Node LinkedList(IList<Vector2> verts, int start, int end, bool clockwise)
        {
            Node last = null;
            double area = 0;
            for (int i = start, j = end - 1; i < end; j = i++)
                area += (double)(verts[j].x - verts[i].x) * (verts[i].y + verts[j].y);

            // clockwise==true means we want a positive-signed ring (outer, CCW output order in this port)
            if (clockwise == (area > 0))
                for (int i = start; i < end; i++) last = InsertNode(i, verts[i], last);
            else
                for (int i = end - 1; i >= start; i--) last = InsertNode(i, verts[i], last);

            if (last != null && Equals(last, last.next))
            {
                RemoveNode(last);
                last = last.next;
            }
            return last;
        }

        static Node InsertNode(int i, Vector2 p, Node last)
        {
            var node = new Node(i, p.x, p.y);
            if (last == null) { node.prev = node; node.next = node; }
            else { node.next = last.next; node.prev = last; last.next.prev = node; last.next = node; }
            return node;
        }

        static void RemoveNode(Node n)
        {
            n.next.prev = n.prev;
            n.prev.next = n.next;
        }

        static Node FilterPoints(Node start, Node end)
        {
            if (start == null) return start;
            if (end == null) end = start;
            Node p = start;
            bool again;
            do
            {
                again = false;
                if (!p.steiner && (Equals(p, p.next) || Area(p.prev, p, p.next) == 0))
                {
                    RemoveNode(p);
                    p = end = p.prev;
                    if (p == p.next) break;
                    again = true;
                }
                else p = p.next;
            } while (again || p != end);
            return end;
        }

        static void EarcutLinked(Node ear, List<int> triangles, int pass)
        {
            if (ear == null) return;
            ear = FilterPoints(ear, null);

            Node stop = ear;
            while (ear.prev != ear.next)
            {
                Node prev = ear.prev, next = ear.next;
                if (IsEar(ear))
                {
                    triangles.Add(prev.i); triangles.Add(ear.i); triangles.Add(next.i);
                    RemoveNode(ear);
                    ear = next.next;
                    stop = next.next;
                    continue;
                }
                ear = next;
                if (ear == stop)
                {
                    // couldn't find any ear this cycle; escalate through the fallback passes,
                    // exactly as mapbox earcut: (0) filter+retry, (1) cure local self-intersections,
                    // (2) split the polygon at a valid diagonal and recurse.
                    if (pass == 0) EarcutLinked(FilterPoints(ear, null), triangles, 1);
                    else if (pass == 1) { ear = CureLocalIntersections(FilterPoints(ear, null), triangles); EarcutLinked(ear, triangles, 2); }
                    else if (pass == 2) SplitEarcut(ear, triangles);
                    break;
                }
            }
        }

        static bool IsEar(Node ear)
        {
            Node a = ear.prev, b = ear, c = ear.next;
            if (Area(a, b, c) >= 0) return false; // reflex or collinear

            Node p = ear.next.next;
            while (p != ear.prev)
            {
                if (PointInTriangle(a.x, a.y, b.x, b.y, c.x, c.y, p.x, p.y) &&
                    Area(p.prev, p, p.next) >= 0)
                    return false;
                p = p.next;
            }
            return true;
        }

        static Node CureLocalIntersections(Node start, List<int> triangles)
        {
            Node p = start;
            do
            {
                Node a = p.prev, b = p.next.next;
                if (!Equals(a, b) && Intersects(a, p, p.next, b) && LocallyInside(a, b) && LocallyInside(b, a))
                {
                    triangles.Add(a.i); triangles.Add(p.i); triangles.Add(b.i);
                    RemoveNode(p);
                    RemoveNode(p.next);
                    p = start = b;
                }
                p = p.next;
            } while (p != start);
            return FilterPoints(p, null);
        }

        static void SplitEarcut(Node start, List<int> triangles)
        {
            Node a = start;
            do
            {
                Node b = a.next.next;
                while (b != a.prev)
                {
                    if (a.i != b.i && IsValidDiagonal(a, b))
                    {
                        Node c = SplitPolygon(a, b);
                        a = FilterPoints(a, a.next);
                        c = FilterPoints(c, c.next);
                        EarcutLinked(a, triangles, 0);
                        EarcutLinked(c, triangles, 0);
                        return;
                    }
                    b = b.next;
                }
                a = a.next;
            } while (a != start);
        }

        static Node EliminateHoles(IList<Vector2> verts, IList<int> holeIndices, Node outerNode, int n)
        {
            var queue = new List<Node>();
            for (int i = 0; i < holeIndices.Count; i++)
            {
                int start = holeIndices[i];
                int end = i < holeIndices.Count - 1 ? holeIndices[i + 1] : n;
                Node list = LinkedList(verts, start, end, false);
                if (list == list.next) list.steiner = true;
                queue.Add(GetLeftmost(list));
            }
            queue.Sort((a, b) => a.x.CompareTo(b.x));
            foreach (var h in queue)
                outerNode = EliminateHole(h, outerNode);
            return outerNode;
        }

        static Node EliminateHole(Node hole, Node outerNode)
        {
            Node bridge = FindHoleBridge(hole, outerNode);
            if (bridge == null) return outerNode;
            Node bridgeReverse = SplitPolygon(bridge, hole);
            FilterPoints(bridgeReverse, bridgeReverse.next);
            return FilterPoints(bridge, bridge.next);
        }

        static Node FindHoleBridge(Node hole, Node outerNode)
        {
            Node p = outerNode;
            float hx = hole.x, hy = hole.y;
            float qx = float.NegativeInfinity;
            Node m = null;

            // find the edge whose intersection with the +x ray from the hole is closest to the hole
            do
            {
                if (hy <= p.y && hy >= p.next.y && p.next.y != p.y)
                {
                    float x = p.x + (hy - p.y) / (p.next.y - p.y) * (p.next.x - p.x);
                    if (x <= hx && x > qx)
                    {
                        qx = x;
                        m = p.x < p.next.x ? p : p.next;
                        if (x == hx) return m;
                    }
                }
                p = p.next;
            } while (p != outerNode);

            if (m == null) return null;

            // look for points strictly inside the triangle (hole, edge intersection, m) - pick the best
            Node stop = m;
            float mx = m.x, my = m.y;
            float tanMin = float.PositiveInfinity;
            p = m;
            do
            {
                if (hx >= p.x && p.x >= mx && hx != p.x &&
                    PointInTriangle(hy < my ? hx : qx, hy, mx, my, hy < my ? qx : hx, hy, p.x, p.y))
                {
                    float tan = Math.Abs(hy - p.y) / (hx - p.x);
                    if (LocallyInside(p, hole) && (tan < tanMin || (tan == tanMin && (p.x > m.x || (p.x == m.x && SectorContainsSector(m, p))))))
                    {
                        m = p; tanMin = tan;
                    }
                }
                p = p.next;
            } while (p != stop);

            return m;
        }

        static bool SectorContainsSector(Node m, Node p)
            => Area(m.prev, m, p.prev) < 0 && Area(p.next, m, m.next) < 0;

        static Node GetLeftmost(Node start)
        {
            Node p = start, leftmost = start;
            do
            {
                if (p.x < leftmost.x || (p.x == leftmost.x && p.y < leftmost.y)) leftmost = p;
                p = p.next;
            } while (p != start);
            return leftmost;
        }

        static bool PointInTriangle(float ax, float ay, float bx, float by, float cx, float cy, float px, float py)
            => (cx - px) * (ay - py) - (ax - px) * (cy - py) >= 0 &&
               (ax - px) * (by - py) - (bx - px) * (ay - py) >= 0 &&
               (bx - px) * (cy - py) - (cx - px) * (by - py) >= 0;

        static bool IsValidDiagonal(Node a, Node b)
            => a.next.i != b.i && a.prev.i != b.i && !IntersectsPolygon(a, b) &&
               ((LocallyInside(a, b) && LocallyInside(b, a) && MiddleInside(a, b) &&
                 (Area(a.prev, a, b.prev) != 0 || Area(a, b.prev, b) != 0)) ||
                (Equals(a, b) && Area(a.prev, a, a.next) > 0 && Area(b.prev, b, b.next) > 0));

        static float Area(Node p, Node q, Node r)
            => (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);

        static bool Equals(Node p1, Node p2) => p1.x == p2.x && p1.y == p2.y;

        static bool Intersects(Node p1, Node q1, Node p2, Node q2)
        {
            int o1 = Sign(Area(p1, q1, p2)), o2 = Sign(Area(p1, q1, q2));
            int o3 = Sign(Area(p2, q2, p1)), o4 = Sign(Area(p2, q2, q1));
            if (o1 != o2 && o3 != o4) return true;
            if (o1 == 0 && OnSegment(p1, p2, q1)) return true;
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true;
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true;
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true;
            return false;
        }

        static int Sign(float n) => n > 0 ? 1 : (n < 0 ? -1 : 0);
        static bool OnSegment(Node p, Node q, Node r)
            => q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) &&
               q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y);

        static bool IntersectsPolygon(Node a, Node b)
        {
            Node p = a;
            do
            {
                if (p.i != a.i && p.next.i != a.i && p.i != b.i && p.next.i != b.i &&
                    Intersects(p, p.next, a, b)) return true;
                p = p.next;
            } while (p != a);
            return false;
        }

        static bool LocallyInside(Node a, Node b)
            => Area(a.prev, a, a.next) < 0
                ? Area(a, b, a.next) >= 0 && Area(a, a.prev, b) >= 0
                : Area(a, b, a.prev) < 0 || Area(a, a.next, b) < 0;

        static bool MiddleInside(Node a, Node b)
        {
            Node p = a;
            bool inside = false;
            float px = (a.x + b.x) / 2, py = (a.y + b.y) / 2;
            do
            {
                if (((p.y > py) != (p.next.y > py)) && p.next.y != p.y &&
                    (px < (p.next.x - p.x) * (py - p.y) / (p.next.y - p.y) + p.x))
                    inside = !inside;
                p = p.next;
            } while (p != a);
            return inside;
        }

        static Node SplitPolygon(Node a, Node b)
        {
            var a2 = new Node(a.i, a.x, a.y);
            var b2 = new Node(b.i, b.x, b.y);
            Node an = a.next, bp = b.prev;
            a.next = b; b.prev = a;
            a2.next = an; an.prev = a2;
            b2.next = a2; a2.prev = b2;
            bp.next = b2; b2.prev = bp;
            return b2;
        }
    }
}
