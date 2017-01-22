using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoat : MonoBehaviour
{
    public Wave wave;

    public GameController gameController;

    private AnimalSpawner spawner;

    public Harpoon harpoon;

	AudioSource audio;
	public AudioClip sndCatched;
	public AudioClip sndNotCatched;

    CircleCollider2D boatCollider;
    float validAngle = 10;
    float rotationForce = 30.0f;
    float adjustRotationSlidingForce = 100.0f;
    float maxAdjustRotationSlidingForce = 100.0f;
    float maxBounceAngle = 20.0f;
    float bounceAngle = 0;
    WaveFn fn;
    bool prevSlidingDown, slidingDown;
    int bounceSteps;
    float nextFailureIn = 0;
    bool spawnFromBack = true;

    void Awake()
    {
        spawner = GetComponentInChildren<AnimalSpawner>();
    }

    void Start()
    {
        boatCollider = GetComponent<CircleCollider2D>();
		audio = GetComponent<AudioSource>();
        fn = new WaveFn();
        prevSlidingDown = false;
        slidingDown = false;
        bounceSteps = 0;
        nextFailureIn = 0;
    }

    void Update()
    {
        float curX = wave.getCurrentX();
        transform.position = new Vector3(transform.position.x, fn.f(curX), 1.0f);

        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.Rotate(Vector3.forward, rotationForce * Time.deltaTime);
        }

        if (fn.invalidAngle(curX, validAngle, transform.rotation))
        {
#if UNITY_EDITOR
            GetComponentInChildren<SpriteRenderer>().color = Color.red;
#endif
            if (nextFailureIn <= 0)
            {
                nextFailureIn = 1;
                spawner.spawnAnimal(spawnFromBack);
                spawnFromBack = !spawnFromBack;
            }
        }
        else
        {
#if UNITY_EDITOR
            GetComponentInChildren<SpriteRenderer>().color = Color.white;
#endif
        }

        if (nextFailureIn >= 0)
            nextFailureIn -= Time.deltaTime;

        prevSlidingDown = slidingDown;
        slidingDown = fn.slidingDown(curX);
        if (!prevSlidingDown && slidingDown)
        {
            adjustRotationSlidingForce = maxAdjustRotationSlidingForce;
            bounceSteps = 5;
            bounceAngle = -maxBounceAngle;
        }

        if (bounceSteps > 0 || slidingDown)
        {
            Quaternion targetRotation = fn.perfectRotation(curX);
            targetRotation = fn.perfectRotation(curX, bounceAngle);
            Vector3 targetAngles = targetRotation.eulerAngles;
            Vector3 currentAngles = transform.rotation.eulerAngles;
            float angle = Mathf.Abs(targetAngles.z - currentAngles.z);
            if (angle <= 2)
            {
                adjustRotationSlidingForce /= 2;
                --bounceSteps;
                bounceAngle = -bounceAngle / 2.0f;
                if (bounceSteps == 1)
                    bounceAngle = 0;
            }

            Quaternion rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                adjustRotationSlidingForce * Time.deltaTime
            );
            transform.localRotation = rotation;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Plunger plunger = collision.GetComponent<Plunger>();
           
            if (harpoon.canPlungerBeDetached())
            {
                harpoon.detachPlunger();
                Destroy(collision.gameObject);
                SpawnedAnimal animal = plunger.GetComponentInChildren<SpawnedAnimal>();
                bool hasAnimalAttached = animal != null;

                if (hasAnimalAttached)
                {
                    gameController.addAnimals(1);
					audio.PlayOneShot(sndCatched);

                }
                else
                {
					audio.PlayOneShot(sndNotCatched);
                }
            }
        }
    }

}
