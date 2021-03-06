using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    //TODO 應該要用 Find 的方法...
    public Transform player;
    public Terrain terrain;
    
    public BlockType[] blocktypes;
    public VoxelInfo VoxelInfo ;

    private void Awake()
    {
        terrain = new Terrain(this.gameObject.transform, this.player);
    }
    private void Start()
    {
        Vector3 temp = new Vector3(0,50,0);
        this.player.position = this.terrain.MidPosition + temp;
        StartCoroutine(this.terrain.UpdateLoadChunk());
    }
    private void Update()
    {
    }

    [System.Serializable]
    public class BlockType
    {
        public string blockName;
        public bool isSolid;
        public Sprite icon;
    }
}