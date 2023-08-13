using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class TileObjectSelectedVisual : MonoBehaviour
{
    [SerializeField] private GameObject highlightPrefab;

    private void Start()
    {
        GameManager.Instance.OnSelectedObjectChanged += GameManager_OnSelectedObject;
    }

    private void GameManager_OnSelectedObject(object sender, System.EventArgs e)
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        if(GameManager.Instance.TryGetSelectedObject(out TileObject tileObject))
        {
            var position = TileManager.Instance.GetGridPosition(tileObject.GetTile().GetXY());
            Instantiate(highlightPrefab, position, Quaternion.identity, transform);
        }
    }
}
