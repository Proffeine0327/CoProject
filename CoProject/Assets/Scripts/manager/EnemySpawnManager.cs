using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    private static EnemySpawnManager instance;

    public static void ActiveEnemy(int index)
    {
        foreach(var enemy in instance.preset[index].EnemyList) enemy.SetActive(true);
    }

    public static void DisableEnemy(int index)
    {
        foreach(var enemy in instance.preset[index].EnemyList) enemy.SetActive(false);
    }

    [SerializeField] private EnemyGroup[] preset;

    private void Awake()
    {
        instance = this;
    }
}

[System.Serializable]
public class EnemyGroup
{
    [SerializeField] private GameObject[] enemyList;
    public GameObject[] EnemyList => enemyList;
}