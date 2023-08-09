using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public event EventHandler OnPointerEnterEvent;
    public event EventHandler OnPointerExitEvent;


    [SerializeField] private TextMeshPro coordinatesText;

    private TileObject objectOnThisTile;

    private static Tile selectedTile;
    private static bool showCoordinates;
    private bool myShowCoordinates;

    private int X, Y;

    private void Awake()
    {

    }

    private void Start()
    {
        UpdateText();
    }

    private void Update()
    {
        if(myShowCoordinates != showCoordinates)
        {
            myShowCoordinates = showCoordinates;
            UpdateText();
        }
    }

    #region ObjectOnThisTile

    public bool HasObjectOnThisTile()
    {
        return objectOnThisTile != null;
    }

    public TileObject GetObjectOnThisTile()
    {
        return objectOnThisTile;
    }

    public bool TryGetObjectOnThisTile(out TileObject obj)
    {
        obj = objectOnThisTile;
        return HasObjectOnThisTile();
    }

    public bool TrySetObjectOnThisTile(TileObject objectOnTileBase)
    {
        if(objectOnThisTile != null)
        {
            return false;
        }
        objectOnThisTile = objectOnTileBase;
        return true;
    }

    #endregion

    #region TextXY
    public void SetXY(int x, int y)
    {
        X = x;
        Y = y;
        UpdateText();
    }

    private void UpdateText()
    {
        if (myShowCoordinates)
        {
            coordinatesText.text = $"x:{X};y{Y}";
        }
        else
        {
            coordinatesText.text = "";
        }
    }

    public static void ToggleShowCoordinates() {
        showCoordinates = !showCoordinates;
    }
    #endregion

    #region Pointer
    public void OnPointerEnter(PointerEventData eventData)
    {
        selectedTile = this;
        OnPointerEnterEvent?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (GameManager.Instance.IsPlayerTurn() || true ) //TODO Change that after GameState is implemented
            {
                if (HasObjectOnThisTile())
                {
                    if (objectOnThisTile.IsPlayerType())
                    {
                        GameManager.Instance.SetSelectedObject(objectOnThisTile);
                    }
                    Debug.Log($"Clicked on {objectOnThisTile.name}");
                }
                else
                {
                    GameManager.Instance.SetSelectedObject(null);
                    Debug.Log("Clicked on Empty Tile");
                }
            }
        }
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            // Display Tooltip
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        OnPointerExitEvent?.Invoke(this, EventArgs.Empty);
    }
    #endregion

}