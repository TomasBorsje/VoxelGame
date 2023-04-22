using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingRecipeDisplay : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private static Color HoveredColour = new Color(225 / 255f, 225 / 255f, 225 / 255f);
    private static Color NotHoveredColour = new Color(200 / 255f, 200 / 255f, 200 / 255f);

    private int _recipeNum; // If slotnum is -1, this will display the mouse held item!!
    private Player _player;
    private CraftingRecipe recipe = null;
    Image itemSprite;
    Image outline;
    TextMeshProUGUI itemCountText;

    public bool hovered;
    // -1 to show mouse held itemstack
    public void Init(Player player, int recipeNum)
    {
        _player = player;
        _recipeNum = recipeNum;
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
        if (_player.GetCraftableRecipes().Count > _recipeNum)
        {
            recipe = _player.GetCraftableRecipes()[_recipeNum];
            // Show ourselves
            itemSprite.sprite = Sprite.Create(TextureAtlas.Instance.atlasTex,
                TextureAtlas.Instance.GetRect(recipe.output.Id),
                Vector2.zero);
            itemSprite.color = Color.white;
            itemCountText.text = recipe.outputCount > 1 ? recipe.outputCount.ToString() : "";
        }
        else
        {
            recipe = null;
            itemSprite.color = Color.clear;
            itemCountText.text = "";
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // On left click, craft into mouse held stack!
        if (eventData.button == PointerEventData.InputButton.Left && recipe != null)
        {
            // If we are able to hold the entire output in mouse held stack, craft!

            if(_player.mouseHeldItem == ItemStack.EMPTY)
            {
                _player.mouseHeldItem = recipe.TryCraft(_player.Inventory);
            }
            else if (_player.mouseHeldItem.CanMerge(recipe.GetOutput()))
            {
                ItemStack output = recipe.TryCraft(_player.Inventory);
                _player.mouseHeldItem.Merge(output);
            }
        }
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
