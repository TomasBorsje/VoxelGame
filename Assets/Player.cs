using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public class Player : LivingEntity
{
    public int reachDistance = 5;
    public static int INVENTORY_SIZE = 30;
    public static int HOTBAR_SIZE = 9;
    Camera cam;

    int selectedSlot = 0; // Wraps around 0-8
    ItemContainer inventory;
    private void Awake()
    {
        cam = transform.GetComponentInChildren<Camera>();
        inventory = new ItemContainer(INVENTORY_SIZE);
        inventory.SetStackInSlot(0, new ItemStack(new BlockItem(BlockRegistry.PLANKS), 8));
        inventory.SetStackInSlot(1, new ItemStack(new BlockItem(BlockRegistry.GLASS), 8));
    }
    void Update()
    {
        HandleInputs();
        Debug.Log("Inventory: " + inventory);
    }
    void HandleInputs()
    {
        // Try to break block
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryBreakBlock();
        }
        // Try to place block
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            TryUseSelectedItem();
        }
        // Switch selected hotbar item if scrolling
        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
        {
            HotbarScroll();
        }
    }
    ItemStack GetSelectedItem()
    {
        return inventory.GetStackInSlot(selectedSlot);
    }
    void HotbarScroll()
    {
        float scrollDir = Input.mouseScrollDelta.y;
        // Scroll down = slot increase
        if(scrollDir < 0)
        {
            selectedSlot = (selectedSlot + 1) % 9;
        }
        else
        {
            selectedSlot = (selectedSlot - 1) % 9;
        }
    }
    void TryBreakBlock()
    {
        RaycastHit hit;
        Transform headTransform = GetHeadTransform();
        // Does the ray intersect any objects excluding the player layer?
        if (Physics.Raycast(headTransform.position, headTransform.TransformDirection(Vector3.forward), out hit, reachDistance))
        {
            // TODO: Fix this!!
            Vector3 forwardHit = hit.point + headTransform.TransformDirection(Vector3.forward) * 0.001f;
            Debug.Log($"Did Hit at {forwardHit.x},{forwardHit.y},{forwardHit.z}");
            (Chunk, Vector3Int) chunkPos = WorldGenHandler.INSTANCE.WorldPosToChunkPos(forwardHit);
            chunkPos.Item1.SetBlock(chunkPos.Item2.x, chunkPos.Item2.y, chunkPos.Item2.z, BlockRegistry.AIR);
        }
    }
    void TryUseSelectedItem()
    {
        // TODO: Try and use any blocks we're looking at first
        if (GetSelectedItem() != ItemStack.EMPTY)
        {
            ItemStack returnedStack;
            UseResult result = GetSelectedItem().Use(this, out returnedStack);
            inventory.SetStackInSlot(selectedSlot, returnedStack);
        }
    }

    public override Transform GetHeadTransform()
    {
        return cam.gameObject.transform;
    }
}
