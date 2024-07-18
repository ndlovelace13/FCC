using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpecialStats : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public TMP_Text titleText;
    [SerializeField] public TMP_Text valueText;
    [SerializeField] public TMP_Text unitsText;

    string statName;
    float statValue;
    string units;

    public SpecialStats(string statName, float statValue, string units)
    {
        this.statName = statName;
        this.statValue = statValue;
        this.units = units;
    }

    public void TransferData(SpecialStats transferData)
    {
        statName = transferData.statName;
        statValue = transferData.statValue;
        units = transferData.units;

        titleText.text = statName;
        valueText.text = statValue.ToString();
        unitsText.text = units;

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
