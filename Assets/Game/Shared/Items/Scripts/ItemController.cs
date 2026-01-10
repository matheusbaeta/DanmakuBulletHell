using UnityEngine;
public enum ItemType
{
    Health = 0,
    PowerUp = 1
}

public class ItemController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public ItemType itemType;

    public void SetItem(ItemType newType)
    {
        itemType = newType;
        spriteRenderer.sprite = sprites[(int)itemType];
    }
}
