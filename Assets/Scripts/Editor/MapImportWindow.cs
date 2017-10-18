/////////////////////////////////////////////////////////////////////////
// 
// PicaVoxel - The tiny voxel engine for Unity - http://picavoxel.com
// By Gareth Williams - @garethiw - http://gareth.pw
// 
// Source code distributed under standard Asset Store licence:
// http://unity3d.com/legal/as_terms
//
/////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace UnityWAD
{
    public class MapImportWindow : EditorWindow
    {
        private WADInfo loadedWad = null;
        List<WADMapEntry> availableMaps = new List<WADMapEntry>();
        private WADMapEntry selectedMap = null;

        //private string fileName = "C:/Users/gareth.MYCRMGROUP/Documents/Github/UnityWAD/DOOM2.wad";
        private string fileName = "G:/Code/UnityWAD/doom.wad";
        private int mapIndex;

        public void Init()
        {
            titleContent = new GUIContent("UnityWAD Map Import");
        }

        public void OnGUI()
        {
            if (loadedWad == null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField("WAD File: ", fileName);
                if (GUILayout.Button("Browse"))
                {
                    string path = EditorUtility.OpenFilePanel("Select WAD file", "", "wad");
                    if (path.Length != 0)
                    {
                        fileName = path;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Cancel")) Close();
                GUI.enabled = !string.IsNullOrEmpty(fileName);
                if (GUILayout.Button("Next")) LoadWad();
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                var maps = new string[availableMaps.Count];
                for (var i = 0; i < maps.Length; i++)
                    maps[i] = availableMaps[i].Name;

                mapIndex = EditorGUILayout.Popup("Select map: ", mapIndex, maps);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Cancel")) Close();
                GUI.enabled = !string.IsNullOrEmpty(fileName);
                if (GUILayout.Button("Import")) ImportMap();
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }
        }

        void LoadWad()
        {
            using (var wadStream = new FileStream(fileName, FileMode.Open))
            {
                loadedWad = WADReader.GetInfo(wadStream);
                availableMaps = WADReader.GetMapList(wadStream, loadedWad);
            }
        }

        void ImportMap()
        {
            var selectedMap = availableMaps[mapIndex];
            MapData data = null;
            using (var wadStream = new FileStream(fileName, FileMode.Open))
            {
                data = WADReader.GetMapData(wadStream, selectedMap);
            }

            if(data!=null)
                EditorMapImport.DoImport(data);

            Close();
        }
    }

}