using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;


public class Chunk
{
    public static readonly byte width = 5, height = 2;
    private readonly Terrain terrain;

    private readonly ChunkCoord coord;
    GameObject gameObj;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    int vertexBase;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] map = new byte[width, height, width];

    public bool IsActive
    {
        get { return this.gameObj.activeSelf; }
        set { this.gameObj.SetActive(value); }
    }

    public Vector3 Position
    {
        get { return this.gameObj.transform.position; }
    }

    public Chunk(Terrain terrain, ChunkCoord coord)
    {
        this.terrain = terrain;
        this.coord = coord;

        this.gameObj = new GameObject();
        this.meshFilter = this.gameObj.AddComponent<MeshFilter>();
        this.meshRenderer = this.gameObj.AddComponent<MeshRenderer>();
        this.meshRenderer.material = this.terrain.material;

        this.gameObj.transform.SetParent(this.terrain.Transform);
        this.gameObj.name = $"chunk {this.coord.x}, {this.coord.z}";
        this.gameObj.transform.position = new Vector3(
            this.coord.x * Chunk.width,
            0,
            this.coord.z * Chunk.width
        );

        this.populateChunk();


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

    void populateChunk()
    {
        for (int x = 0; x < Chunk.width; x++)
        {
            for (int y = 0; y < Chunk.height; y++)
            {
                for (int z = 0; z < Chunk.width; z++)
                {
                    this.map[x, y, z] = this.terrain.GetVoxelType(new Vector3(x, y, z) + this.Position);
                }
            }
        }
    }

    public bool HasBlock(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        int blockId = -1;
        if (!this.ValidArea(x, y, z))
            blockId = this.terrain.GetVoxelType(pos + this.Position);
        else
            blockId = this.map[x, y, z];


        return this.terrain.GetVoxelInfo(blockId).IsSolid;
    }

    private void AddVoxelData(Vector3 pos)
    {
        int blockId = this.map[(int)pos.x, (int)pos.y, (int)pos.z];
        foreach (int face in Enum.GetValues(typeof(Voxel.Face)))
        {
            if (HasBlock(pos + Voxel.faceDirs[face])) continue;
            if (pos.y == 0 && face != (int)Voxel.Face.TOP) continue;//最下層的 voxel 只需渲染最上面的方塊即可

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
        var block = this.terrain.GetVoxelInfo(blockId);

        var textureIndex = block.GetTexture(face);

        float wUnit = 1f / VoxelData.wNum;
        float hUnit = 1f / VoxelData.hNum;

        //左下角
        float y = textureIndex / VoxelData.wNum;
        float x = textureIndex % VoxelData.wNum;

        y = VoxelData.hNum - 1 - y;

        x *= wUnit;
        y *= hUnit;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + hUnit));
        uvs.Add(new Vector2(x + wUnit, y + hUnit));
        uvs.Add(new Vector2(x + wUnit, y));
    }

    bool ValidArea(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0) return false;
        if (x >= width || y >= height || z >= width) return false;

        return true;
    }
}

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