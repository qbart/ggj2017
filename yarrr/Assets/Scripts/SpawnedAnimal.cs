﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedAnimal : MonoBehaviour
{
    const float DRIFT_AWAY_TIME = 4.0f;
    const float DRIFT_AWAY_SPEED = 3.5f;

    public Wave wave;
	public AudioClip[] sndSplashes;
	private AudioSource audio;
	private Animator animator;

    Rigidbody2D body;
    float targetScale = 0;
    float driftAwayTime;
    WaveFn fn;
    bool attachedToWave;
    Vector2 attachedOrigin;
    float howMuchBelowWater;
    bool connected;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
		audio = GetComponent<AudioSource>();

        driftAwayTime = DRIFT_AWAY_TIME;
        connected = false;
        howMuchBelowWater = Random.Range(0, 1.5f);
        fn = new WaveFn();
        targetScale = transform.localScale.x * 2;

        Vector3 scale = transform.localScale;
        scale.x = scale.y = 1.2f;
        transform.localScale = scale;

        attachedToWave = false;
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!attachedToWave && !connected)
        {
            if (fn.hitWave(wave.getCurrentX(), transform, howMuchBelowWater))
            {
				animator.SetTrigger("swim");
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
            driftAwayTime -= Time.deltaTime;
            if (driftAwayTime <= 0)
            {
                attachedOrigin.x -= DRIFT_AWAY_SPEED * Time.deltaTime;
            }

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

    public bool isConnected()
    {
        return connected;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Plunger plunger = collision.gameObject.GetComponent<Plunger>();
            if (!plunger.isConnected() && attachedToWave)
            {
                connected = true;
                attachedToWave = false;
                plunger.connectAnimal(gameObject);
                animator.SetTrigger("idle");
            }
        }
    }
}
