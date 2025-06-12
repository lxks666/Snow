using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    
    private PlayerHealth playerHealth;
    private PlayerTask playerTask;

    private void Start()
    {
        InitializePlayerReferences();
    }

    private void InitializePlayerReferences()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerTask = player.GetComponent<PlayerTask>();
        }
        else
        {
            Debug.LogWarning("Player not found in scene!");
        }
    }

    private string SimpleEncrypt(string data)
    {
        char[] chars = data.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = (char)(chars[i] ^ 0x55); 
        }
        return new string(chars);
    }

    // Game over
    public void GameOver(string message)
    {
        PlayerPrefs.SetString("GameOverMessage", message);
        PlayerPrefs.SetString("GameResult", "Defeat"); 

        LoadEndScene(2);
    }

    // Victory
    public void Victory(string message)
    {
        PlayerPrefs.SetString("GameOverMessage", message);
        PlayerPrefs.SetString("GameResult_Enc", SimpleEncrypt("Victory"));

        LoadEndScene(1);
    }

    private void LoadEndScene(int sceneOffset)
    {
        int endSceneIndex = SceneManager.GetActiveScene().buildIndex + sceneOffset;
        if (endSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(endSceneIndex);
        }
        else
        {
            Debug.LogError("The game ending scene has not been added to the Build Settings!");

            SceneManager.LoadScene(0);
        }
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(0); 
    }
}