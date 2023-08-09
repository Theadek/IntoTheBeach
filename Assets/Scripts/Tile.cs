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

    public bool TrySetObjectOnThisTile(TileObject objectOnTileBase)
    {
        if(objectOnThisTile != null)
        {
            return false;
        }
        objectOnThisTile = objectOnTileBase;
        return true;
    }

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
                GameManager.Instance.SetSelectedObject(objectOnThisTile);
                Debug.Log($"Clicked on {objectOnThisTile}");
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