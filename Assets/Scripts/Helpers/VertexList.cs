using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityWAD
{
    public class VertexList
    {
        public enum VertexListMode
        {
            Shared,
            PerFace
        }

        public VertexListMode Mode = VertexListMode.Shared;
        public List<Vector3> Vertices = new List<Vector3>();

        public int Add(Vector3 newVert)
        {
            if (Mode == VertexListMode.Shared)
            {
                for (int i = 0; i < Vertices.Count; i++)
                    if (Vertices[i] == newVert) return i;
            }

            Vertices.Add(newVert);
            return Vertices.Count-1;
        }

        public int Count()
        {
            return Vertices.Count;
        }
        public Vector3[] ToArray()
        {
            return Vertices.ToArray();
        }
    }
}
