using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wave : MonoBehaviour
{
    Mesh mesh;
    WaveFn fn;
    float curX;
    float speed = 5;
    float floatingWaveSpeed = 1.5f;
    float floatingWaveFoamSpeed = 1.6f;

    const int CHUNKS_COUNT = 80;
    const int VERTEX_COUNT = 4 * CHUNKS_COUNT;
    const int BATCHED_VERTEX_COUNT = 2 * VERTEX_COUNT;
    const int TRIS_COUNT = 6 * CHUNKS_COUNT;
    const int UVS_COUNT = BATCHED_VERTEX_COUNT;

    Vector3[] vertices;
    int[] indices, indicesFoam;
    Vector2[] uvs;

    void Start()
    {
        fn = new WaveFn();
        mesh = new Mesh();
        mesh.subMeshCount = 2;

        vertices = new Vector3[BATCHED_VERTEX_COUNT];
        indices = new int[TRIS_COUNT];
        indicesFoam = new int[TRIS_COUNT];
        uvs = new Vector2[UVS_COUNT];

        float x1 = WaveFn.START_X;
        float x2 = WaveFn.START_X + WaveFn.STEP;
        int currentChunk = 0;
        while (currentChunk < CHUNKS_COUNT)
        {
            int offset = 4 * currentChunk;
            int foamOffset = offset + VERTEX_COUNT;

            float y1 = fn.f(x1 + curX) - 1;
            float y2 = fn.f(x2 + curX) - 1;
            vertices[offset + 0] = new Vector3(0, 0, 0);
            vertices[offset + 1] = new Vector3(0, 0, 0);
            vertices[offset + 2] = new Vector3(0, 0, 0);
            vertices[offset + 3] = new Vector3(0, 0, 0);

            vertices[foamOffset + 0] = new Vector3(0, 0, 0);
            vertices[foamOffset + 1] = new Vector3(0, 0, 0);
            vertices[foamOffset + 2] = new Vector3(0, 0, 0);
            vertices[foamOffset + 3] = new Vector3(0, 0, 0);

            uvs[offset + 0] = new Vector2(0, 0);
            uvs[offset + 1] = new Vector2(0, 0.1f);
            uvs[offset + 2] = new Vector2(0.1f, 0);
            uvs[offset + 3] = new Vector2(0.1f, 0.1f);

            currentChunk++;
            x1 += WaveFn.STEP;
            x2 += WaveFn.STEP;
        }

        for (int i = 0; i < CHUNKS_COUNT; i++)
        {
            int offset = 6 * i;
            int verOffset = 4 * i;
            int verFoamOffset = verOffset + VERTEX_COUNT;

            indices[offset + 0] = 0 + verOffset;
            indices[offset + 1] = 1 + verOffset;
            indices[offset + 2] = 2 + verOffset;

            indices[offset + 3] = 2 + verOffset;
            indices[offset + 4] = 1 + verOffset;
            indices[offset + 5] = 3 + verOffset;

            indicesFoam[offset + 0] = 0 + verFoamOffset;
            indicesFoam[offset + 1] = 1 + verFoamOffset;
            indicesFoam[offset + 2] = 2 + verFoamOffset;

            indicesFoam[offset + 3] = 2 + verFoamOffset;
            indicesFoam[offset + 4] = 1 + verFoamOffset;
            indicesFoam[offset + 5] = 3 + verFoamOffset;
        }
        
        mesh.vertices = vertices;
        mesh.SetTriangles(indices, 0);
        mesh.SetTriangles(indicesFoam, 1);
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
    }
	
	void Update()
    {
        float step = WaveFn.STEP * 50;
        fn.draw(curX);

        float x1 = WaveFn.START_X;
        float x2 = WaveFn.START_X + step;
        int currentChunk = 0;
        while (currentChunk < CHUNKS_COUNT)
        {
            int offset = 4 * currentChunk;
            int foamOffset = offset + VERTEX_COUNT;

            float y1 = fn.f(x1 + curX);
            float y2 = fn.f(x2 + curX);

            float z = -0.01f;
            vertices[offset + 0] = new Vector3(x1, y1 - 10, z);
            vertices[offset + 1] = new Vector3(x1, y1, z);
            vertices[offset + 2] = new Vector3(x2, y2 - 10, z);
            vertices[offset + 3] = new Vector3(x2, y2, z);

            float foamSize = 0.5f;
            float foamSizeOffset = -0.08f;
            vertices[foamOffset + 0] = new Vector3(x1, y1 + foamSizeOffset, z);
            vertices[foamOffset + 1] = new Vector3(x1, y1 + foamSizeOffset + foamSize, z);
            vertices[foamOffset + 2] = new Vector3(x2, y2 + foamSizeOffset, z);
            vertices[foamOffset + 3] = new Vector3(x2, y2 + foamSizeOffset + foamSize, z);

            float scale = 8.0f;
            float u1 = (x1 + curX * floatingWaveSpeed) / scale;
            float u2 = (x2 + curX * floatingWaveSpeed) / scale;
            float v1 = y1 / scale;
            float v2 = y2 / scale;

            uvs[offset + 0] = new Vector2(u1, 0);
            uvs[offset + 1] = new Vector2(u1, 1);
            uvs[offset + 2] = new Vector2(u2, 0);
            uvs[offset + 3] = new Vector2(u2, 1);

            u1 = (x1 + curX * floatingWaveFoamSpeed) / scale;
            u2 = (x2 + curX * floatingWaveFoamSpeed) / scale;

            uvs[foamOffset + 0] = new Vector2(u1, 0);
            uvs[foamOffset + 1] = new Vector2(u1, 1);
            uvs[foamOffset + 2] = new Vector2(u2, 0);
            uvs[foamOffset + 3] = new Vector2(u2, 1);

            currentChunk++;
            x1 += step;
            x2 += step;
        }
        mesh.uv = uvs;
        mesh.vertices = vertices;
        mesh.RecalculateBounds();

        curX += speed * Time.deltaTime;
    }

    public float getCurrentX()
    {
        return curX;
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireMesh(mesh);
    //}
}
