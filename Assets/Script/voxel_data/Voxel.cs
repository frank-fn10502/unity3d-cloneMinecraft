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
}