using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    WaypointManager manager;
    [SerializeField]
    bool isFinish;
    int numberOfPlayers;
    public int counter { private get;  set; } = 0;
    private void Start()
    {

        manager = GetComponentInParent<WaypointManager>();
        //numberOfPlayers = manager.numberofCars;

    }
    public void ActualizeNumberOfPlayers()
    {
        //numberOfPlayers++;
    }
    private void OnTriggerEnter(Collider other)
    {
        //PlayerOnlineController car = other.GetComponentInParent<PlayerOnlineController>();
        OnlineCarController onlineCarController=other.GetComponentInParent<OnlineCarController>();
        CarController carController = other.GetComponentInParent<CarController>();
        PlayerOnlineController playerOnlineController=other.GetComponentInParent<PlayerOnlineController>();

        if (onlineCarController != null)
        {
            Debug.Log("increasing wp");

            counter++;
            Debug.Log(onlineCarController.nameCar);
            //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //sphere.transform.position = transform.position;
        }
        else if (playerOnlineController != null)
        {

            if (isFinish)
            {
                numberOfPlayers = manager.numberofCars;
                if (playerOnlineController.CheckWaypoint(this))
                {
                    if (counter == numberOfPlayers)
                    {
                        manager.FinishRace(carController);

                    }
                    else
                    {
                        counter++;
                        playerOnlineController.CalculateActualWP();
                        manager.PositionCalc(carController.CarName, carController.actualWP);
                    }
                }

            }
            else
            {
                if (playerOnlineController.CheckWaypoint(this))
                {
                    playerOnlineController.CalculateActualWP();
                    manager.PositionCalc(carController.CarName, carController.actualWP);
                }

            }


        }
        else if (carController != null)
        {

            if (isFinish)
            {
                numberOfPlayers = manager.numberofCars;

                if (counter==numberOfPlayers)
                {
                    manager.FinishRace(carController);

                }
                else
                {
                    counter++;

                    carController.actualWP++;
                    manager.PositionCalc(carController.CarName, carController.actualWP);
                }
            }
            else
            {
                carController.actualWP++;
                manager.PositionCalc(carController.CarName, carController.actualWP);
            }
        }
    }
    public void IncreaseCounterStart()
    {
        counter++;
    }
    public bool GetIsFinish()
    {
        return isFinish;
    }
}
