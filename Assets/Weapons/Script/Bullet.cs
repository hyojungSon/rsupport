using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : DestroyByTime
{
    [SerializeField] float speed = 20f;
    [SerializeField] GameObject destroyPrefab;
    public int dmg = 10;

    /// <summary>
    /// √—æÀ ¡÷¿Œ!
    /// </summary>
    [SerializeField] public string owner = null;
    [SerializeField] public PlayerMovement player = null;

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            if (other.CompareTag("Player")
                && other.gameObject.GetComponent<PlayerMovement>())
            {
                other.gameObject.GetComponent<PlayerMovement>().OnDamage(10);
            }
        }

        DestroyObject();
    }


    void DestroyObject()
    {
        Instantiate(destroyPrefab, transform.position, Quaternion.identity);

        Destroy(this.gameObject);
    }
}
