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
    public static event EventHandler<OnAnyPointerEnterEventArgs> OnAnyPointerEnterEvent;
    public class OnAnyPointerEnterEventArgs : EventArgs
    {
        public Vector2Int _xy;
    }


    [SerializeField] private TextMeshPro coordinatesText;

    private TileObject objectOnThisTile;

    private static Tile selectedTile;
    private static bool showCoordinates;
    private bool myShowCoordinates;

    private Vector2Int XY;

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

    #region TileObject

    public bool HasObjectOnThisTile()
    {
        return objectOnThisTile != null;
    }

    public TileObject GetTileObject()
    {
        return objectOnThisTile;
    }

    public bool TryGetTileObject(out TileObject obj)
    {
        obj = objectOnThisTile;
        return HasObjectOnThisTile();
    }

    public void RemoveTileObject()
    {
        objectOnThisTile = null;
    }



    public bool TrySetTileObject(TileObject tileObject)
    {
        if(objectOnThisTile != null)
        {
            return false;
        }
        tileObject.transform.position = transform.position;
        tileObject.transform.parent = transform;
        tileObject.GetTile().RemoveTileObject();
        tileObject.SetTile(this);
        objectOnThisTile = tileObject;
        return true;
    }

    public bool TrySetTileObjectWithoutMoving(TileObject tileObject)
    {
        if (objectOnThisTile != null)
        {
            return false;
        }
        tileObject.transform.parent = transform;
        tileObject.GetTile().RemoveTileObject();
        tileObject.SetTile(this);
        objectOnThisTile = tileObject;
        return true;
    }

    #endregion

    #region XY

    public void SetXY(Vector2Int xy)
    {
        XY = xy;
        UpdateText();
    }

    public void SetXY(int x, int y)
    {
        XY.x = x;
        XY.y = y;
        UpdateText();
    }

    public Vector2Int GetXY()
    {
        return XY;
    }


    private void UpdateText()
    {
        if (myShowCoordinates)
        {
            coordinatesText.text = $"x:{XY.x};y{XY.y}";
        }
        else
        {
            coordinatesText.text = "";
        }
    }
    #endregion

    public static void ToggleShowCoordinates() {
        showCoordinates = !showCoordinates;
    }

    #region Pointer
    public void OnPointerEnter(PointerEventData eventData)
    {
        selectedTile = this;
        OnPointerEnterEvent?.Invoke(this, EventArgs.Empty);
        OnAnyPointerEnterEvent?.Invoke(this, new OnAnyPointerEnterEventArgs
        {
            _xy = this.XY
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameManager.Instance.TileLeftClicked(this);
        }
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            GameManager.Instance.TileRightClicked(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        OnPointerExitEvent?.Invoke(this, EventArgs.Empty);
    }
    #endregion

}