using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VoxelTextureMap
{
    private static VoxelTextureMap voxelTextureMap;

    private static readonly int block_num_width = 16;
    private static readonly int block_num_height = 16;
    private static readonly int block_width = 16;

    public Material material { get; private set; }

    public VoxelInfo BedRock { get; private set; }
    public VoxelInfo Bricks { get; private set; }
    public VoxelInfo Cobblestone { get; private set; }
    public VoxelInfo Dirt { get; private set; }
    public VoxelInfo Grass { get; private set; } 
    public VoxelInfo Planks { get; private set; }
    public VoxelInfo Sand { get; private set; }
    public VoxelInfo Stone { get; private set; }
    public VoxelInfo Wood { get; private set; }
    public VoxelInfo Air { get; private set; }
    

    VoxelInfo[] voxelInfoList;

    private VoxelTextureMap()
    {
        material = Resources.Load<Material>("Materials/terrain");

        Air = createVoxelInfo(0, 0, 0, 0, 0, 0, 0, false);
        BedRock = createVoxelInfo(1, 17, 17, 17, 17, 17, 1);
        Grass = createVoxelInfo(2, 3, 3, 3, 3, 0, 2);
        Stone = createVoxelInfo(3, 1, 1, 1, 1, 1, 1);
        Bricks = createVoxelInfo(4, 7, 7, 7, 7, 7, 7);
        Cobblestone = createVoxelInfo(5, 62, 62, 62, 62, 62, 62);
        Dirt = createVoxelInfo(6, 2, 2, 2, 2, 2, 2);
        Planks = createVoxelInfo(7, 4, 4, 4, 4, 4, 4);
        Sand = createVoxelInfo(8, 18, 18, 18, 18, 18, 18);
        Wood = createVoxelInfo(9, 20, 20, 20, 20, 21, 21); 
        

        voxelInfoList = new VoxelInfo[]{
            Air,
            BedRock,
            Grass,
            Stone,
            Bricks,
            Cobblestone,
            Dirt,
            Planks,
            Sand,
            Wood
        };
    }

    public static VoxelTextureMap getVoxelTextureMap(){
        if(voxelTextureMap == null){
            voxelTextureMap = new VoxelTextureMap();
        }
        return voxelTextureMap;
    }

    VoxelInfo createVoxelInfo(byte id, int front, int back, int left, int right, int top, int bottom, bool isSolid = true)
    {
        List<Vector2> front_uvs;
        List<Vector2> back_uvs;
        List<Vector2> left_uvs;
        List<Vector2> right_uvs;
        List<Vector2> top_uvs;
        List<Vector2> bottom_uvs;

        front_uvs = getUvsById(front);
        back_uvs = getUvsById(back);
        left_uvs = getUvsById(left);
        right_uvs = getUvsById(right);
        top_uvs = getUvsById(top);
        bottom_uvs = getUvsById(bottom);

        return new VoxelInfo(id, front_uvs, back_uvs, left_uvs, right_uvs, top_uvs, bottom_uvs, isSolid);
    }

    /// <summary>
    /// texture map 的對應位置，左上為 0，右下為最大數值
    /// </summary>
    List<Vector2> getUvsById(int textureIndex){
        List<Vector2> result = new List<Vector2>();

        float unit = 1f / block_width;

        float y = textureIndex / block_num_height;
        float x = textureIndex % block_num_width;

        y = block_num_height - 1 - y;

        x *= unit;
        y *= unit;

        result.Add(new Vector2(x, y));
        result.Add(new Vector2(x, y + unit));
        result.Add(new Vector2(x + unit, y + unit));
        result.Add(new Vector2(x + unit, y));

        return result;
    }

    public VoxelInfo GetVoxelInfo(int blockId){
        return this.voxelInfoList[blockId];
    }
}