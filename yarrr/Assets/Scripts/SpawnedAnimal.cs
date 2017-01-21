using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedAnimal : MonoBehaviour
{
    public Wave wave;
	public AudioClip[] sndSplashes;
	private AudioSource audio;

    Rigidbody2D body;
    float targetScale = 0;
    WaveFn fn;
    bool attachedToWave;
    Vector2 attachedOrigin;
    float howMuchBelowWater;

    void Start()
    {
		audio = GetComponent<AudioSource>();
        howMuchBelowWater = Random.Range(0, 1.5f);
        fn = new WaveFn();
        targetScale = transform.localScale.x * 2;

        Vector3 scale = transform.localScale;
        scale.x = scale.y = 0.75f;
        transform.localScale = scale;

        attachedToWave = false;
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!attachedToWave)
        {
            if (fn.hitWave(wave.getCurrentX(), transform, howMuchBelowWater))
            {
				audio.PlayOneShot (sndSplashes[Random.Range(0,3)]);
                attachedToWave = true;
                body.bodyType = RigidbodyType2D.Kinematic;
                body.velocity = Vector2.zero;
                float y = fn.f(wave.getCurrentX() + transform.position.x) - transform.position.y;
                attachedOrigin = new Vector2(transform.position.x, y);
            }
        }

        Vector3 position = new Vector3(transform.position.x, transform.position.y, -2.0f);

        if (attachedToWave)
        {
            position.x = attachedOrigin.x;
            position.y = fn.f(wave.getCurrentX() + attachedOrigin.x) - attachedOrigin.y;
        }

        transform.position = position;

        if (transform.localScale.x < targetScale)
        {
            Vector3 scale = transform.localScale;
            scale.x = scale.y = scale.x + 0.05f;
            transform.localScale = scale;
        }
    }
}
