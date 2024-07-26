using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
[RequireComponent(typeof(SocketController))]
public class OnlineManager : MonoBehaviour
{
    private SocketController socket;
    private CarDeploymentSystem carDeploymentSystem;
    public PlayerOnlineController playerOnline {  get;private  set; }
    private WaypointManager waypointManager;
    private string instanceName;
    private Dictionary<string,OnlineCarController> cars = new Dictionary<string,OnlineCarController>();
    [SerializeField]
    GameObject onlineCarPrefab;
    bool isAssigned = false;
    // Start is called before the first frame update
    void Awake()
    {
        socket=GetComponent<SocketController>();   
        playerOnline=FindObjectOfType<PlayerOnlineController>();
        carDeploymentSystem=FindObjectOfType<CarDeploymentSystem>();
        waypointManager=FindObjectOfType<WaypointManager>();
    }
    public void Join()
    {
        instanceName = (playerOnline.onlineName +
        playerOnline.GetHashCode());
        socket.Send("join|" + instanceName+"|"+ playerOnline.onlineName);        
    }
    public void ParseMessages(byte[] iBuffer,int bytesReceived)
    {
        byte[] strBuffer=new byte[bytesReceived];
        Buffer.BlockCopy(iBuffer,0,strBuffer,0,
            bytesReceived);
        string str=Encoding.ASCII.GetString(iBuffer);
        string[] messages=str.Split('$');
        foreach(string message in messages)
        {
            ParseMessage(message);
        }
    }
    private void ParseMessage(string message)
    {
        string[] parameters = message.Split('|');
        switch (parameters[0])
        {
            case "startRace":
                carDeploymentSystem.StartRace();
                break;
            case "increaseWP":
                waypointManager.PositionCalc(parameters[1]);
                break;
            case "finishRace":
                waypointManager.FinishRaceOnline(parameters[1]);
                break;
            case "join":
                //add a new online car
                if (cars.ContainsKey(parameters[1]))
                {
                    Debug.LogWarning($"Player {parameters[1]} is trying to enter but is already in the scene D:");
                }
                else
                {
                    if (!isAssigned)
                    {
                        int value = Convert.ToInt32(parameters[3]);

                        playerOnline.transform.position = carDeploymentSystem.positionsForCars[value].transform.position;
                        playerOnline.position = Convert.ToInt32(parameters[3]);
                        isAssigned = true;
                    }
                    else if (Convert.ToInt32(parameters[3])!=playerOnline.position &&isAssigned)
                    {
                        GameObject newPlayer = GameObject.Instantiate
                        (onlineCarPrefab);
                        newPlayer.name = parameters[1];
                        cars.Add(parameters[1], newPlayer.GetComponent<OnlineCarController>());
                        newPlayer.GetComponent<OnlineCarController>().nameCar = parameters[1];
                        carDeploymentSystem.Join(newPlayer.GetComponent<OnlineCarController>());
                        string output = Regex.Replace(newPlayer.name, @"[\d-]", string.Empty);
                        waypointManager.AddPlayerOnline(output, 0);
                    }


                    socket.Send("join|" + instanceName + "|" + parameters[3]);
                }
                break;
            case "updatePosition":
                if (!cars.ContainsKey(parameters[1]))
                {
                    Debug.LogWarning($"Unable to update position of {parameters[1]} key not found");
                }
                else
                {
                    OnlineCarController car;
                    cars.TryGetValue(parameters[1], out car);
                    car.targetPosition=StringToVector3(parameters[2]);
                }
                break;
            case "updateRotation":
                if (!cars.ContainsKey(parameters[1]))
                {
                    Debug.LogWarning($"Unable to update rotation of {parameters[1]} key not found");
                }
                else
                {
                    OnlineCarController car;
                    cars.TryGetValue(parameters[1], out car);
                    car.targetRotation.eulerAngles = StringToVector3(parameters[2]);
                }
                break;
            case "updateWheelFL":
                if (!cars.ContainsKey(parameters[1]))
                {
                    Debug.LogWarning($"Unable to update rotation of {parameters[1]} key not found");
                }
                else
                {
                    OnlineCarController car;
                    cars.TryGetValue(parameters[1], out car);
                    car.targetRotationupdateRotationFLWheel.eulerAngles = StringToVector3(parameters[2]);
                }
                break;
            case "updateWheelFR":
                if (!cars.ContainsKey(parameters[1]))
                {
                    Debug.LogWarning($"Unable to update rotation of {parameters[1]} key not found");
                }
                else
                {
                    OnlineCarController car;
                    cars.TryGetValue(parameters[1], out car);
                    car.targetRotationupdateRotationFRWheel.eulerAngles = StringToVector3(parameters[2]);
                }
                break;
            case "updateWheelBR":
                if (!cars.ContainsKey(parameters[1]))
                {
                    Debug.LogWarning($"Unable to update rotation of {parameters[1]} key not found");
                }
                else
                {
                    OnlineCarController car;
                    cars.TryGetValue(parameters[1], out car);
                    car.targetRotationupdateRotationBRWheel.eulerAngles = StringToVector3(parameters[2]);
                }
                break;
            case "updateWheelBL":
                if (!cars.ContainsKey(parameters[1]))
                {
                    Debug.LogWarning($"Unable to update rotation of {parameters[1]} key not found");
                }
                else
                {
                    OnlineCarController car;
                    cars.TryGetValue(parameters[1], out car);
                    car.targetRotationupdateRotationBLWheel.eulerAngles = StringToVector3(parameters[2]);
                }
                break;
        }
    }

    public void UpdatePosition()
    {
        socket.Send("updatePosition|" + instanceName + "|" +playerOnline.transform.position);
    }

    public void UpdateRotation()
    {
        socket.Send("updateRotation|" + instanceName + "|"+
            playerOnline.transform.rotation.eulerAngles);
    }
    public void UpdateRotationWheels()
    {
        socket.Send("updateWheelFL|" + instanceName + "|" +
            playerOnline.wheelFLmesh.transform.rotation.eulerAngles);
        socket.Send("updateWheelFR|" + instanceName + "|" +
            playerOnline.wheelFRmesh.transform.rotation.eulerAngles);
        socket.Send("updateWheelBR|" + instanceName + "|" +
            playerOnline.wheelBRmesh.transform.rotation.eulerAngles);
        socket.Send("updateWheelBL|" + instanceName + "|" +
            playerOnline.wheelBLmesh.transform.rotation.eulerAngles);
    }
    private static Vector3 StringToVector3(string strVector)
    {
        // Remove the parentheses
        if (strVector.StartsWith("(") && strVector.EndsWith(")"))
            strVector = strVector.Substring(1, strVector.Length - 2);

        // split the items
        string[] sArray = strVector.Split(',');

        return new Vector3(
            float.Parse(sArray[0], CultureInfo.InvariantCulture.NumberFormat),
            float.Parse(sArray[1], CultureInfo.InvariantCulture.NumberFormat),
            float.Parse(sArray[2], CultureInfo.InvariantCulture.NumberFormat)
        );
    }

}
