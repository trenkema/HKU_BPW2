using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    public float throwForceForward = 4f;
    public float throwForceUp = 2f;
    public GameObject throwPrefab;
    public Transform throwPoint;

    public void ObjectThrow()
    {
        GameObject grenade = Instantiate(throwPrefab, throwPoint.position, transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForceForward, ForceMode.Impulse);
        rb.AddForce(transform.up * throwForceUp, ForceMode.Impulse);
    }
}
