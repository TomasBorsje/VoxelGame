using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseHeldSlotDisplay : MonoBehaviour
{
    private Player _player;
    Image itemSprite;
    TextMeshProUGUI itemCountText;
    public void Init(Player player)
    {
        _player = player;
    }
    void Start()
    {
        itemSprite = transform.GetChild(0).GetComponent<Image>();
        itemCountText = transform.GetComponentInChildren<TextMeshProUGUI>();
    }
    void Update()
    {
        // Display non empty itemstack
        ItemStack displayStack = _player.mouseHeldItem;
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
