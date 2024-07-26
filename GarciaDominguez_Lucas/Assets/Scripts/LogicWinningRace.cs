using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogicWinningRace : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI winnerTXT;
    CameraController controllerCamera;
    private void Awake()
    {
        controllerCamera = Camera.main.gameObject.GetComponent<CameraController>();
    }
    private void Start()
    {
        this.gameObject.SetActive(false);

    }
    public void EnableCanvas(string name,GameObject cameraTarget)
    {
        this.gameObject.SetActive(true);
        winnerTXT.text = name;
        controllerCamera.SetNewTargetToFollow(cameraTarget.transform);

    }
}
