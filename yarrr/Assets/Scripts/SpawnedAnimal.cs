using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedAnimal : MonoBehaviour
{
    float targetScale = 0;

    void Start()
    {
        targetScale = transform.localScale.x * 2;
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -2.0f);
    }

    void Update()
    {
        if (transform.localScale.x < targetScale)
        {
            Vector3 scale = transform.localScale;
            scale.x = scale.y = scale.x + 0.01f;
            transform.localScale = scale;
        }
    }
}
