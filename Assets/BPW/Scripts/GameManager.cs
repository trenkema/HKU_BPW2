using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public System.Action onPlayerSpawned;

    public Quest activeQuest;
    public GameObject playerPrefab;
    public DungeonGenerator dungeonGenerator { get; private set; }
    public GameObject[] colliderChecks;
    public GameObject playerObject;
    public InventoryObject inventory;
    public Hand hand;
    public int activeSlot;
    public List<KeyCode> keyCodesInventory = new List<KeyCode>();
    public List<ItemType> inventorySlotTypes = new List<ItemType>();
    public float inventoryCooldown = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        dungeonGenerator = FindObjectOfType<DungeonGenerator>();

        dungeonGenerator.OnDungeonGenerationDone += SpawnPlayer;
        dungeonGenerator.OnDungeonGenerationDone += StartRoutine;

        dungeonGenerator.GenerateDungeon();
    }

    void StartRoutine()
    {
        colliderChecks = GameObject.FindGameObjectsWithTag("EnemySpawner");
        StartCoroutine(AssignFloors());
    }

    IEnumerator AssignFloors()
    {
        foreach (GameObject coll in colliderChecks)
        {
            coll.GetComponent<ColliderCheck>().AssignDungeonFloors();
            coll.GetComponent<ColliderCheck>().AssignDungeonDestructibles();
            coll.GetComponent<ColliderCheck>().AssignCorridorTraps();
        }

        yield return new WaitForEndOfFrame();
    }

    public void SpawnPlayer()
    {
        playerObject = Instantiate(playerPrefab, new Vector3(dungeonGenerator.startRoom.position.x + dungeonGenerator.startRoom.size.x / 2, 0.2f, dungeonGenerator.startRoom.position.y + dungeonGenerator.startRoom.size.y / 2), playerPrefab.transform.rotation);
        onPlayerSpawned?.Invoke();
    }

    public void SaveInventory()
    {
        inventory.Save();
    }

    private void OnApplicationQuit()
    {
        SaveInventory();
    }
}
