using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class ChunkCoord
{
    public int x;
    public int z;

    public ChunkCoord(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public bool Equals(ChunkCoord coord)
    {
        return x == coord.x &&
               z == coord.z;
    }
}

public class Chunk
{
    public static readonly byte width = 16, height = 5;
    private readonly Terrain terrain;

    private readonly ChunkCoord coord;
    public GameObject chunk;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    int vertexBase;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] map = new byte[width, height, width];

    public Chunk(Terrain terrain, ChunkCoord coord)
    {
        this.terrain = terrain;
        this.coord = coord;

        this.chunk = new GameObject();
        this.meshFilter = this.chunk.AddComponent<MeshFilter>();
        this.meshRenderer = this.chunk.AddComponent<MeshRenderer>();
        this.meshRenderer.material = this.terrain.material;

        this.chunk.transform.SetParent(this.terrain.gameObj.transform);
        this.chunk.name = $"chunk {this.coord.x}, {this.coord.z}";
        this.chunk.transform.position = new Vector3(
            this.coord.x * Chunk.width,
            0,
            this.coord.z * Chunk.width
        );

        for (int x = 0; x < Chunk.width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < Chunk.width; z++)
                {
                    this.map[x, y, z] = 2;
                }
            }
        }


        this.vertexBase = 0;
        for (int x = 0; x < Chunk.width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < Chunk.width; z++)
                {
                    AddVoxelData(new Vector3(x, y, z));
                }
            }
        }

        CreateMesh();
    }

    public bool HasBlock(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (y < 0 || y >= height) return false;

        if (x < 0 || y < 0 || z < 0) return false;
        if (x >= width || y >= height || z >= width) return false;

        return this.terrain.textureDatas[this.map[x, y, z]].IsSolid;
    }

    private void AddVoxelData(Vector3 pos)
    {
        int blockId = this.map[(int)pos.x, (int)pos.y, (int)pos.z];
        foreach (int face in Enum.GetValues(typeof(Voxel.Face)))
        {
            if (HasBlock(pos + Voxel.faceDirs[face])) continue;

            vertices.AddRange(Voxel.faceIndexs[face].Select(x => Voxel.vertices[x] + pos));
            this.AddTexture(blockId, (Voxel.Face)face);

            triangles.AddRange(Voxel.triangleIndex.Select(x => vertexBase + x));

            vertexBase += Voxel.verticesPerFace;
        }
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

    public void AddTexture(int blockId, Voxel.Face face)
    {
        var block = this.terrain.textureDatas[blockId];

        var textureIndex = block.GetTexture(face);

        float wUnit = 1f / TextureData.wNum;
        float hUnit = 1f / TextureData.hNum;

        //左下角
        float y = textureIndex / TextureData.wNum;
        float x = textureIndex % TextureData.wNum;

        y = TextureData.hNum - 1 - y;

        x *= wUnit;
        y *= hUnit;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + hUnit));
        uvs.Add(new Vector2(x + wUnit, y + hUnit));
        uvs.Add(new Vector2(x + wUnit, y));
    }
}
