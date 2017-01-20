using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class WaveFn
{
    public float f(float x)
    {
        float y = Mathf.Sin(0.5f * x);
        return y;
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
}

public class PlayerBoat : MonoBehaviour
{
    private Rigidbody2D body;
    private CircleCollider2D boatCollider;
    WaveFn fn;
    float curX;

	void Start()
    {
        body = GetComponent<Rigidbody2D>();
        boatCollider = GetComponent<CircleCollider2D>();
        fn = new WaveFn();
        curX = transform.position.x;
    }
	
	void Update()
    {
        float step = 0.01f;
        float x1 = curX;
        float startX = -15;
        float stopX = +15;
        x1 = startX;
        float x2 = x1 + step;

        float drawX1 = startX;
        float drawX2 = startX + step; 
        while (drawX1 < stopX)
        {
            drawX1 += step;
            drawX2 += step;
            float y1 = fn.f(drawX1 + curX);
            float y2 = fn.f(drawX2 + curX);

            Debug.DrawLine(
                new Vector2(drawX1, y1 - boatCollider.radius),
                new Vector2(drawX2, y2 - boatCollider.radius),
                Color.red
            );
        }

        transform.position = new Vector2(transform.position.x, fn.f(curX));
        transform.localRotation = fn.perfectRotation(curX);

        curX += 0.1f;
    }
}
