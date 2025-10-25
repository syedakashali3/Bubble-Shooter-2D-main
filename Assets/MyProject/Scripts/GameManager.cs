using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagmenet;


public class GameManager : MonoBehaviour {
    
    public void AppReload()
    {
        if (MySceneLoader.instance)
        {
            MySceneLoader.instance.LoadScene("ClassicMode")
        }
        else
        {
            
        }
        Application.LoadLevel(0);
    }
     
    public void Debug()
    {
        print("debug");
    }
}
