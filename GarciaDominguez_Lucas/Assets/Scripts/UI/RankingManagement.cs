using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static System.Net.Mime.MediaTypeNames;

public class RankingManagement : MonoBehaviour
{
    [SerializeField]
    List<GameObject> positionstextGO = new List<GameObject>();
    List<TextMeshProUGUI> positionstext = new List<TextMeshProUGUI>();

    public void SortPositions(Dictionary<string, int> carPositions)
    {
        int i = 0;
        foreach (string clave in carPositions.Keys)
        {
            positionstext[i].text = clave;
            i++;
        }
    }
    public void EnableUI(int numberofCars)
    {
        foreach (var text in positionstextGO)
        {
            text.SetActive( false);
        }
        for (int i = 0; i < numberofCars; i++)
        {
            positionstextGO[i].SetActive(true);
        }
        for (int i = 0; i < numberofCars; i++)
        {
            positionstext.Add(positionstextGO[i].GetComponentInChildren<TextMeshProUGUI>());
        }
    }
    public void RechargeUI(int numberofCars)
    {
        positionstext=new List<TextMeshProUGUI>();  
        for (int i = 0; i < numberofCars; i++)
        {
            positionstextGO[i].SetActive(true);
        }
        for (int i = 0; i < numberofCars; i++)
        {
            positionstext.Add(positionstextGO[i].GetComponentInChildren<TextMeshProUGUI>());
        }
    }
}
