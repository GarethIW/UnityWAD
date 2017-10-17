/////////////////////////////////////////////////////////////////////////
// 
// PicaVoxel - The tiny voxel engine for Unity - http://picavoxel.com
// By Gareth Williams - @garethiw - http://gareth.pw
// 
// Source code distributed under standard Asset Store licence:
// http://unity3d.com/legal/as_terms
//
/////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityWAD
{
    public static class EditorMenus
    {
        public const string MENU_MAIN_ROOT = "UnityWAD/";

        [MenuItem(MENU_MAIN_ROOT + "Import map from WAD", false, 1000)]
        private static void DoCreateManager()
        {
            MapImportWindow window = (MapImportWindow)EditorWindow.GetWindowWithRect((typeof(MapImportWindow)), new Rect(100, 100, 400, 100), true);
            window.Init();
        }
    }

}