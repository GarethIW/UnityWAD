﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityWAD
{
    // Palettes give us colour information for rendering sprites
    public class PaletteData
    {
        public Color32[] Colors;

        public PaletteData(Color32[] colors)
        {
            Colors = colors;
        }
    }
}
