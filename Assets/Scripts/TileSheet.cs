using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityWAD
{
    public class TileSheet
    {
        public Texture2D Texture;

        // Size of tiles in this sheet
        public int TileWidth;
        public int TileHeight;

        // Number of rows/columns in sheet
        public int Rows;
        public int Columns;

        // Padding
        public int Padding;

        // Lookup table maps sprite names to sprite index/width/height in sheet
        public Dictionary<string, TileSheetSprite> LookupTable = new Dictionary<string, TileSheetSprite>();
    }

    public class TileSheetSprite
    {
        public int Width;
        public int Height;
        public int XOffset;
        public int YOffset;
        public int TileNum;
    }
}
