using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private string[] possibleNames = { "Thunderbolt", "Vortex", "Blaze", "Phantom", "Inferno", "Velocity", "Eclipse", "Turbo" };
    public CarController[] followerControllers { get;private set; }
    // Start is called before the first frame update
    void Awake()
    {
        List<CarController> followerControllersList = new List<CarController>();

        foreach (Transform tr in transform)
            followerControllersList.Add(tr.GetComponentInChildren<CarController>());

        followerControllers = followerControllersList.ToArray();
        foreach (var car in followerControllers)
        {
            car.CarName = ChooseRandomNameForAI();

        }
    }
    string ChooseRandomNameForAI()
    {
        // Generar un número aleatorio entre 0 y 4 (índices del array)
        int randomIndex = UnityEngine.Random.Range(0, possibleNames.Length);
        string name = possibleNames[randomIndex];
        List<string> list = new List<string>(possibleNames);
        list.Remove(name);
        possibleNames=list.ToArray();
        // Devolver el nombre en el índice aleatorio
        return name;

    }
    public void FinishRaceForAI(CarController carController)
    {
        foreach (var item in followerControllers)
        {
            if (item!= carController)
            {
                item.canDrive = false;

            }
        }
    }
}
