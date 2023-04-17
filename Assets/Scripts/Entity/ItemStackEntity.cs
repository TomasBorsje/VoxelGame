using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStackEntity : Entity
{
    GameObject modelHolder;
    MeshFilter filter;
    MeshRenderer meshRenderer;
    ItemStack stack;
    Rigidbody rb;
    BoxCollider boxCollider;
    public void Init(ItemStack item)
    {
        stack = item;
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = Vector3.one * 0.3f;
        rb = gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Create empty gameobject to hold mesh
        modelHolder = new GameObject();
        modelHolder.transform.SetParent(this.transform);
        modelHolder.transform.localPosition = Vector3.zero;
        modelHolder.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        filter = modelHolder.AddComponent<MeshFilter>();
        meshRenderer = modelHolder.AddComponent<MeshRenderer>();

        // Get stack and populate model holder
        ItemModelManager.Instance.PopulateMeshRendererBlockItem(
            (BlockItem)stack.Item, 
            filter, 
            meshRenderer);
    }
    protected override void Update()
    {
        base.Update();
        // animate model holder!
        modelHolder.transform.localPosition = new Vector3(0, (Mathf.Sin(age) + 1) / 5.5f + 0.25f, 0);
        modelHolder.transform.Rotate(0, Time.deltaTime * 120f, 0);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Player>() != null)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            stack = player.Inventory.AddStack(stack);
            if(stack == ItemStack.EMPTY)
            {
                Destroy(this.gameObject);
                return;
            }
        }
        else if (collision.gameObject.GetComponent<ItemStackEntity>() != null)
        {
            // If another object is falling onto us, ignore. It will delete itself (as it will collide with us too)
            if (collision.gameObject.transform.position.y > transform.position.y) { return; }

            ItemStackEntity otherItemEntity = collision.gameObject.GetComponent<ItemStackEntity>();
            // Try merge and delete other if we merge
            stack = otherItemEntity.stack.Merge(stack);
            if (stack == ItemStack.EMPTY)
            {
                Destroy(this.gameObject);
                return;
            }
        }
    }
    public static GameObject CreateItemStackEntity(Vector3 pos, ItemStack stack)
    {
        GameObject gameObject = new GameObject(stack.Item.Id);
        gameObject.transform.position = pos;
        ItemStackEntity comp = gameObject.AddComponent<ItemStackEntity>();
        comp.Init(stack);
        return gameObject;
    }
}
