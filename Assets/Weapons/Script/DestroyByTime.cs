using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    [SerializeField] float lifetime = 1f;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision is null)
        {
            throw new ArgumentNullException(nameof(collision));
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
