using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    public string gameSeed = "Default";
    public int currentSeed = 0;

    private void Awake()
    {
        gameSeed = PlayerPrefs.GetString("Seed", "Default");
        currentSeed = gameSeed.GetHashCode();
        Random.InitState(currentSeed);
    }
}
