using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Tile : MonoBehaviour
{
    [SerializeField] private TextMeshPro coordinatesText;
    private static Tile selectedTile;

    private static bool showCoordinates;
    private bool myShowCoordinates;

    private int X, Y;


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

    private void OnMouseOver()
    {

    }

    private void OnMouseEnter()
    {

    }

    private void OnMouseExit()
    {

    }
}
