using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneLoader : MonoBehaviour
{
    public static MySceneLoader instance;

    void Awake()
    {
        // Ensure only one instance exists and persists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Simple scene load by string name
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}



