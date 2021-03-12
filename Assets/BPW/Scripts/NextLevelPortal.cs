using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelPortal : MonoBehaviour
{
    private FSM fsm;

    private void Start()
    {
        fsm = FindObjectOfType<FSM>();
    }

    private void OnTriggerEnter(Collider other)
    {
        fsm.GameWinState();
    }
}
