using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{

    [Header("Prefabs")]
    public GameObject[] enemyPrefab;
    public GameObject SpawnerStatsPanel;
    private ColliderCheck colliderCheck;
    public TextMeshProUGUI enemyCountText;

    [Header("Settings")]
    public int enemyCountMin;
    public int enemyCountMax;
    public float timeBetweenSpawns;
    public float minSpawnDistanceFromPlayer;
    private int enemiesCountAlive = 0;
    private bool alreadySpawned = false;
    private int totalEnemies;
    public int eventID;
    public Room room;

    private void Start()
    {
        colliderCheck = GetComponent<ColliderCheck>();
    }

    private void Update()
    {
        if (!alreadySpawned)
        {
            SpawnEnemies();
        }

        if (colliderCheck.CheckPlayerInCollider())
        {
            SpawnerStatsPanel.SetActive(true);
        }
        else
        {
            SpawnerStatsPanel.SetActive(false);
        }

        DisplaySpawnerStats();
    }

    public void DisplaySpawnerStats()
    {
        if (SpawnerStatsPanel.activeSelf)
        {
            if (totalEnemies != 0)
            {
                enemyCountText.text = enemiesCountAlive.ToString();
            }
            else
            {
                enemyCountText.text = "Room Cleared";
            }
        }
    }

    public void SpawnEnemies()
    {
        if (!colliderCheck.CheckPlayerInCollider())
        {
            return;
        }

        alreadySpawned = true;
        totalEnemies = Random.Range(enemyCountMin, enemyCountMax);
        StartCoroutine(MyCounter(totalEnemies));
    }

    IEnumerator MyCounter(int number)
    {
        for (int i = 0; i < number; i++)
        {
            float randomX = Random.Range(room.position.x, room.position.x + room.size.x);
            float randomY = Random.Range(room.position.y, room.position.y + room.size.y);
            Vector3 playerPos = GameManager.Instance.playerObject.transform.position;
            float dist = Vector3.Distance(new Vector3(randomX, 0.3f, randomY), new Vector3(playerPos.x, 0.3f, playerPos.y));

            if (dist < minSpawnDistanceFromPlayer)
            {
                randomX = Random.Range(room.position.x, room.position.x + room.size.x);
                randomY = Random.Range(room.position.y, room.position.y + room.size.y);
            }

            int randomEnemy = Random.Range(0, enemyPrefab.Length);
            GameObject enemy = Instantiate(enemyPrefab[randomEnemy], new Vector3(randomX, 0.3f, randomY), Quaternion.identity);
            enemiesCountAlive++;
            enemy.GetComponent<EnemyAI>().OnEnemyDead += EnemyDied;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    public void EnemyDied()
    {
        enemiesCountAlive--;
        totalEnemies--;

        if (totalEnemies == 0)
        {
            colliderCheck.enemiesDefeated = true;

            if (GameManager.Instance.activeQuest != null && GameManager.Instance.activeQuest.isActive)
            {
                GameManager.Instance.activeQuest.goal.EventHappened(eventID);
            }
        }

    }
}
