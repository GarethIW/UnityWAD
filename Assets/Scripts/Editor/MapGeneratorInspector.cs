using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityWAD
{
    [CustomEditor(typeof(MapGenerator))]
    class MapGeneratorInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate"))
            {
                ((MapGenerator)target).GenerateMesh();
            }
        }

    }
}

