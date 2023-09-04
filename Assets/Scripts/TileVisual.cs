using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileVisual : MonoBehaviour
{
    [SerializeField] private Tile tile;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tile.OnPointerEnterEvent += Tile_OnMouseEnterEvent;
        tile.OnPointerExitEvent += Tile_OnPointerExitEvent;
    }

    private void Tile_OnPointerExitEvent(object sender, System.EventArgs e)
    {
        spriteRenderer.color = Color.white;
    }

    private void Tile_OnMouseEnterEvent(object sender, System.EventArgs e)
    {
        if(!GameManager.Instance.IsSelectedFirstWeapon() && !GameManager.Instance.IsSelectedSecondWeapon())
            spriteRenderer.color = Color.yellow;
    }
}
