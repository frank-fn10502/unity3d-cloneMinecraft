using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;


public class Chunk : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        int indexBase = 0;
        foreach (int face in Enum.GetValues(typeof(Voxel.Face)))
        {
            vertices.AddRange(Voxel.faceIndexs[face].Select(x => Voxel.vertices[x]));
            uvs.AddRange(Voxel.uvs.Select(x => x));
            triangles.AddRange(Voxel.triangleIndex.Select(x => indexBase + x));

            indexBase += 4;
        }

        CreateMesh();
    }

    private void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void Update()
    {

    }
}
