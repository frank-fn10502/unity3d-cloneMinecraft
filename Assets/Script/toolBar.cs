using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class toolBar : MonoBehaviour
{
    World world;
    //public Player player;
    public RectTransform highlight;
    public ItemSlot[] itemSlots;
    int slotIndex = 0;

    private void Start()
    {
        world = GameObject.Find("world").GetComponent<World>();
        foreach (ItemSlot slot in itemSlots)// VoxelTextureMap.getVoxelTextureMap().voxelInfoList
        {
            slot.icon.sprite = world.blocktypes[slot.itemID].icon;
            slot.icon.enabled = true;
            slot.VoxelInfo = VoxelTextureMap.getVoxelTextureMap().GetVoxelInfo(slot.itemID);
        }
        world.VoxelInfo = itemSlots[slotIndex].VoxelInfo;
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
                slotIndex--;
            else
                slotIndex++;
            if (slotIndex > itemSlots.Length - 1)
                slotIndex = 0;
            else if (slotIndex < 0)
                slotIndex = itemSlots.Length - 1;

            highlight.position = itemSlots[slotIndex].icon.transform.position;
            //player.selectedBlockIndex = itemSlots[slotIndex].itemID;
            world.VoxelInfo = itemSlots[slotIndex].VoxelInfo;
        }
    }

}

[System.Serializable]
public class ItemSlot
{
    public byte itemID;
    public Image icon;
    public VoxelInfo VoxelInfo;
}