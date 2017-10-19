using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityWAD
{
    // Wall textures are made up of one or more Patches, overlaid onto a single texture.
    // WallTextureData is the metadata for describing how a wall texture is constucted
    public class WallTextureData
    {
        public string Name;

        public int Width;
        public int Height;
        public int NumPatches;

        public WallPatch[] Patches;

        public WallTextureData(string name, int w, int h, WallPatch[] patches)
        {
            Name = name;
            Width = w;
            Height = h;
            Patches = patches;
            NumPatches = patches.Length;
        }
    }

    public class WallPatch
    {
        public int XOffset;
        public int YOffset;
        public int PatchIndex;
        public string PatchName;

        public WallPatch(int xOffset, int yOffset, int patchIndex, string patchName)
        {
            XOffset = xOffset;
            YOffset = yOffset;
            PatchIndex = patchIndex;
            PatchName = patchName;
        }
    }
}
