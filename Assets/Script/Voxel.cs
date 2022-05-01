using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel
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
        new int[4]{0,1,2,3}, //front
        new int[4]{7,6,5,4}, //back
        new int[4]{4,5,1,0}, //left
        new int[4]{3,2,6,7}, //right
        new int[4]{1,5,6,2}, //top
        new int[4]{4,0,3,7} //bottom
    };

    public static readonly int[] triangleIndex = new int[6] { 0, 1, 3, 3, 1, 2 };

    public static readonly Vector2[] uvs = new Vector2[4]{
        new Vector2(0f,0f),
        new Vector2(0f,1f),
        new Vector2(1f,1f),
        new Vector2(1f,0f)
    };
}