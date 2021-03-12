using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public GameObject fogOfWarPlane;
    public GameObject player;
    public LayerMask fogLayer;
    public float radius = 5f;
    public float radiusSqr { get { return radius * radius; } }

    public Mesh mesh;
    public Vector3[] vertices;
    public Color[] colors;

    private void Awake()
    {
        GameManager.Instance.onPlayerSpawned += assignPlayer;
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (player != null)
        {
            Ray r = new Ray(transform.position, player.transform.position - transform.position);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 1000, fogLayer, QueryTriggerInteraction.Collide))
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 v = fogOfWarPlane.transform.TransformPoint(vertices[i]);
                    float dist = Vector3.SqrMagnitude(v - hit.point);
                    if (dist < radiusSqr)
                    {
                        float alpha = Mathf.Min(colors[i].a, dist / radiusSqr);
                        colors[i].a = alpha;
                    }
                }
                UpdateColor();
            }
        }
    }

    private void LateUpdate()
    {
        Vector3 newPosition = player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);
    }

    void Initialize()
    {
        mesh = fogOfWarPlane.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        colors = new Color[vertices.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.black;
        }

        UpdateColor();
    }

    private void assignPlayer()
    {
        player = GameManager.Instance.playerObject;
    }

    void UpdateColor()
    {
        mesh.colors = colors;
    }
}
