using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityWAD
{
    // There are two types of sprite: Picture and Raw. WADSprite will hold data for both types
    public enum SpriteType
    {
        Picture,
        Raw
    }

    public class SpriteData
    {
        public string Name;
        public SpriteType Type;

        public int Width;
        public int Height;
        public int XOffset;
        public int YOffset;

        public byte[] Data;

        public SpriteData(string name, SpriteType type, int w, int h, int xoff, int yoff, byte[] data)
        {
            Name = name;
            Type = type;
            Width = w;
            Height = h;
            XOffset = xoff;
            YOffset = yoff;
            Data = data;
        }
    }
}
