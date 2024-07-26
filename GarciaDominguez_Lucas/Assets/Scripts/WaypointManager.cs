using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Dictionary<string,int> carPositions;
    public int numberofCars {  get;private set; }
    RankingManagement rankingManagement;
    AIManager aiManager;
    PlayerController playerController;
    LogicWinningRace logicWinningRace;
    CircuitMng circuitMng;
    SocketController socketController;
    PersistenceData persistenceData;
    CarDeploymentSystem carDeploymentSystem;
    bool isFinished = false;
    // Start is called before the first frame update
    private void Awake()
    {
        rankingManagement=FindObjectOfType<RankingManagement>();
        aiManager=FindObjectOfType<AIManager>();
        playerController = FindObjectOfType<PlayerController>();
        persistenceData = FindObjectOfType<PersistenceData>();
        socketController = FindObjectOfType<SocketController>();
        carDeploymentSystem = FindObjectOfType<CarDeploymentSystem>();
        logicWinningRace = FindObjectOfType<LogicWinningRace>();
        circuitMng = GetComponent<CircuitMng>();
    }
    void Start()
    {

        if (!persistenceData.IsMultiplayer )
        {
            numberofCars = aiManager.followerControllers.Length;
            carPositions = new Dictionary<string, int>();
            carPositions.Add(playerController.carOnline.onlineName, 0);
            for (int i = 0; i < numberofCars; i++)
            {
                carPositions.Add(aiManager.followerControllers[i].CarName, 0);
            }
            carPositions = OrdenarDiccionarioPorValor(carPositions);

            numberofCars++;
            rankingManagement.EnableUI(numberofCars);
            rankingManagement.SortPositions(carPositions);
        }
        else
        {
            carPositions = new Dictionary<string, int>
            {
                { persistenceData.Name, 0 }
            };
            carPositions = OrdenarDiccionarioPorValor(carPositions);
            numberofCars++;
            rankingManagement.EnableUI(numberofCars);
            rankingManagement.SortPositions(carPositions);
        }

    }
    static Dictionary<string, int> OrdenarDiccionarioPorValor(Dictionary<string, int> diccionario)
    {
        var listaOrdenable = diccionario.ToList();

        listaOrdenable.Sort((x, y) => y.Value.CompareTo(x.Value));

        Dictionary<string, int> diccionarioOrdenado = listaOrdenable.ToDictionary(x => x.Key, x => x.Value);
        return diccionarioOrdenado;
    }
    public void PositionCalc(string name,int waypointActual)
    {
        if (!persistenceData.IsMultiplayer)
        {
            if (carPositions.ContainsKey(name))
            {
                carPositions[name] = waypointActual;
                carPositions = OrdenarDiccionarioPorValor(carPositions);
                rankingManagement.SortPositions(carPositions);
            }
        }
        else
        {
            if (carPositions.ContainsKey(name))
            {
                carPositions[name] = waypointActual;
                carPositions = OrdenarDiccionarioPorValor(carPositions);
                rankingManagement.SortPositions(carPositions);
                socketController.Send("increaseWP|" + name);
            }
        }

    }
    public void PositionCalc(string name)
    {
        if (!persistenceData.IsMultiplayer)
        {
            if (carPositions.ContainsKey(name))
            {
                carPositions[name]++;
                carPositions = OrdenarDiccionarioPorValor(carPositions);
                rankingManagement.SortPositions(carPositions);
            }
        }
        else
        {
            if (carPositions.ContainsKey(name))
            {
                if (carPositions[name]==0)
                {
                    PositionCalc(name,true);

                }
                else
                {               
                    carPositions[name]++;
                    carPositions = OrdenarDiccionarioPorValor(carPositions);
                    rankingManagement.SortPositions(carPositions);
                }

            }
        }

    }
    public void PositionCalc(string name,bool isStartandFinishWP)
    {
        if(isStartandFinishWP)
        {
            circuitMng.ActualizeStart();
            carPositions[name]++;
            carPositions = OrdenarDiccionarioPorValor(carPositions);
            rankingManagement.SortPositions(carPositions);
        }


    }
    public void FinishRace(CarController carController)
    {
        if (!isFinished)
        {
            aiManager.FinishRaceForAI(carController);
            playerController.carPlayer.canDrive = false;
            logicWinningRace.EnableCanvas(carController.CarName,carController.cameraTarget);
            isFinished = true;
            if(persistenceData.IsMultiplayer)
            {
                socketController.Send("finishRace|" + carController.CarName);

            }
        }
    }
    public void FinishRaceOnline(string name)
    {
        if (!isFinished)
        {
            playerController.carPlayer.canDrive = false;
            if (carPositions.ContainsKey(name))
            {
                logicWinningRace.EnableCanvas(name, carDeploymentSystem.ReturnOnlineCarByName(name).cameraTarget);
            }
            isFinished = true;
        }
    }
    public void AddPlayerOnline(string name,int waypoint)
    {
        carPositions.Add(name, waypoint);
        carPositions = OrdenarDiccionarioPorValor(carPositions);
        numberofCars++;
        rankingManagement.RechargeUI(numberofCars);
        rankingManagement.SortPositions(carPositions);
    }

}
