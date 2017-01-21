using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class WaveFn
{
    public float f(float x)
    {
        float y = 2 * Mathf.Sin(0.25f * x);
        return y;
    }

    public bool slidingDown(float x)
    {
        float x1 = x - 0.1f;
        float x2 = x + 0.1f;
        float y1 = f(x1);
        float y2 = f(x2);
        bool sliding = y2 < y1;
        float epsilon = 0.05f;
        bool allowSlope = Mathf.Abs(y1 - y2) >= epsilon;
        return sliding && allowSlope;
    }

    public Quaternion perfectRotation(float x, float extraRotationAngle = 0)
    {
        float x1 = x - 0.05f;
        float x2 = x + 0.05f;
        float y1 = f(x1);
        float y2 = f(x2);

        Vector2 waveChunk1 = new Vector2(x1, y1);
        Vector2 waveChunk2 = new Vector2(x2, y2);
        Vector2 direction = (waveChunk2 - waveChunk1).normalized;
        Vector2 rotatedDirection = Quaternion.Euler(0, 0, 90) * direction;
        if (extraRotationAngle != 0)
        {
            rotatedDirection = Quaternion.Euler(0, 0, extraRotationAngle) * rotatedDirection;
        }

        return Quaternion.FromToRotation(Vector3.up, rotatedDirection);
    }

    public void draw(float x, float offset)
    {
        float step = 0.01f;
        float startX = -15;
        float stopX = +15;

        float drawX1 = startX;
        float drawX2 = startX + step;
        while (drawX1 < stopX)
        {
            drawX1 += step;
            drawX2 += step;
            float y1 = f(drawX1 + x);
            float y2 = f(drawX2 + x);

            Debug.DrawLine(
                new Vector2(drawX1, y1 - offset),
                new Vector2(drawX2, y2 - offset),
                slidingDown(drawX1 + x) ? Color.yellow : Color.red
            );
        }
    }
}

public class PlayerBoat : MonoBehaviour
{
    CircleCollider2D boatCollider;
    float speed = 5;    
    float rotationForce = 30.0f;
    float adjustRotationConstantForce = 40.0f;
    float adjustRotationSlidingForce = 100.0f;
    float maxAdjustRotationSlidingForce = 100.0f;
    float maxBounceAngle = 20.0f;
    float bounceAngle = 0;
    WaveFn fn;
    float curX;
    bool prevSlidingDown, slidingDown;
    int bounceSteps;

	void Start()
    {
        boatCollider = GetComponent<CircleCollider2D>();
        fn = new WaveFn();
        curX = transform.position.x;
        prevSlidingDown = false;
        slidingDown = false;
        bounceSteps = 0;
    }
	
	void Update()
    {
        fn.draw(curX, boatCollider.radius);

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
        //else
        //{
        //    Quaternion rotation = Quaternion.RotateTowards(
        //        transform.rotation,
        //        perfectRotation,
        //        adjustRotationConstantForce * Time.deltaTime
        //    );
        //    //transform.rotation.eulerAngles;
        //    transform.localRotation = rotation;
        //}

        curX += speed * Time.deltaTime;
    }
}
