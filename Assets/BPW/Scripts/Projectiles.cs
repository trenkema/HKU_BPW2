using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [Header("References")]
    public GameObject explosionParticleSystem;

    [Header("Settings")]
    public int damage;
    public bool explosive;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            if (explosive)
            {
                GameObject particles = Instantiate(explosionParticleSystem, transform.position, Quaternion.identity);
                Destroy(particles, 2.5f);
            }

            Destroy(gameObject);
        }
    }
}
