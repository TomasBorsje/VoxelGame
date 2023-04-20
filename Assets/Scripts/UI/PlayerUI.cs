using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI
{
    public enum Screen { None, Inventory }
    public static float UI_SCALE = 2;

    private Player player;
    GameObject canvas;
    GameObject hotbarDisplay;
    HotbarSlotDisplay[] hotbarSlots = new HotbarSlotDisplay[Player.HOTBAR_SIZE];
    InventorySlotDisplay[] invSlots = new InventorySlotDisplay[Player.INVENTORY_SIZE];
    MouseHeldSlotDisplay mouseHeldSlot;
    GameObject inventoryDisplay;
    private Screen currentScreen;
    FirstPersonLook lookScript;

    public Screen CurrentScreen => currentScreen;
    public bool UiOpen => currentScreen != Screen.None;

    public PlayerUI(Player player)
    {
        this.player = player;
        lookScript = player.GetComponentInChildren<FirstPersonLook>();
        // Init UI object holders
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        // Hotbar holder
        hotbarDisplay = new GameObject();
        hotbarDisplay.transform.SetParent(canvas.transform);
        hotbarDisplay.transform.localPosition = Vector3.zero;
        // Inventory holder
        inventoryDisplay = new GameObject();
        inventoryDisplay.transform.SetParent(canvas.transform);
        inventoryDisplay.transform.localPosition = Vector3.zero;

        // Create individual hotbar slots and add to hotbar object
        for (int hotbarSlot = 0; hotbarSlot < Player.HOTBAR_SIZE; hotbarSlot++)
        {
            GameObject slotHandler = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/HotbarSlotDisplay"));
            hotbarSlots[hotbarSlot] = slotHandler.GetComponent<HotbarSlotDisplay>();
            hotbarSlots[hotbarSlot].Init(this.player, hotbarSlot);
            slotHandler.transform.SetParent(hotbarDisplay.transform);
            ((RectTransform)slotHandler.transform).localPosition = new Vector2(40 * hotbarSlot * UI_SCALE - 40 * 4.5f * UI_SCALE, -350);
            slotHandler.transform.localScale = Vector3.one * UI_SCALE;
        }

        // Create individual inv slots and add to inventory object
        for (int invSlot = 0; invSlot < player.Inventory.Size; invSlot++)
        {
            GameObject slotHandler = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/InventorySlotDisplay"));
            invSlots[invSlot] = slotHandler.GetComponent<InventorySlotDisplay>();
            invSlots[invSlot].Init(this.player, invSlot);
            slotHandler.transform.SetParent(inventoryDisplay.transform);
            int x = invSlot % Player.HOTBAR_SIZE;
            int y = invSlot / Player.HOTBAR_SIZE;

            ((RectTransform)slotHandler.transform).localPosition = new Vector2((40 * x - 40 * 4.5f)*(UI_SCALE+1), (-40+(40*y)) * (UI_SCALE+1));
            slotHandler.transform.localScale = Vector3.one * (UI_SCALE+1);
        }

        // Create inv slot for mouse held item

            GameObject heldSlotHandler = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/MouseHeldSlotDisplay"));
            mouseHeldSlot = heldSlotHandler.GetComponent<MouseHeldSlotDisplay>();
            mouseHeldSlot.Init(this.player); // -1 for mouse held item
            heldSlotHandler.transform.SetParent(inventoryDisplay.transform);
            heldSlotHandler.transform.localScale = Vector3.one * (UI_SCALE + 1);
        

        ShowScreen(Screen.None);
    }
    public void UpdateUI()
    {
        Cursor.visible = UiOpen;
        Cursor.lockState = UiOpen ? CursorLockMode.None : CursorLockMode.Locked;
        lookScript.sensitivity = UiOpen ? 0 : 2;
    }
    public void UpdateUILate()
    {
        if (currentScreen == Screen.Inventory)
        {
            if (player.mouseHeldItem != ItemStack.EMPTY)
            {
                mouseHeldSlot.transform.localPosition = Input.mousePosition - new Vector3(UnityEngine.Screen.width / 2 - 20 * (UI_SCALE + 1), UnityEngine.Screen.height / 2 + 20 * (UI_SCALE + 1));
                mouseHeldSlot.gameObject.SetActive(true);
            }
            else
            {
                mouseHeldSlot.gameObject.SetActive(false);
            }
        }
    }
    public void ShowScreen(Screen screen)
    {
        switch (screen)
        {
            case Screen.None:
                {
                    foreach(InventorySlotDisplay invSlot in invSlots)
                    {
                        invSlot.hovered = false;
                    }
                    hotbarDisplay.SetActive(true);
                    inventoryDisplay.SetActive(false);
                    currentScreen = screen;
                    break;
                }
            case Screen.Inventory:
                {
                    hotbarDisplay.SetActive(false);
                    inventoryDisplay.SetActive(true);
                    currentScreen = screen;
                    break;
                }
        }
    }

}
