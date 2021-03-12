using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public float health;
    public GameObject[] itemDrop;

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            int randomItem = Random.Range(0, itemDrop.Length);
            if (itemDrop[randomItem] != null)
            {
                GameObject droppedItem = Instantiate(itemDrop[randomItem], transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
