using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class CarDeploymentSystem : MonoBehaviour
{
    PersistenceData data;


    int maxNumberOfPlayers = 5;
    PlayerOnlineController onlineController;
    AIManager manager;
    OnlineManager onlineManager;
    [SerializeField]
    GameObject positionsForCarsGO;
    [SerializeField]
    GameObject waitingForPlayersGO;
    public GameObject[] positionsForCars { get;private set; }
    public event Action OnStartRace;
    List<OnlineCarController> onlineCarControllers = new List<OnlineCarController>();
    [SerializeField]
    int numberOfPlayersOnline = 0;
    private void Awake()
    {
        List<GameObject> waypointList = new List<GameObject>();

        foreach (Transform tr in positionsForCarsGO.transform)
            waypointList.Add(tr.gameObject);

        positionsForCars = waypointList.ToArray();
        data = FindObjectOfType<PersistenceData>();
        onlineController = FindObjectOfType<PlayerOnlineController>();
        onlineManager = FindObjectOfType<OnlineManager>();
        manager = FindObjectOfType<AIManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(data.IsMultiplayer)
        {
            SetMultiPlayerMode();
        }
        else
        {
            SetSinglePlayerMode();

        }
    }
    void SetSinglePlayerMode()
    {
        onlineController.gameObject.transform.position = positionsForCars[0].transform.position;
        for (int i = 1, j=0; i < positionsForCars.Length; i++, j++)
        {
            manager.followerControllers[j].transform.position = positionsForCars[i].transform.position;
        }
        SetStatusCars(false);

        OnStartRace.Invoke();
    }
    void SetMultiPlayerMode()
    {
        SetStatusCars(false);
        for (int j = 0; j < manager.followerControllers.Length;  j++)
        {
            manager.followerControllers[j].gameObject.SetActive(false);
        }
        numberOfPlayersOnline++;
        //StartCoroutine( WaitingForPlayers());
    }
    public void StartRace()
    {
        waitingForPlayersGO.SetActive(false);
        OnStartRace.Invoke();

    }
    public void Join(OnlineCarController newPlayer)
    {
        if (numberOfPlayersOnline<5)
        {
            onlineCarControllers.Add(newPlayer);
            numberOfPlayersOnline++;
        }
    }
    public OnlineCarController ReturnOnlineCarByName(string name)
    {
        for (int i = 0; i < onlineCarControllers.Count; i++)
        {
            if (onlineCarControllers[i].name.Contains(name))
            {
                return onlineCarControllers[i];
            }
            
        }
        return null;
    }
    public void SetStatusCars(bool status)
    {
        onlineController.GetComponent<CarController>().canDrive = status;
        for (int i = 1, j = 0; i < positionsForCars.Length; i++, j++)
        {
            manager.followerControllers[j].canDrive = status;
        }

    }
}
