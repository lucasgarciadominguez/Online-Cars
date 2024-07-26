using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LoadData : MonoBehaviour
{
    PersistenceData data;
    [SerializeField]
    TMP_InputField nameText;
    [SerializeField]
    TMP_InputField ipText;
    [SerializeField]
    TMP_InputField portText;
    [SerializeField]
    bool isMultiplayer;
    // Start is called before the first frame update
    void Start()
    {
        data=FindObjectOfType<PersistenceData>();
        data.SetIsMultiplayer(isMultiplayer);
    }
    public void LoadName()
    {
        data.SetName(nameText.text);
    }
    public void LoadIPandPort()
    {
        data.SetConnection(ipText.text,portText.text);      
    }
}
