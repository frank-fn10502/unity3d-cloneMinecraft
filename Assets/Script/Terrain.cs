using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;


public class Terrain
{
    static readonly int worldSize = 100;
    static readonly int viewDist = 5;

    public Material material { get; private set; }

    Chunk[,] map = new Chunk[Terrain.worldSize, Terrain.worldSize];

    LaodArea laodArea;

    readonly VoxelData[] textureDatas = new VoxelData[(int)Voxel.Type.NumberOfMyEnum];


    public Transform Transform { get; private set; }

    public Vector3 MidPosition
    {
        get
        {
            return new Vector3(
                laodArea.Mid.x * Chunk.width + (Chunk.width / 2),
                Chunk.height + 3f,
                laodArea.Mid.z * Chunk.width + (Chunk.width / 2)
                );
        }
    }

    public Terrain(Transform transform)
    {
        this.Transform = transform;
        this.material = Resources.Load<Material>("Materials/terrain");

        for (int i = 0; i < (int)Voxel.Type.NumberOfMyEnum; i++)
        {
            textureDatas[i] = Voxel.voxelTypes[i];
        }
    }

    public void initTerrain()
    {
        int half = Convert.ToInt16(Math.Ceiling(Terrain.worldSize / 2f));

        this.laodArea = new LaodArea(Terrain.viewDist, new ChunkCoord(half, half));

        ActivateArea();
    }
    public void updateLoadChunk(Vector3 midPos)
    {
        var midChunk = this.Convert2ChunkCoord(midPos);
        if (laodArea.Mid.Equals(midChunk)) return;

        var preLoadArea = this.laodArea;
        this.laodArea = new LaodArea(Terrain.viewDist, midChunk);

        ActivateArea();
        DeActivateArea(preLoadArea);
    }
    void ActivateArea()
    {
        if (!ValidChunkIndex(this.laodArea.Left) && !ValidChunkIndex(this.laodArea.Right)) return;
        if (!ValidChunkIndex(this.laodArea.Bottom) && !ValidChunkIndex(this.laodArea.Top)) return;

        for (int x = LimitValue(laodArea.Left); x <= LimitValue(laodArea.Right); x++)
        {
            for (int z = LimitValue(laodArea.Bottom); z <= LimitValue(laodArea.Top); z++)
            {
                if (map[x, z] == null)
                    map[x, z] = new Chunk(this, new ChunkCoord(x, z));
                else if (!map[x, z].IsActive)
                    map[x, z].IsActive = true;
            }
        }
    }

    /// <summary>
    /// 在 2 維方向的移動最多只會 1.長方形區塊 2. L 型區塊
    /// 處理第二種狀態需要將 L 型切成兩個不相交的長方形，目前採用 "側邊長方型(沒有上/下相交) + (上/下)全通長方形"
    /// </summary>
    /// <param name="preLoadArea">上一次的有效區塊</param>
    int DeActivateArea(LaodArea preLoadArea)
    {
        int x1 = Math.Min(laodArea.Right, preLoadArea.Right);
        int x0 = Math.Max(laodArea.Left, preLoadArea.Left);

        int z1 = Math.Min(laodArea.Top, preLoadArea.Top);
        int z0 = Math.Max(laodArea.Bottom, preLoadArea.Bottom);

        int sx = -1;
        int ex = -1;
        int sz = -1;
        int ez = -1;
        //左右區塊
        //向左走
        if (x0 == preLoadArea.Left)
        {
            sx = x1 + 1;
            ex = preLoadArea.Right;
            sz = z0;
            ez = z1;
        }
        else
        {
            sx = preLoadArea.Left;
            ex = x0 - 1;
            sz = z0;
            ez = z1;
        }
        var disArea = new DisableArea(ez, ex, sz, sx);
        if (!disArea.HasValidArea) return -1;
        DeActivatColumn(disArea);

        //上下區塊
        //向上走
        if (z0 == laodArea.Bottom)
        {
            sz = preLoadArea.Bottom;
            ez = z0 - 1;
        }
        else
        {
            sz = z1 + 1;
            ez = preLoadArea.Top;
        }
        disArea = new DisableArea(ez, preLoadArea.Right, sz, preLoadArea.Left);
        if (!disArea.HasValidArea) return -1;
        DeActivatRow(disArea);

        return 0;
    }
    void DeActivatColumn(DisableArea area)
    {
        for (int x = area.Left; x <= area.Right; x++)
        {
            for (int z = area.Bottom; z <= area.Top; z++)
            {
                if (this.map[x, z] != null)
                    this.map[x, z].IsActive = false;
            }
        }
    }
    void DeActivatRow(DisableArea area)
    {
        for (int z = area.Bottom; z <= area.Top; z++)
        {
            for (int x = area.Left; x <= area.Right; x++)
            {
                if (this.map[x, z] != null)
                    this.map[x, z].IsActive = false;
            }
        }
    }

    public static bool ValidChunkIndex(int index)
    {
        return index >= 0 && index < Terrain.worldSize;
    }
    public static int LimitValue(int index)
    {
        if (index < 0) return 0;
        if (index >= Terrain.worldSize) return Terrain.worldSize - 1;

        return index;
    }

    ChunkCoord Convert2ChunkCoord(Vector3 pos)
    {
        int x = (int)pos.x / Chunk.width;
        int z = (int)pos.z / Chunk.width;

        return new ChunkCoord(x, z);
    }

    public VoxelData GetVoxelInfo(int blockId)
    {
        return this.textureDatas[blockId];
    }

    public byte GetVoxelType(Vector3 pos)
    {
        if (!this.ValidArea(pos)) return (int)Voxel.Type.Air;
        if (pos.y == 0) return (int)Voxel.Type.BedRock;
        if (pos.y == Chunk.height - 1) return (int)Voxel.Type.Grass;

        return (int)Voxel.Type.Stone;
    }

    bool ValidArea(Vector3 pos)
    {
        return pos.x >= 0 && pos.x < Chunk.width * Terrain.worldSize &&
               pos.z >= 0 && pos.z < Chunk.width * Terrain.worldSize &&
               pos.y >= 0 && pos.y < Chunk.height;

    }
}

class DisableArea
{
    private int top;
    private int right;
    private int bottom;
    private int left;

    public int Top { get => top; }
    public int Right { get => right; }
    public int Bottom { get => bottom; }
    public int Left { get => left; }

    public DisableArea(int top, int right, int bottom, int left)
    {
        this.top = top;
        this.right = right;
        this.bottom = bottom;
        this.left = left;
    }

    public bool HasValidArea
    {
        get
        {
            return Terrain.ValidChunkIndex(Top) && Terrain.ValidChunkIndex(Right) && Terrain.ValidChunkIndex(Bottom) && Terrain.ValidChunkIndex(Left);
        }
    }


}
class LaodArea
{
    private readonly int viewDist;

    ChunkCoord mid;

    public ChunkCoord Mid
    {
        get { return mid; }
        set
        {
            this.mid = value;
            this.Top = this.mid.z + this.viewDist;
            this.Right = this.mid.x + this.viewDist;
            this.Bottom = this.mid.z - this.viewDist;
            this.Left = this.mid.x - this.viewDist;
        }
    }

    public int Top { get; private set; }
    public int Right { get; private set; }
    public int Bottom { get; private set; }
    public int Left { get; private set; }

    public LaodArea(int viewDist, ChunkCoord mid)
    {
        this.viewDist = viewDist;
        this.Mid = mid;
    }
}