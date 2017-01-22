using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plunger : MonoBehaviour
{
    bool connected;
    Rigidbody2D body;

    AudioSource audio;
    public AudioClip sndShot;
    public AudioClip sndHit;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        connected = false;
        body = GetComponent<Rigidbody2D>();
        audio.PlayOneShot(sndShot);
    }

 	void Update()
    {
        Vector3 pos = transform.position;
        pos.z = -3;
        transform.position = pos;
    }

    public bool isConnected()
    {
        return connected;
    }

    public void connectAnimal(GameObject animal)
    {
        audio.PlayOneShot(sndHit);
        connected = true;
        body.velocity = Vector2.one / 4;
        body.drag = 0;
        animal.transform.parent = transform;
    }
}
