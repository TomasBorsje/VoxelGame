using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotDisplay : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private static Color HoveredColour = new Color(225 / 255f, 225 / 255f, 225 / 255f);
    private static Color NotHoveredColour = new Color(200/255f, 200/255f, 200/255f);

    private int _slotNum; // If slotnum is -1, this will display the mouse held item!!
    private ItemContainer _inventory;
    private Player _player;
    Image itemSprite;
    Image outline;
    TextMeshProUGUI itemCountText;
    public bool hovered;
    // -1 to show mouse held itemstack
    public void Init(Player player, int slotNum)
    {
        _player = player;
        _inventory = _player.Inventory;
        _slotNum = slotNum;
    }
    void Start()
    {
        outline = transform.GetChild(0).GetComponent<Image>();
        itemSprite = transform.GetChild(2).GetComponent<Image>();
        itemCountText = transform.GetComponentInChildren<TextMeshProUGUI>();
    }
    void Update()
    {
        // Highlight selected slot
        outline.color = hovered ? HoveredColour : NotHoveredColour;

        // Display non empty itemstack
        ItemStack displayStack = _slotNum == -1 ? _player.mouseHeldItem :_inventory.GetStackInSlot(_slotNum);
        if (displayStack != ItemStack.EMPTY)
        {
            itemCountText.text = displayStack.Count.ToString();
            itemSprite.sprite = Sprite.Create(TextureAtlas.Instance.atlasTex, 
                TextureAtlas.Instance.GetRect(displayStack.Item.Id), 
                Vector2.zero);
            itemSprite.color = Color.white;
        }
        else
        {
            itemCountText.text = "";
            itemSprite.color = Color.clear;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_player.mouseHeldItem == ItemStack.EMPTY)
        {
            _player.mouseHeldItem = _inventory.GetStackInSlot(_slotNum);
            _inventory.SetStackInSlot(_slotNum, ItemStack.EMPTY);
            return;
        }
        if (_inventory.GetStackInSlot(_slotNum) == ItemStack.EMPTY)
        {
            _inventory.SetStackInSlot(_slotNum, _player.mouseHeldItem);
            _player.mouseHeldItem = ItemStack.EMPTY;
            return;
        }
        int countBefore = _player.mouseHeldItem.Count;
        _player.mouseHeldItem = _inventory.GetStackInSlot(_slotNum).Merge(_player.mouseHeldItem);
        if(_player.mouseHeldItem.Count == countBefore)
        {
            // Swap
            ItemStack temp = _inventory.GetStackInSlot(_slotNum);
            _inventory.SetStackInSlot(_slotNum, _player.mouseHeldItem);
            _player.mouseHeldItem = temp;
        }
        // Otherwise we merged
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }
}
