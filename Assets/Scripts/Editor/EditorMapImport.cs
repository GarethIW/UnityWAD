using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityWAD
{
    public static class EditorMapImport
    {
        public static void DoImport(MapData mapData)
        {
            // Create an empty gameobject to hold our map
            var map = new GameObject();
            map.name = mapData.Name;
            map.AddComponent<MapGenerator>();
            map.transform.position = Vector3.zero;

            var generator = map.GetComponent<MapGenerator>();
            generator.Data = mapData;

            generator.GenerateMesh();
        }
    }
}
