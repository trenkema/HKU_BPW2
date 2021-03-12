using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCheck : MonoBehaviour
{
    public GameObject barricadePrefab;
    private BoxCollider bCollider;
    public GameObject player;
    public GameObject[] destructiblePrefab;
    bool m_Started;
    public LayerMask m_LayerMask;
    public List<GameObject> barricades = new List<GameObject>();
    public bool barricadesTriggered = false;
    private AudioSource audioSource;
    public AudioClip trapsUpSound;
    public AudioClip trapsDownSound;
    [HideInInspector]
    public bool enemiesDefeated = false;
    private bool barricadesUp = false;

    public void Awake()
    {
        audioSource = GameObject.FindGameObjectWithTag("AmbientSounds").GetComponent<AudioSource>();
        bCollider = GetComponent<BoxCollider>();
        GameManager.Instance.onPlayerSpawned += assignPlayer;
    }

    private void Start()
    {
        m_Started = true;
    }

    private void Update()
    {
        if (CheckPlayerInCollider())
        {
            PlayerInColliderAction();
        }
    }

    private void assignPlayer()
    {
        player = GameManager.Instance.playerObject;
    }

    public bool CheckPlayerInCollider()
    {
        Vector3 playerPosition = player.transform.position;
        if (bCollider.bounds.Contains(playerPosition))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PlayerInColliderAction()
    {
        if (!barricadesTriggered && !barricadesUp)
        {
            foreach (GameObject barricade in barricades)
            {
                barricade.GetComponent<Animator>().SetBool("RoomEntered", true);
                audioSource.clip = trapsUpSound;
                audioSource.Play();
            }
            barricadesTriggered = true;
            barricadesUp = true;
        }

        if (barricadesTriggered && enemiesDefeated && barricadesUp)
        {
            foreach (GameObject barricade in barricades)
            {
                barricade.GetComponent<Animator>().SetBool("RoomEntered", false);
                audioSource.clip = trapsDownSound;
                audioSource.Play();
            }
            barricadesUp = false;
        }
    }

    public void AssignDungeonFloors()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, bCollider.size / 3.5f, Quaternion.identity, m_LayerMask);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            //Output all of the collider names
            hitColliders[i].GetComponent<Renderer>().material.color = Color.green;
            hitColliders[i].name = "DungeonFloor";
        }
    }

    public void AssignDungeonDestructibles()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, bCollider.size / 2.2f, Quaternion.identity, m_LayerMask);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].name != "DungeonFloor")
            {
                int randomDestructible = Random.Range(0, destructiblePrefab.Length);
                if (destructiblePrefab[randomDestructible] != null)
                {
                    GameObject destructibleObject = Instantiate(destructiblePrefab[randomDestructible], new Vector3(hitColliders[i].transform.position.x, destructiblePrefab[randomDestructible].transform.position.y, hitColliders[i].transform.position.z), Quaternion.identity);
                }
                hitColliders[i].GetComponent<Renderer>().material.color = Color.green;
                hitColliders[i].name = "Destructible";
            }
        }
    }

    public void AssignCorridorTraps()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, bCollider.size / 1.8f, Quaternion.identity, m_LayerMask);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].name != "DungeonFloor" && hitColliders[i].name != "Destructible")
            {
                GameObject barricadeObject = Instantiate(barricadePrefab, hitColliders[i].transform.position, Quaternion.identity);
                barricades.Add(barricadeObject);
                barricadeObject.name = "CorridorTrap";
                barricadeObject.GetComponent<Renderer>().material.color = Color.red;
                hitColliders[i].gameObject.SetActive(false);
            }
        }
        bCollider.size = bCollider.size * 0.95f;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (m_Started)
        {
            Gizmos.DrawWireCube(new Vector3(transform.position.x, 0.5f, transform.position.z), bCollider.size);
        }
    }
}
