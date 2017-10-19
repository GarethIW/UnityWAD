using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityWAD
{
    public class SpriteSheet
    {
        public Texture2D Texture;

        // Lookup table maps sprite names to index in sheet
        public Dictionary<string, int> LookupTable;
    }
}
