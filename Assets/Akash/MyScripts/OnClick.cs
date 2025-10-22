using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClick : MonoBehaviour
{
   public void onClickSceneLoad(string sceneName)
    {
        MySceneLoader.instance.LoadScene(sceneName);
    }
}
