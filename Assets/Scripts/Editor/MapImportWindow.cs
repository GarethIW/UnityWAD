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
            using (var wadStream = new FileStream(fileName, FileMode.Open))
            {
                // Get the map data
                var mapData = WADReader.GetMapData(wadStream, loadedWad, selectedMap);

                // Get wad palette
                var paletteData = WADReader.GetPalette(wadStream, 0);

                // Get texture resources
                var texList = new List<WallTextureData>();
                var patchDict = new Dictionary<string, SpriteData>();
                foreach (var tex in mapData.WallsUsed)
                {
                    texList.Add(tex);
                    var patches = WADReader.GetWallPatches(wadStream, loadedWad, tex);
                    foreach(var kvp in patches)
                        if(!patchDict.ContainsKey(kvp.Key)) patchDict.Add(kvp.Key, kvp.Value);
                }
                var wallsSheet = SpriteGenerator.GenerateWallTextureSheet(texList, patchDict, paletteData);

                // Let's save walls as a PNG
                byte[] bytes = wallsSheet.Texture.EncodeToPNG();
                File.WriteAllBytes(Path.Combine(Application.dataPath,  mapData.Name + "-walls.png"), bytes);

                var map = new GameObject();
                map.name = mapData.Name;
                map.AddComponent<MapGenerator>();
                map.transform.position = Vector3.zero;

                var generator = map.GetComponent<MapGenerator>();
                generator.Data = mapData;
                generator.WallTiles = wallsSheet;

                // Get default diffuse for now
                var mat = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
                generator.FloorMaterial = mat;
                generator.CeilingMaterial = mat;
                generator.WallsMaterial = mat;

                generator.GenerateMesh();
            }

            Close();
        }
    }

}