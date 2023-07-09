using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    private static EnemySpawnManager instance;

    public static void ActiveEnemy(int index)
    {
        instance.presets[index].SetActive(true);
    }

    public static void DisableEnemy(int index)
    {
        instance.presets[index].SetActive(false);
    }

    [SerializeField] private GameObject[] presets;

    private void Awake()
    {
        instance = this;
    }
}