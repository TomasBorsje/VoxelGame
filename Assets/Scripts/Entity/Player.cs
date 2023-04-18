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
    public static float UI_SCALE = 2;

    private static readonly KeyCode[] HOTBAR_KEYCODES = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };
    Camera cam;
    InventorySlotDisplay[] uiSlots = new InventorySlotDisplay[HOTBAR_SIZE];
    int selectedSlot = 0; // Wraps around 0-8
    ItemContainer inventory;

    public ItemContainer Inventory { get => inventory; }
    public int SelectedSlot { get => selectedSlot; }
    private void Awake()
    {
        cam = transform.GetComponentInChildren<Camera>();
        inventory = new ItemContainer(INVENTORY_SIZE);
        inventory.SetStackInSlot(0, new ItemStack(new BlockItem(BlockRegistry.PLANKS), 8));
        inventory.SetStackInSlot(1, new ItemStack(new BlockItem(BlockRegistry.GLASS), 8));
        inventory.SetStackInSlot(2, new ItemStack(new BlockItem(BlockRegistry.DANDELION), 8));

        // Setup ui
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        for (int hotbarSlot = 0; hotbarSlot < HOTBAR_SIZE; hotbarSlot++) 
        {
            GameObject slotHandler = Instantiate(Resources.Load<GameObject>("Prefabs/UI/InventorySlotDisplay"));
            uiSlots[hotbarSlot] = slotHandler.GetComponent<InventorySlotDisplay>();
            uiSlots[hotbarSlot].Init(this, hotbarSlot);
            slotHandler.transform.SetParent(canvas.transform);
            ((RectTransform)slotHandler.transform).localPosition = new Vector2(40*hotbarSlot*UI_SCALE-40*4.5f*UI_SCALE, -350);
            slotHandler.transform.localScale = Vector3.one * UI_SCALE;
        }
    }
    protected override void Update()
    {
        base.Update();
        HandleInputs();
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
        TrySwitchSelectedSlot();
    }
    private void TrySwitchSelectedSlot()
    {
        for (int i = 0; i < HOTBAR_KEYCODES.Length; i++)
        {
            if (Input.GetKeyDown(HOTBAR_KEYCODES[i]))
            {
                selectedSlot = i;
            }
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
            selectedSlot = (selectedSlot + 1) % HOTBAR_SIZE;
        }
        else
        {
            selectedSlot = (selectedSlot - 1) % HOTBAR_SIZE;
            if(selectedSlot < 0) { selectedSlot += HOTBAR_SIZE; }
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
            // Get hit block
            Block hitBlock = chunkPos.Item1.GetBlock(chunkPos.Item2.x, chunkPos.Item2.y, chunkPos.Item2.z);
            // Don't interact with air!
            if (!hitBlock.Empty)
            {
                BlockBreakEffect.CreateBlockBreakEffect(Vector3Int.FloorToInt(forwardHit) + new Vector3(0.5f, 0.5f, 0.5f));
                // Delete block
                chunkPos.Item1.SetBlock(chunkPos.Item2.x, chunkPos.Item2.y, chunkPos.Item2.z, BlockRegistry.AIR);
                // Spawn item entity!
                ItemStackEntity.CreateItemStackEntity(Vector3Int.FloorToInt(forwardHit) + new Vector3(0.5f, 0.5f, 0.5f), new ItemStack(BlockItemRegistry.ITEMS.Get(hitBlock.Id)()));
            }
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
