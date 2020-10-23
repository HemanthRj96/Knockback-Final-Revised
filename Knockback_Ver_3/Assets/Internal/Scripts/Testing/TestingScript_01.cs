using Knockback.Utility;
using System.Collections.Generic;
using UnityEngine;


public class TestingScript_01 : MonoBehaviour
{
    string xpString;


    private void ParseValueFromString(out float fValue, out int iValue)
    {
        iValue = int.Parse(xpString.Split('|')[0]);
        fValue = float.Parse(xpString.Split('|')[1]);
    }

    private void Start()
    {
        int level;
        float currentXP;

        xpString = $"{45366}|{385.58323}";

        ParseValueFromString(out currentXP, out level);
        Debug.Log($"Level: {level}, xp: {currentXP}");
    }
}
