using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class WaveFn
{
    public const float STEP = 0.01f;
    public const float START_X = -15;
    public const float STOP_X = +15;

    public float f(float x)
    {
        float y = 2 * Mathf.Sin(0.25f * x) - 1;
        return y;
    }

    public bool hitWave(float x, Transform transform)
    {
        float y = f(x + transform.position.x);
        bool belowWater = transform.position.y < y;
        return belowWater;
    }

    public bool invalidAngle(float x, float validAngle, Quaternion currentRotation)
    {
        float y = f(x);

        if (!slidingDown(x))
        {
            Quaternion targetRotation = perfectRotation(x);
            float angle = Quaternion.Angle(currentRotation, targetRotation);
            return angle > validAngle;
        }

        return false;
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

    public void draw(float x)
    {
        float drawX1 = START_X;
        float drawX2 = START_X + STEP;
        while (drawX1 < STOP_X)
        {
            drawX1 += STEP;
            drawX2 += STEP;
            float y1 = f(drawX1 + x);
            float y2 = f(drawX2 + x);

            Debug.DrawLine(
                new Vector3(drawX1, y1, -10),
                new Vector3(drawX2, y2, -10),
                slidingDown(drawX1 + x) ? Color.yellow : Color.red
            );
        }
    }
}
