using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceData : MonoBehaviour
{
    private static PersistenceData _instance;
    public static PersistenceData Instance => _instance;
    [SerializeField]
    private bool isMultiplayer=false;

    [SerializeField]
    private string namePlayer = "";
    [SerializeField]
    private string ip = "";
    [SerializeField]
    private int port;
    public bool IsMultiplayer
    {
        get { return isMultiplayer; }
        private set { isMultiplayer = value; }
    }
    public string Name
    {
        get { return namePlayer; }
        private set { namePlayer = value; }
    }

    public string IP
    {
        get { return ip; }
        private set { ip = value; }
    }

    public int Port
    {
        get { return port; }
         set { port = value; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void SetIsMultiplayer(bool ismultiplayer) => IsMultiplayer = ismultiplayer;
    public void SetName(string name) => Name = name;

    public void SetConnection(string ip, string port)
    {
        IP = ip;
        bool valid1 = int.TryParse(port, out int result);
        if (valid1)
            Port = result;
        else
            Port = 666;
    }

    public string GetConnectionInfo()
    {
        return $"IP: {IP}, Port: {Port} , Name: {Name}";
    }
}
