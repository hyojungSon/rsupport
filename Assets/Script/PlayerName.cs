using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerName : MonoBehaviour
{
    public GameObject _cam;

    Vector3 startScale;

    float distance = 1;

    // Start is called before the first frame update
    void Start()
    {
        startScale = transform.localScale;

        _cam = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        if (_cam == null)
            return;

        float dist = Vector3.Distance(_cam.transform.position, transform.position);
        Vector3 newScale = startScale * dist / distance;
        transform.localScale = newScale;
    }
}
