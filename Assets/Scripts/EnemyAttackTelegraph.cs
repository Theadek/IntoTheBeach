using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTelegraph : MonoBehaviour
{
    [SerializeField] EnemyAI enemyAI;
    private void Start()
    {
        enemyAI.OnEnemyAttackChanged += EnemyAI_OnEnemyAtackChanged;
    }

    private void EnemyAI_OnEnemyAtackChanged(object sender, System.EventArgs e)
    {
        ClearHighlight();
        if (enemyAI.GetTile() == null || enemyAI.GetWeapon() == null)
        {
            return;
        }

        enemyAI.GetWeapon().DisplayEffectOnTile(enemyAI.GetTileObject().GetTile(), enemyAI.GetTile(), transform);
    }

    private void ClearHighlight()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
