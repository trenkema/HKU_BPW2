using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DungeonGenerator : MonoBehaviour
{
    public System.Action OnDungeonGenerationDone;
    public NavMeshSurface navMeshSurface;

    public enum Tile { Floor, CorridorFloor }

    [Header("Prefabs")]
    public GameObject[] roofPrefab;
    public GameObject[] corridorRoofPrefab;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject enemySpawner;
    public GameObject bossSpawner;
    public GameObject firstRoom;

    [Header("Dungeon Settings")]
    public int amountRooms;
    public int width = 100;
    public int height = 100;
    public int minRoomSize = 3;
    public int maxRoomSize = 7;

    public Room startRoom;
    private Room furthestRoom;
    private Dictionary<Vector2Int, Tile> dungeonDictionary = new Dictionary<Vector2Int, Tile>();
    public List<Room> roomList = new List<Room>();
    public List<Corridor> corridorList = new List<Corridor>();
    private List<GameObject> allSpawnedObjects = new List<GameObject>();
    private Dictionary<Room, GameObject> enemySpawners = new Dictionary<Room, GameObject>();

    private void AllocateRooms()
    {
        for (int i = 0; i < amountRooms; i++)
        {
            Room room = new Room()
            {
                position = new Vector2Int(Random.Range(0, width), Random.Range(0, height)),
                size = new Vector2Int(Random.Range(minRoomSize, maxRoomSize), Random.Range(minRoomSize, maxRoomSize))
            };

            if (CheckIfRoomFitsInDungeon(room))
            {
                if (i == 0)
                {
                    room.firstRoom = true;
                    startRoom = room;
                }
                AddRoomToDungeon(room);
            }
            else
            {
                i--; // Dit wil je eigenlijk niet doen
            }
        }
    }

    private void AddRoomToDungeon(Room room)
    {
        for (int x_axis = room.position.x; x_axis < room.position.x + room.size.x; x_axis++)
        {
            for (int y_axis = room.position.y; y_axis < room.position.y + room.size.y; y_axis++)
            {
                Vector2Int pos = new Vector2Int(x_axis, y_axis);
                dungeonDictionary.Add(pos, Tile.Floor);
            }
        }
        roomList.Add(room);

        // Add a Spawner at Every Room Thats Not a First/Last Room
        if (!room.firstRoom && !room.lastRoom)
        {
            float roomFinalHeight = 0;
            float roomFinalWidth = 0;
            if (room.size.x % 2 == 0)
            {
                roomFinalWidth = -0.5f;
            }
            if (room.size.y % 2 == 0)
            {
                roomFinalHeight = -0.5f;
            }
            GameObject enemySpawnerPrefab = Instantiate(enemySpawner, new Vector3((room.position.x + room.size.x / 2) + roomFinalWidth, 0.3f, (room.position.y + room.size.y / 2) + roomFinalHeight), Quaternion.identity);
            enemySpawnerPrefab.GetComponent<BoxCollider>().size = new Vector3(room.size.x, 1, room.size.y);
            enemySpawnerPrefab.GetComponent<EnemySpawner>().room = room;
            enemySpawners.Add(room, enemySpawnerPrefab);
        }
        else if (room.firstRoom)
        {
            Instantiate(firstRoom, new Vector3(room.position.x + room.size.x / 2, 0.3f, room.position.y + room.size.y / 2), Quaternion.identity);
        }
    }

    private bool CheckIfRoomFitsInDungeon(Room room)
    {
        for (int x_axis = room.position.x; x_axis < room.position.x + room.size.x; x_axis++)
        {
            for (int y_axis = room.position.y; y_axis < room.position.y + room.size.y; y_axis++)
            {
                Vector2Int pos = new Vector2Int(x_axis, y_axis);
                if (dungeonDictionary.ContainsKey(pos))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void AllocateCorridors()
    {

        for (int i = 0; i < roomList.Count; i++)
        {
            bool firstSpawn = true;
            Room startRoom = roomList[i];
            Room otherRoom = roomList[(i + Random.Range(1, roomList.Count - 1)) % roomList.Count];

            // -1, 0, 1
            int dirX = Mathf.RoundToInt(Mathf.Sign(otherRoom.position.x - startRoom.position.x));
            for (int x = startRoom.position.x; x != otherRoom.position.x; x += dirX)
            {
                Vector2Int pos = new Vector2Int(x, startRoom.position.y);
                if (!dungeonDictionary.ContainsKey(pos))
                {
                    dungeonDictionary.Add(pos, Tile.CorridorFloor);

                    Corridor corridor = new Corridor()
                    {
                        position = pos
                    };
                    if (firstSpawn)
                    {
                        corridor.firstCorridor = true;
                    }
                    corridorList.Add(corridor);
                }
            }
            firstSpawn = true;

            int dirY = Mathf.RoundToInt(Mathf.Sign(otherRoom.position.y - startRoom.position.y));
            for (int y = startRoom.position.y; y != otherRoom.position.y; y += dirY)
            {
                Vector2Int pos = new Vector2Int(otherRoom.position.x, y);
                if (!dungeonDictionary.ContainsKey(pos))
                {
                    dungeonDictionary.Add(pos, Tile.CorridorFloor);

                    Corridor corridor = new Corridor()
                    {
                        position = pos
                    };
                    if (firstSpawn)
                    {
                        corridor.firstCorridor = true;
                    }
                    corridorList.Add(corridor);
                }
            }

        }
    }

    private void BuildDungeon()
    {
        foreach (KeyValuePair<Vector2Int, Tile> kv in dungeonDictionary)
        {
            if (kv.Value == Tile.Floor)
            {
                GameObject floor = Instantiate(floorPrefab, new Vector3Int(kv.Key.x, 0, kv.Key.y), Quaternion.identity);
                int randomRoof = Random.Range(0, roofPrefab.Length);
                GameObject roof = Instantiate(roofPrefab[randomRoof], new Vector3(kv.Key.x, 1.025f, kv.Key.y), Quaternion.identity);
                allSpawnedObjects.Add(roof);
                allSpawnedObjects.Add(floor);
            }

            if (kv.Value == Tile.CorridorFloor)
            {
                GameObject floor = Instantiate(floorPrefab, new Vector3Int(kv.Key.x, 0, kv.Key.y), Quaternion.identity);
                int randomCorridorRoof = Random.Range(0, corridorRoofPrefab.Length);
                GameObject corridorRoof = Instantiate(corridorRoofPrefab[randomCorridorRoof], new Vector3(kv.Key.x, 1.025f, kv.Key.y), Quaternion.identity);
                allSpawnedObjects.Add(floor);
                allSpawnedObjects.Add(corridorRoof);
            }

            SpawnWallsForTile(kv.Key);
        }
    }

    private void SpawnWallsForTile(Vector2Int position)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (Mathf.Abs(x) == Mathf.Abs(z)) { continue; }
                Vector2Int gridPos = position + new Vector2Int(x, z);
                if (!dungeonDictionary.ContainsKey(gridPos))
                {
                    // Spawn Wall
                    Vector3 dir = new Vector3(gridPos.x, 0, gridPos.y) - new Vector3(position.x, 0, position.y);
                    GameObject wall = Instantiate(wallPrefab, new Vector3(position.x, 0, position.y), Quaternion.LookRotation(dir));
                    allSpawnedObjects.Add(wall);
                }
            }
        }
    }

    private void AllocateLastRoom()
    {
        float furthestDistance = 0;
        for (int i = 0; i < roomList.Count; i++)
        {
            if (i > 0)
            {
                Vector3 startRoomCenter = new Vector3(roomList[0].position.x + roomList[0].size.x / 2, 0f, roomList[0].position.y + roomList[0].size.y / 2);
                Vector3 centerCurrentRoom = new Vector3(roomList[i].position.x + roomList[i].size.x / 2, 0f, roomList[i].position.y + roomList[i].size.y / 2);
                float newDistance = Vector3.Distance(startRoomCenter, centerCurrentRoom);
                if (newDistance > furthestDistance)
                {
                    furthestDistance = newDistance;
                    furthestRoom = roomList[i];
                }
            }
        }

        furthestRoom.lastRoom = true;

        float roomFinalHeight = 0;
        float roomFinalWidth = 0;
        if (furthestRoom.size.x % 2 == 0)
        {
            roomFinalWidth = -0.5f;
        }
        if (furthestRoom.size.y % 2 == 0)
        {
            roomFinalHeight = -0.5f;
        }
        GameObject bossSpawnerPrefab = Instantiate(bossSpawner, new Vector3((furthestRoom.position.x + furthestRoom.size.x / 2) + roomFinalWidth, 0.3f, (furthestRoom.position.y + furthestRoom.size.y / 2) + roomFinalHeight), Quaternion.identity);
        bossSpawnerPrefab.GetComponent<BoxCollider>().size = new Vector3(furthestRoom.size.x, 1, furthestRoom.size.y);
        bossSpawnerPrefab.GetComponent<BossSpawner>().room = furthestRoom;
        GameObject enemySpawnerToRemove = enemySpawners[furthestRoom];
        Destroy(enemySpawnerToRemove);
        enemySpawners.Remove(furthestRoom);
    }

    public void GenerateDungeon()
    {
        AllocateRooms();
        AllocateCorridors();
        AllocateLastRoom();
        BuildDungeon();
        navMeshSurface.BuildNavMesh();
        OnDungeonGenerationDone?.Invoke();
    }
}

[System.Serializable]
public class Room
{
    public Vector2Int position;
    public Vector2Int size;
    public bool firstRoom;
    public bool lastRoom;
}

[System.Serializable]
public class Corridor
{
    public Vector2Int position;
    public bool firstCorridor;
    public bool lastCorridor;
}
