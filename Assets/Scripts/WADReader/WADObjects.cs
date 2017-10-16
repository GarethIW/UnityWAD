using System.Collections.Generic;

namespace WADReader
{
    // Classes to hold WAD file *metdata* 
    // We're not actually interested about the raw data at this level, just names and pointers

    // Holds root-level info about this WAD
    public class WADInfo
    {
        public string Type;
        public int NumEntries;
        public int DirectoryOffset;

        public List<WADEntry> Entries = new List<WADEntry>();
        public List<WADMapEntry> Maps = new List<WADMapEntry>();

        public override string ToString()
        {
            var s = "Type:\t\t\t" + Type + "\nNum Entries:\t\t" + NumEntries + "\nDirectory Start:\t" + DirectoryOffset + "\n\nEntry Name\tOffset\tLength\n------------------------------\n";
            foreach (var entry in Entries)
                s += entry.ToString() + "\n";

            return s;
        }
    }

    // Holds info about a resource entry in a WAD
    public class WADEntry
    {
        public int ResourceOffset;
        public int ResourceLength;
        public string Name;

        public override string ToString()
        {
            return Name + "\t" + ResourceOffset + "\t" + ResourceLength;
        }
    }

    // Holds metadata about a map in a WAD. Map name and pointers to resources in the WAD file where the map data can be found
    public class WADMapEntry
    {
        public string Name;
        public WADEntry Things;
        public WADEntry LineDefs;
        public WADEntry SideDefs;
        public WADEntry Vertexes;
        public WADEntry Sectors;
        // There are other parts to a map, but these mainly deal with partitioning and collision, we're not worried about that in 2017

        public override string ToString()
        {
            return Name + "\n-----\n" + Things.ToString() + "\n" + LineDefs.ToString() + "\n" + SideDefs.ToString() + "\n" + Vertexes.ToString() + "\n" + Sectors.ToString();
        }
    }
}