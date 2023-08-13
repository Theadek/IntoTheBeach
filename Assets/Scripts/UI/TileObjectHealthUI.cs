using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileObjectHealthUI : MonoBehaviour
{
    [SerializeField] private TileObject tileObject;
    private TextMeshPro healthText;

    private void Awake()
    {
        healthText = GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        UpdateHealthText();
        tileObject.OnHealthChanged += TileObject_OnHealthChanged;

    }

    private void TileObject_OnHealthChanged(object sender, System.EventArgs e)
    {
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        healthText.text = $"{tileObject.GetCurrentHealth()}/{tileObject.GetMaxHealth()}";
    }
}
