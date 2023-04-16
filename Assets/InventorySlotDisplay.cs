using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotDisplay : MonoBehaviour
{
    private static Color SelectedColour = Color.white;
    private static Color UnselectedColour = new Color(200/255f, 200/255f, 200/255f);

    private int _slotNum;
    private ItemContainer _inventory;
    private Player _player;
    Image itemSprite;
    Image outline;
    TextMeshProUGUI itemCountText;
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
        outline.color = _player.SelectedSlot == _slotNum ? SelectedColour : UnselectedColour;

        // Display non empty itemstack
        ItemStack displayStack = _inventory.GetStackInSlot(_slotNum);
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
}
