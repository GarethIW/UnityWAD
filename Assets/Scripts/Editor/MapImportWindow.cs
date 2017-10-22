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

        //private string fileName = "C:/Users/gareth.MYCRMGROUP/Documents/Github/UnityWAD/DOOM.wad";
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

                // Get tilesheets
                // Walls
                var texList = new List<WallTextureData>();
                var patchDict = new Dictionary<string, SpriteData>();
                foreach (var tex in mapData.WallsUsed)
                {
                    texList.Add(tex);
                    var patches = WADReader.GetWallPatches(wadStream, loadedWad, tex);
                    foreach(var kvp in patches)
                        if(!patchDict.ContainsKey(kvp.Key)) patchDict.Add(kvp.Key, kvp.Value);
                }
                var wallsSheet = SpriteGenerator.GenerateWallTextureSheet(texList, patchDict, paletteData,0);
                // Flats
                var floorSprites = WADReader.GetRawSprites(wadStream, mapData.FlatsUsed);
                var flatsSheet = SpriteGenerator.GenerateTileSheet(floorSprites, paletteData,0);

                // Get our shaders
                var wallShader = Shader.Find("UnityWAD/DoomWalls");
                var flatShader = Shader.Find("UnityWAD/DoomFlats");

                // Let's save walls as a PNG and create an asset
                byte[] bytes = wallsSheet.Texture.EncodeToPNG();
                File.WriteAllBytes(Path.Combine(Application.dataPath, mapData.Name + "-walls.png"), bytes);
                AssetDatabase.ImportAsset("Assets/" + mapData.Name + "-walls.png", ImportAssetOptions.ForceUpdate);
                var importer = (TextureImporter)AssetImporter.GetAtPath("Assets/" + mapData.Name + "-walls.png");
                importer.filterMode = FilterMode.Point;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
                var wallsAsset = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/"+ mapData.Name + "-walls.png", typeof(Texture2D));

                // Now save the flats
                bytes = flatsSheet.Texture.EncodeToPNG();
                File.WriteAllBytes(Path.Combine(Application.dataPath, mapData.Name + "-flats.png"), bytes);
                AssetDatabase.ImportAsset("Assets/" + mapData.Name + "-flats.png", ImportAssetOptions.ForceUpdate);
                importer = (TextureImporter)AssetImporter.GetAtPath("Assets/" + mapData.Name + "-flats.png");
                importer.filterMode = FilterMode.Point;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
                var flatsAsset = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/" + mapData.Name + "-flats.png", typeof(Texture2D));

                // Create materials
                var wallMat = new Material(wallShader);
                wallMat.SetTexture("_TileMap", wallsAsset);
                wallMat.SetInt("_XTiles", wallsSheet.Columns);
                wallMat.SetInt("_YTiles", wallsSheet.Rows);
                wallMat.SetInt("_TileWidth", wallsSheet.TileWidth);
                wallMat.SetInt("_TileHeight", wallsSheet.TileHeight);
                wallMat.SetInt("_Padding", wallsSheet.Padding);
                AssetDatabase.CreateAsset(wallMat, "Assets/" + mapData.Name + "-walls.mat");

                var flatsMat = new Material(flatShader);
                flatsMat.SetTexture("_TileMap", flatsAsset);
                flatsMat.SetInt("_XTiles", flatsSheet.Columns);
                flatsMat.SetInt("_YTiles", flatsSheet.Rows);
                flatsMat.SetInt("_TileWidth", flatsSheet.TileWidth);
                flatsMat.SetInt("_TileHeight", flatsSheet.TileHeight);
                flatsMat.SetInt("_Padding", flatsSheet.Padding);
                AssetDatabase.CreateAsset(flatsMat, "Assets/" + mapData.Name + "-flats.mat");

                AssetDatabase.SaveAssets();

                // Create map parent gameobject
                var map = new GameObject(mapData.Name);
                map.transform.position = Vector3.zero;

                // Initialise the MapGenerator component and feed in tilesheets and materials
                var generator = map.AddComponent<MapGenerator>();
                generator.Data = mapData;
                generator.WallTiles = wallsSheet;
                generator.FlatTiles = flatsSheet;
                generator.WallsMaterial = wallMat;
                generator.FlatsMaterial = flatsMat;

                generator.GenerateMesh();
            }

            Close();
        }
    }

}