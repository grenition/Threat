using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasController : MonoBehaviour
{
    [SerializeField] private GameObject[] closeWhilePcOpen;
    private bool lastOpenedState = false;
    void Update()
    {
        if(lastOpenedState != ComputerAnimator._isSomeWindowOpened)
        {
            SetObjectsActive(!ComputerAnimator._isSomeWindowOpened);
        }
        lastOpenedState = ComputerAnimator._isSomeWindowOpened;
    }

    public void SetObjectsActive(bool activeState)
    {
        foreach(var obj in closeWhilePcOpen)
        {
            obj.SetActive(activeState);
        }
    }
}
