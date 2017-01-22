using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

class PerlinNoise
{

    System.Random random;

    private const int P = 8;
    private const int B = 1 << P;
    private const int M = B - 1;

    private const int NP = 8;
    private const int N = 1 << NP;

    private int[] p;
    private float[,] g2;
    private float[] g1;
    private float[,] points;

    int amplitude, wave, seed;

    public PerlinNoise(int amplitude, int wave, int seed)
    {
        this.amplitude = amplitude;
        this.wave = wave;
        this.seed = seed;

        p = new int[B + B + 2];
        g2 = new float[B + B + 2, 2];
        g1 = new float[B + B + 2];
        points = new float[32, 3];

        init(seed);
    }

    private float lerp(float t, float a, float b)
    {
        return a + t * (b - a);
    }

    private float s_curve(float t)
    {
        return t * t * (3 - t - t);
    }

    public float noise(float x)
    {
        return noiseForX(x / wave) * amplitude;
    }

    public float noiseForX(float x)
    {

        int bx0, bx1;
        float rx0, rx1, sx, t, u, v;
        t = x + N;
        bx0 = ((int)t) & M;
        bx1 = (bx0 + 1) & M;
        rx0 = t - (int)t;
        rx1 = rx0 - 1;

        sx = s_curve(rx0);
        u = rx0 * g1[p[bx0]];
        v = rx1 * g1[p[bx1]];

        return lerp(sx, u, v);
    }

    float nextFloat()
    {
        return (float) random.NextDouble();
    }

    int nextLong()
    {
        return random.Next(); 
    }

    private void init(int seed)
    {
        int i, j, k;
        float t, u, v, w, U, V, W, Hi, Lo;

        random = new System.Random(seed);

        for (i = 0; i < B; i++)
        {
            p[i] = i;
            g1[i] = 2 * nextFloat() - 1;

            do
            {
                u = 2 * nextFloat() - 1;
                v = 2 * nextFloat() - 1;
            } while (u * u + v * v > 1 || Mathf.Abs(u) > 2.5 * Mathf.Abs(v) || Mathf.Abs(v) > 2.5 * Mathf.Abs(u) || Mathf.Abs(Mathf.Abs(u) - Mathf.Abs(v)) < .4);
            g2[i, 0] = u;
            g2[i, 1] = v;

            float[] g2a = new float[2];
            g2a[0] = g2[i, 0];
            g2a[1] = g2[i, 1];
            normalize2(ref g2a);
            g2[i, 0] = g2a[0];
            g2[i, 1] = g2a[1];

            do
            {
                u = 2 * nextFloat() - 1;
                v = 2 * nextFloat() - 1;
                w = 2 * nextFloat() - 1;
                U = Mathf.Abs(u);
                V = Mathf.Abs(v);
                W = Mathf.Abs(w);
                Lo = Mathf.Min(U, Mathf.Min(V, W));
                Hi = Mathf.Max(U, Mathf.Max(V, W));
            } while (u * u + v * v + w * w > 1 || Hi > 4 * Lo || Mathf.Min(Mathf.Abs(U - V), Mathf.Min(Mathf.Abs(U - W), Mathf.Abs(V - W))) < .2);
        }

        while (--i > 0)
        {
            k = p[i];
            j = (int)(nextLong() & M);
            p[i] = p[j];
            p[j] = k;
        }
        for (i = 0; i < B + 2; i++)
        {
            p[B + i] = p[i];
            g1[B + i] = g1[i];
            for (j = 0; j < 2; j++)
            {
                g2[B + i, j] = g2[i, j];
            }
        }

        points[3, 0] = points[3, 1] = points[3, 2] = Mathf.Sqrt(1.0f / 3);
        float r2 = Mathf.Sqrt(1.0f / 2);
        float s = Mathf.Sqrt(2 + r2 + r2);

        for (i = 0; i < 3; i++)
            for (j = 0; j < 3; j++)
                points[i, j] = (i == j ? 1 + r2 + r2 : r2) / s;
        for (i = 0; i <= 1; i++)
            for (j = 0; j <= 1; j++)
                for (k = 0; k <= 1; k++)
                {
                    int n = i + j * 2 + k * 4;
                    if (n > 0)
                        for (int m = 0; m < 4; m++)
                        {
                            points[4 * n + m, 0] = (i == 0 ? 1 : -1) * points[m, 0];
                            points[4 * n + m, 1] = (j == 0 ? 1 : -1) * points[m, 1];
                            points[4 * n + m, 2] = (k == 0 ? 1 : -1) * points[m, 2];
                        }
                }
    }

    private void normalize2(ref float[] v)
    {
        float s;
        s = Mathf.Sqrt(v[0] * v[0] + v[1] * v[1]);
        v[0] = v[0] / s;
        v[1] = v[1] / s;
    }


}

class WaveFn
{
    public const float STEP = 0.01f;
    public const float START_X = -15;
    public const float STOP_X = +15;

    PerlinNoise noise;

    public WaveFn()
    {
        noise = new PerlinNoise(6, 7, 200);
    }

    public float f(float x)
    {
        float y;

        y = 1.5f * Mathf.Sin(0.25f * x) - 1;
        y = noise.noise(x);


        return y;
    }

    public bool hitWave(float x, Transform transform, float offsetY)
    {
        float y = f(x + transform.position.x);
        bool belowWater = transform.position.y < (y - offsetY);
        return belowWater;
    }

    public bool invalidAngle(float x, float validAngle, Quaternion currentRotation)
    {
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
        return false;

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
