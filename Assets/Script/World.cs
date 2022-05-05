using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class World : MonoBehaviour
{
    //TODO 應該要用 Find 的方法...
    public Transform player;
    Terrain terrain;

    private void Awake()
    {
        terrain = new Terrain(this.gameObject);
    }
    private void Start()
    {
        this.terrain.initTerrain();
        this.player.position = this.terrain.MidPosition;
    }
    private void Update()
    {
        this.terrain.updateLoadChunk(this.player.position);
    }
}