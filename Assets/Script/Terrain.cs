using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;


public class Terrain
{
    #region LoadArea
    class LoadArea{
        public int top;
        public int bottom;
        public int left;
        public int right;

        public LoadArea(ChunkCoord mid)
        {
            this.top = mid.z + Terrain.viewDist;
            this.bottom = mid.z - Terrain.viewDist;
            this.left = mid.x - Terrain.viewDist;
            this.right = mid.x + Terrain.viewDist;

            this.validLoadArea();
        }

        void validLoadArea(){
            if(top >= Terrain.worldSize){
                this.top = Terrain.worldSize - 1;
                this.bottom = this.top - Terrain.viewDist * 2;
            }
            if(bottom < 0){
                this.bottom = 0;
                this.top = this.bottom + Terrain.viewDist * 2;
            }
            if(left < 0){
                this.left = 0;
                this.right = this.left + Terrain.viewDist * 2;
            }
            if(right >= Terrain.worldSize){
                this.right = Terrain.worldSize - 1;
                this.left = this.right - Terrain.viewDist * 2;
            }
        }
    }
    #endregion

    static readonly int worldSize = 100;
    static readonly int viewDist = 3;

    Chunk[,] map = new Chunk[Terrain.worldSize, Terrain.worldSize];

    List<Chunk> loadingChunks;
    ChunkCoord midChunkCoord;

    List<ChunkCoord> chunkToCreate;

    VoxelTextureMap voxelTextureMap{
        get { return VoxelTextureMap.getVoxelTextureMap(); }
    }

    public Transform Transform { get; private set; }
    Transform player;

    public Vector3 MidPosition
    {
        get
        {
            return new Vector3(
                (Terrain.worldSize / 2f) * Chunk.width + (Chunk.width / 2),
                Chunk.height + 3f,
                (Terrain.worldSize / 2f) * Chunk.width + (Chunk.width / 2)
                );
        }
    }

    public Terrain(Transform transform, Transform player)
    {
        this.Transform = transform;
        this.player = player;
        this.loadingChunks = new List<Chunk>();
        this.chunkToCreate = new List<ChunkCoord>();

        this.midChunkCoord = this.Convert2ChunkCoord(this.MidPosition);
        this.UpdateLoadArea();
    }

    public bool CheckForVoxel(Vector3 pos){
        if(!ValidArea(pos)) return false;

        var cood = Convert2ChunkCoord(pos);
        var chunk = map[cood.x, cood.z];

        if(chunk == null) return false;
        if(!chunk.IsActive) return false;

        // return chunk.HasBlock(pos);
        return true;
    }

    public void CreateVoxel(Vector3 pos, Vector3 dir, byte id){
        pos -= dir * .5f;
        if(!CheckForVoxel(pos)) return;
        if(!voxelTextureMap.isValidId(id)) return;

        var cood = Convert2ChunkCoord(pos);
        var chunk = map[cood.x, cood.z];
        
        chunk.EditVoxel(pos, id);
    }

    public void RemoveVoxel(Vector3 pos, Vector3 dir){
        pos += dir * .1f;
        if(!CheckForVoxel(pos)) return;

        var cood = Convert2ChunkCoord(pos);
        var chunk = map[cood.x, cood.z];

        chunk.EditVoxel(pos, voxelTextureMap.Air.Id);
    }

    public IEnumerator UpdateLoadChunk()
    {
        while(true)
        {
            //生成新的 chunk
            while(chunkToCreate.Count > 0)
            {
                var coord = chunkToCreate[0];
                this.map[coord.x, coord.z] = new Chunk(this, coord);

                chunkToCreate.RemoveAt(0);
                this.loadingChunks.Add(this.map[coord.x, coord.z]);

                yield return null;
            }

            //更新顯示區域
            var midChunkCoord = this.Convert2ChunkCoord(this.player.position);
            if (this.midChunkCoord.x != midChunkCoord.x || this.midChunkCoord.z != midChunkCoord.z) 
            {
                this.midChunkCoord = midChunkCoord;
                UpdateLoadArea();
            }

            yield return null;
        }
    }
    
    void UpdateLoadArea()
    {
        var loadArea = new LoadArea(this.midChunkCoord);

        var oldLoadingChunks = this.loadingChunks;
        var newLoadingChunks = new List<Chunk>();

        for (int i = loadArea.left; i <= loadArea.right; i++)
        {
            for (int j = loadArea.bottom ; j <= loadArea.top ; j++)
            {
                if(map[i,j] != null)
                    newLoadingChunks.Add(map[i,j]);
                else
                {
                    chunkToCreate.Add(new ChunkCoord(i, j));
                }
            }
        }

        newLoadingChunks.Except(oldLoadingChunks).ToList().ForEach( chunk => chunk.IsActive = true);
        oldLoadingChunks.Except(newLoadingChunks).ToList().ForEach( chunk => chunk.IsActive = false);

        // chunkToCreate.ForEach( coord => {
        //     this.map[coord.x, coord.z] = new Chunk(this, coord);
        //     newLoadingChunks.Add(this.map[coord.x, coord.z]);
        // });

        this.loadingChunks = newLoadingChunks;
    }

    ChunkCoord Convert2ChunkCoord(Vector3 pos)
    {
        int x = (int)pos.x / Chunk.width;
        int z = (int)pos.z / Chunk.width;

        return new ChunkCoord(x, z);
    }

    public byte GetVoxelType(Vector3 pos)
    {
        if (!this.ValidArea(pos)) return voxelTextureMap.Air.Id;
        if (pos.y == 0) return voxelTextureMap.BedRock.Id;
        if (pos.y == Chunk.height - 1) return voxelTextureMap.Grass.Id;

        return voxelTextureMap.Stone.Id;
    }

    bool ValidArea(Vector3 pos)
    {
        return pos.x >= 0 && pos.x < Chunk.width * Terrain.worldSize &&
               pos.z >= 0 && pos.z < Chunk.width * Terrain.worldSize &&
               pos.y >= 0 && pos.y < Chunk.height;

    }
}