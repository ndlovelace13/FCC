using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] TMP_Text bossName;
    [SerializeField] RectTransform healthFill;
    [SerializeField] RectMask2D healthMask;

    EnemyBehavior currentBoss;

    // Start is called before the first frame update
    void Start()
    {
        currentBoss = GameObject.FindWithTag("boss").GetComponent<EnemyBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBoss != null)
        {
            bossName.text = currentBoss.health + " / " + currentBoss.maxHealth;
        }
    }
}
