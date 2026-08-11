using System.Collections.Generic;

namespace UnityWAD
{
    public enum MoverKind { None, Door, LockedDoor, Lift, Floor, Ceiling, Crusher, Stairs, Elevator, Donut }

    // Classifies DOOM linedef special types into the kind of sector movement they cause, and
    // resolves which sectors move. DOOM drives all movers from linedef specials + sector tags:
    //   - "manual" specials (local doors) operate the sector on the linedef's BACK side, tag ignored
    //   - "tagged" specials operate every sector whose tag matches the linedef tag
    // Only the movement-causing specials are listed; everything else resolves to MoverKind.None.
    public static class Movers
    {
        static readonly Dictionary<int, MoverKind> Kinds = new Dictionary<int, MoverKind>();
        static readonly HashSet<int> Manual = new HashSet<int>();

        static Movers()
        {
            void Add(MoverKind k, bool manual, params int[] types)
            {
                foreach (var t in types) { Kinds[t] = k; if (manual) Manual.Add(t); }
            }

            // Manual doors (operate the sector behind the used linedef)
            Add(MoverKind.Door, true, 1, 31, 46, 117, 118);
            Add(MoverKind.LockedDoor, true, 26, 27, 28, 32, 33, 34);

            // Remote / tagged doors
            Add(MoverKind.Door, false, 2, 3, 4, 16, 29, 42, 50, 61, 63, 75, 76, 86, 90, 103,
                                       105, 106, 107, 108, 111, 112, 113, 114, 115, 116, 133, 135, 137);

            // Lifts / platforms (floor drops, waits, returns)
            Add(MoverKind.Lift, false, 10, 21, 62, 88, 120, 121, 122, 123, 87, 53, 54, 89);

            // Floors (raise/lower to a target)
            Add(MoverKind.Floor, false, 5, 9, 18, 19, 22, 23, 24, 30, 36, 37, 38, 45, 55, 56, 58, 59,
                                        60, 64, 65, 69, 70, 71, 82, 83, 84, 91, 92, 93, 94, 96, 98, 101,
                                        102, 119, 128, 129, 130, 131, 132, 140);

            // Ceilings (raise/lower)
            Add(MoverKind.Ceiling, false, 40, 41, 43, 44, 72);

            // Crushing ceilings
            Add(MoverKind.Crusher, false, 6, 25, 49, 57, 73, 74, 77, 141);

            // Stairs
            Add(MoverKind.Stairs, false, 7, 8, 100, 127);

            // Elevators (Boom)
            Add(MoverKind.Elevator, false, 227, 228, 229, 230, 231, 232, 233, 234);
        }

        public static bool IsMover(int type) => Kinds.ContainsKey(type);
        public static bool IsManual(int type) => Manual.Contains(type);
        public static MoverKind Kind(int type) => Kinds.TryGetValue(type, out var k) ? k : MoverKind.None;

        // sector index -> the first mover kind found affecting it.
        public static Dictionary<int, MoverKind> ResolveMovingSectors(MapData data)
        {
            var tagToSectors = new Dictionary<int, List<int>>();
            for (int s = 0; s < data.Sectors.Length; s++)
            {
                int tag = data.Sectors[s].Trigger;
                if (tag == 0) continue;
                if (!tagToSectors.TryGetValue(tag, out var l)) { l = new List<int>(); tagToSectors[tag] = l; }
                l.Add(s);
            }

            var result = new Dictionary<int, MoverKind>();
            void Mark(int sector, MoverKind k)
            {
                if (sector < 0) return;
                if (!result.ContainsKey(sector)) result[sector] = k;
            }

            foreach (var ln in data.LineDefs)
            {
                if (ln.Type == 0 || !IsMover(ln.Type)) continue;
                var kind = Kind(ln.Type);
                if (IsManual(ln.Type))
                {
                    int back = ln.Left != -1 ? data.SideDefs[ln.Left].Sector : -1;
                    Mark(back, kind);
                }
                else
                {
                    int tag = ln.Trigger;
                    if (tag != 0 && tagToSectors.TryGetValue(tag, out var secs))
                        foreach (var s in secs) Mark(s, kind);
                }
            }
            return result;
        }

        // Lowest ceiling height among sectors sharing a two-sided line with this sector.
        // (A DOOM door opens to lowestNeighbourCeiling - 4.)
        public static int LowestNeighbourCeiling(MapData data, int sector, int fallback)
        {
            int lowest = int.MaxValue;
            foreach (var ln in data.LineDefs)
            {
                if (ln.Left == -1) continue;
                int rs = data.SideDefs[ln.Right].Sector, ls = data.SideDefs[ln.Left].Sector;
                int other = rs == sector ? ls : (ls == sector ? rs : -1);
                if (other < 0) continue;
                if (data.Sectors[other].CeilingHeight < lowest) lowest = data.Sectors[other].CeilingHeight;
            }
            return lowest == int.MaxValue ? fallback : lowest;
        }

        // Lowest floor height among neighbouring sectors. (A DOOM lift drops to lowestNeighbourFloor.)
        public static int LowestNeighbourFloor(MapData data, int sector, int fallback)
        {
            int lowest = int.MaxValue;
            foreach (var ln in data.LineDefs)
            {
                if (ln.Left == -1) continue;
                int rs = data.SideDefs[ln.Right].Sector, ls = data.SideDefs[ln.Left].Sector;
                int other = rs == sector ? ls : (ls == sector ? rs : -1);
                if (other < 0) continue;
                if (data.Sectors[other].FloorHeight < lowest) lowest = data.Sectors[other].FloorHeight;
            }
            return lowest == int.MaxValue ? fallback : lowest;
        }
    }
}
