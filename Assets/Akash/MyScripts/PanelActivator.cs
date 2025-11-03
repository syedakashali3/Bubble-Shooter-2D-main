using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelActivator : MonoBehaviour
{
    public GameObject[] panels;

    public void ActivatePanel(string name)
    {
        foreach (GameObject panel in panels)
        {
            if (panel != null)
                panel.SetActive(panel.name == name);
        }
    }
}
