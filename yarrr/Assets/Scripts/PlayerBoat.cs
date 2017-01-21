using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoat : MonoBehaviour
{
    public Wave wave;

    CircleCollider2D boatCollider;
    float rotationForce = 30.0f;
    float adjustRotationConstantForce = 40.0f;
    float adjustRotationSlidingForce = 100.0f;
    float maxAdjustRotationSlidingForce = 100.0f;
    float maxBounceAngle = 20.0f;
    float bounceAngle = 0;
    WaveFn fn;
    bool prevSlidingDown, slidingDown;
    int bounceSteps;

    void Start()
    {
        boatCollider = GetComponent<CircleCollider2D>();
        fn = new WaveFn();
        prevSlidingDown = false;
        slidingDown = false;
        bounceSteps = 0;
    }

    void Update()
    {
        float curX = wave.getCurrentX();
        transform.position = new Vector2(transform.position.x, fn.f(curX));

        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.Rotate(Vector3.forward, rotationForce * Time.deltaTime);
        }

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

}
