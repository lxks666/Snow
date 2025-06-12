using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    public void StartMenu()
    {
        int targetIndex = SceneManager.GetActiveScene().buildIndex + 2;
        if (targetIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(targetIndex);
        }
        else
        {
            Debug.LogError("Target scene does not exist!");
            SceneManager.LoadScene(0);
        }
    }

    public void Instruction()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
