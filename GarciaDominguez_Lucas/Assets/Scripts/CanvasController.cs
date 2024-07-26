using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField]
    TMP_Text velocimeter;
    [SerializeField]
    CarController car;

    void FixedUpdate()
    {
        velocimeter.text = (car.rb.velocity.magnitude * 3.6).ToString("F0") + "Km/h";
    }
}
