using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject bossPrefab;
    public GameObject SpawnerStatsPanel;
    private ColliderCheck colliderCheck;
    public TextMeshProUGUI enemyCountText;

    [Header("Settings")]
    public float timeBetweenWaves;
    public float minSpawnDistanceFromPlayer;
    private int enemiesCountAlive = 0;
    private bool alreadySpawned = false;
    private int totalEnemies;
    private bool wavesDefeated = false;
    private bool bossDefeated = false;
    public int eventID;
    public int bossEventID;
    public Room room;
    public int waveIndex = 0;
    public List<Waves> waves = new List<Waves>();
    public GameObject NextLevelPortal;
    public GameObject[] reward;

    private void Start()
    {
        colliderCheck = GetComponent<ColliderCheck>();
    }

    private void Update()
    {
        if (!alreadySpawned)
        {
            SpawnWave(waves[waveIndex]);
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
            if (!wavesDefeated)
            {
                if (totalEnemies != 0)
                {
                    enemyCountText.text = enemiesCountAlive.ToString();
                }
                else
                {
                    enemyCountText.text = "Wave Cleared!";
                }
            }
            else if (wavesDefeated && !bossDefeated)
            {
                enemyCountText.text = "Boss Alive";
            }
            else if (wavesDefeated && bossDefeated)
            {
                enemyCountText.text = "Boss Defeated";
            }
        }
    }

    public void SpawnWave(Waves wave)
    {
        if (!colliderCheck.CheckPlayerInCollider())
        {
            return;
        }

        alreadySpawned = true;
        totalEnemies = Random.Range(wave.enemyCountMin, wave.enemyCountMax);
        StartCoroutine(SpawnEnemies(totalEnemies));
    }

    public IEnumerator SpawnBoss(float timeBetweenWaves)
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        float randomX = Random.Range(room.position.x, room.position.x + room.size.x);
        float randomY = Random.Range(room.position.y, room.position.y + room.size.y);
        Vector3 playerPos = GameManager.Instance.playerObject.transform.position;
        float dist = Vector3.Distance(new Vector3(randomX, 0.3f, randomY), new Vector3(playerPos.x, 0.3f, playerPos.y));

        if (dist < minSpawnDistanceFromPlayer)
        {
            randomX = Random.Range(room.position.x, room.position.x + room.size.x);
            randomY = Random.Range(room.position.y, room.position.y + room.size.y);
        }

        GameObject boss = Instantiate(bossPrefab, new Vector3(randomX, 0.3f, randomY), Quaternion.identity);
        boss.GetComponent<EnemyAI>().OnEnemyDead += BossDied;
    }

    IEnumerator SpawnEnemies(int number)
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

            int randomEnemy = Random.Range(0, waves[waveIndex].enemyPrefab.Length);
            GameObject enemy = Instantiate(waves[waveIndex].enemyPrefab[randomEnemy], new Vector3(randomX, 0.3f, randomY), Quaternion.identity);
            enemiesCountAlive++;
            enemy.GetComponent<EnemyAI>().OnEnemyDead += EnemyDied;
            yield return new WaitForSeconds(waves[waveIndex].timeBetweenSpawns);
        }
        waveIndex++;
    }

    public IEnumerator NextWave(Waves wave, float timeBetweenWaves)
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        SpawnWave(wave);
    }

    public void EnemyDied()
    {
        enemiesCountAlive--;
        totalEnemies--;

        if (totalEnemies == 0)
        {
            if (GameManager.Instance.activeQuest != null && GameManager.Instance.activeQuest.isActive)
            {
                GameManager.Instance.activeQuest.goal.EventHappened(eventID);
            }

            if (waveIndex < waves.Count)
            {
                StartCoroutine(NextWave(waves[waveIndex], timeBetweenWaves));
                return;
            }

            if (waveIndex == waves.Count)
            {
                wavesDefeated = true;
                StartCoroutine(SpawnBoss(timeBetweenWaves));
            }
        }
    }

    public void BossDied()
    {
        bossDefeated = true;
        colliderCheck.enemiesDefeated = true;
        Instantiate(NextLevelPortal, transform.position, Quaternion.identity);
        if (GameManager.Instance.activeQuest != null && GameManager.Instance.activeQuest.isActive)
        {
            GameManager.Instance.activeQuest.goal.EventHappened(bossEventID);
        }
    }
}

[System.Serializable]
public class Waves
{
    public GameObject[] enemyPrefab;
    public int enemyCountMin;
    public int enemyCountMax;
    public float timeBetweenSpawns;
}
