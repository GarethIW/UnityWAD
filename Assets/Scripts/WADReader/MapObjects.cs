using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityWAD
{
    // Structs to hold map data
    // As defined by: https://github.com/nukeop/TheUnofficialDoomSpecs/blob/master/Chapter4.md

    public class MapLineDef
    {
        public int From;
        public int To;
        public BitArray Attribute;
        public int Type;
        public int Trigger;
        public int Right;
        public int Left;

        public MapLineDef(int from, int to, int attribute, int type, int trigger, int right, int left)
        {
            From = from;
            To = to;
            Attribute = new BitArray(BitConverter.GetBytes(attribute));
            Type = type;
            Trigger = trigger;
            Right = right;
            Left = left;
        }
    }

    public class MapSideDef
    {
        public int XOffset;
        public int YOffset;
        public string UpperTexture;
        public string LowerTexture;
        public string FullTexture;
        public int Sector;

        public MapSideDef(int xoff, int yoff, string uppertex, string lowertex, string fulltex, int sector)
        {
            XOffset = xoff;
            YOffset = yoff;
            UpperTexture = uppertex;
            LowerTexture = lowertex;
            FullTexture = fulltex;
            Sector = sector;
        }
    }

    public class MapSector
    {
        public int FloorHeight;
        public int CeilingHeight;
        public string FloorTexture;
        public string CeilingTexture;
        public int Brightness;
        public int Special;
        public int Trigger;

        public MapSector(int floorh, int ceilingh, string floortex, string ceiltex, int brightness, int special, int trigger)
        {
            FloorHeight = floorh;
            CeilingHeight = ceilingh;
            FloorTexture = floortex;
            CeilingTexture = ceiltex;
            Brightness = brightness;
            Special = special;
            Trigger = trigger;
        }
    }

    public class MapSubSector
    {
        public int NumSegs;
        public int FirstSeg;

        public MapSubSector(int numsegs, int firstseg)
        {
            NumSegs = numsegs;
            FirstSeg = firstseg;
        }
    }

    public class MapSeg
    {
        public int From;
        public int To;
        public int Angle;
        public int LineDef;
        public int Side;
        public int Offset;

        public MapSeg(int from, int to, int angle, int line, int side, int offset)
        {
            From = from;
            To = to;
            Angle = angle;
            LineDef = line;
            Side = side;
            Offset = offset;
        }
    }

    public class MapVertex
    {
        public int X;
        public int Y;

        public MapVertex(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return X + "," + Y;
        }
    }
}
