using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VoxelInfo
{
    public enum Face
    {
        FRONT = 0,
        BACK,
        LEFT,
        RIGHT,
        TOP,
        BOTTOM
    }

    #region static 區段

    public const int verticesPerFace = 4;
    public static readonly Vector3[] vertices = new Vector3[8] {
        new Vector3(0f,0f,0f),
        new Vector3(0f,1f,0f),
        new Vector3(1f,1f,0f),
        new Vector3(1f,0f,0f),
        new Vector3(0f,0f,1f),
        new Vector3(0f,1f,1f),
        new Vector3(1f,1f,1f),
        new Vector3(1f,0f,1f)
    };

    public static readonly int[][] faceIndexs = new int[6][]{
        new int[verticesPerFace]{0,1,2,3}, //front
        new int[verticesPerFace]{7,6,5,4}, //back
        new int[verticesPerFace]{4,5,1,0}, //left
        new int[verticesPerFace]{3,2,6,7}, //right
        new int[verticesPerFace]{1,5,6,2}, //top
        new int[verticesPerFace]{4,0,3,7} //bottom
    };

    public static readonly Vector3[] faceDirs = new Vector3[6]{
        new Vector3(0f,0f,-1f), //front
        new Vector3(0f,0f,1f), //back
        new Vector3(-1f,0f,0f), //left
        new Vector3(1f,0f,0f), //right
        new Vector3(0f,1f,0f), //top
        new Vector3(0f,-1f,0f), //bottom
    };

    public static readonly int[] triangleIndex = new int[6] { 0, 1, 3, 3, 1, 2 };

    #endregion

    readonly List<Vector2> front;
    readonly List<Vector2> back;
    readonly List<Vector2> left;
    readonly List<Vector2> right;
    readonly List<Vector2> top;
    readonly List<Vector2> bottom;

    public byte Id { get; private set; }
    public bool IsSolid { get; private set; }

    public VoxelInfo(byte id, List<Vector2> front, List<Vector2> back, List<Vector2> left, List<Vector2> right, List<Vector2> top, List<Vector2> bottom, bool isSolid = true)
    {
        this.Id = id;
        this.IsSolid = isSolid;

        this.front = front;
        this.back = back;
        this.left = left;
        this.right = right;
        this.top = top;
        this.bottom = bottom;
    }

    public List<Vector2> GetTextureUvs(VoxelInfo.Face face)
    {
        switch (face)
        {
            case VoxelInfo.Face.FRONT: return this.front;
            case VoxelInfo.Face.BACK: return this.back;
            case VoxelInfo.Face.LEFT: return this.left;
            case VoxelInfo.Face.RIGHT: return this.right;
            case VoxelInfo.Face.TOP: return this.top;
            case VoxelInfo.Face.BOTTOM: return this.bottom;

            default:
                return null;
        }
    }
}