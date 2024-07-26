using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountDownStartRace : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI countDownStart;
    CarDeploymentSystem carDeploymentSystem;
    private float currentTime = 10f;

    void Awake()
    {
        carDeploymentSystem=FindObjectOfType<CarDeploymentSystem>();
        if (carDeploymentSystem)
            carDeploymentSystem.OnStartRace += EnableRace;
        this.gameObject.SetActive(false);
    }
    void EnableRace()
    {
        this.gameObject.SetActive(true);
        StartCoroutine(StartCountdown());
    }
    IEnumerator StartCountdown()
    {
        while (currentTime > 0)
        {
            int seconds = Mathf.RoundToInt(currentTime);
            countDownStart.text = seconds.ToString();
            yield return new WaitForSeconds(1f);
            currentTime--;
        }
        this.gameObject.SetActive(false);
        carDeploymentSystem.SetStatusCars(true);

    }
}
