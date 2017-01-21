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
        float x1 = x - 0.05f;
        float x2 = x + 0.05f;
        float y1 = f(x1);
        float y2 = f(x2);

        return y2 < y1;
    }

    public Quaternion perfectRotation(float x)
    {
        float x1 = x - 0.05f;
        float x2 = x + 0.05f;
        float y1 = f(x1);
        float y2 = f(x2);

        Vector2 waveChunk1 = new Vector2(x1, y1);
        Vector2 waveChunk2 = new Vector2(x2, y2);
        Vector2 direction = (waveChunk2 - waveChunk1).normalized;
        Vector2 rotatedDirection = Quaternion.Euler(0, 0, 90) * direction;

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
                Color.red
            );
        }
    }
}

public class PlayerBoat : MonoBehaviour
{
    CircleCollider2D boatCollider;
    float speed = 5;
    float rotationForce = 30.0f;
    float rotationRestoreForce = 100.0f;
    WaveFn fn;
    float curX;

	void Start()
    {
        boatCollider = GetComponent<CircleCollider2D>();
        fn = new WaveFn();
        curX = transform.position.x;
    }
	
	void Update()
    {
        fn.draw(curX, boatCollider.radius);

        transform.position = new Vector2(transform.position.x, fn.f(curX));

        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.Rotate(Vector3.forward, rotationForce * Time.deltaTime);
        }

        if (fn.slidingDown(curX))
        {
            Quaternion perfectRotation = fn.perfectRotation(curX);
            Quaternion rotation = Quaternion.RotateTowards(
                transform.rotation,
                perfectRotation,
                rotationRestoreForce * Time.deltaTime
            );

            transform.localRotation = rotation;
        }

        curX += speed * Time.deltaTime;
    }
}
