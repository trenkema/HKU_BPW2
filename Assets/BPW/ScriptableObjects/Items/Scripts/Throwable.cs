using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Throwable", menuName = "ScriptableObjects/Items/Throwable")]
public class Throwable : ItemObject
{
    public GameObject throwPrefab;

    private void Awake()
    {
        type = ItemType.Throwable;
    }

    public override void Use()
    {
        GameObject player = GameManager.Instance.playerObject;
        ThrowObject throwObject = player.GetComponent<ThrowObject>();
        if (throwObject != null)
        {
            throwObject.throwPrefab = throwPrefab;
            throwObject.ObjectThrow();
        }
    }
}
