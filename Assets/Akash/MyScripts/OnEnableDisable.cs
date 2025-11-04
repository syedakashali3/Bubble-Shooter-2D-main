using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnableDisable : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onEnabled;
    public UnityEvent onDisabled;

    private void OnEnable()
    {
        onEnabled?.Invoke();
    }

    private void OnDisable()
    {
        onDisabled?.Invoke();
    }
}
