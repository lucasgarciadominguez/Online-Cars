using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneManagment : MonoBehaviour  // lo uso tanto para desbloquear niveles como para cargarlos, como para volver al menu desde el juego
{
    [SerializeField]
    GameObject fadeGO;
    [SerializeField]
    float delayTime=1.5f;
    public void ChangeLevel(int level)
    {
        fadeGO.SetActive(true);
        StartCoroutine(TransitionsLevel(level));
    }
    public void GoBackToMenu()
    {
        ChangeLevel(0);
    }
    IEnumerator TransitionsLevel(int level)
    {
        yield return new WaitForSeconds(delayTime);

        switch (level)
        {
            case 0:
                SceneManager.LoadScene("MenuGame");
                break;
            case 1:
                SceneManager.LoadScene("SingleplayerPlayerSelection");
                break;
            case 2:
                SceneManager.LoadScene("MultiplayerPlayerSelection");
                break;
            case 3:
                SceneManager.LoadScene("MultiplayerSelection");
                break;
            case 4:
                //SceneManager.LoadScene("TestScene");

                SceneManager.LoadScene("MultiPlayer");
                break;
            case 5:
                SceneManager.LoadScene("SinglePlayer");
                break;
            case 10:
                Application.Quit();
                break;
            default:
                break;
        }
    }
}
