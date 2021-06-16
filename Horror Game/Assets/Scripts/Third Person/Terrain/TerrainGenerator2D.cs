using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator2D : AbstractMeshGenerator
{
    [SerializeField, Range(2, 1000)] private int resolution = 100;

    [SerializeField] private float xScale = 1;
    [SerializeField] private float yScale = 1;

    [SerializeField] private float meshHeight = 1;

    [SerializeField, Range(1, 8)] private int octaves = 1;
    [SerializeField] private float Lacunarity = 2;
    //needs to be between 0 and 1 so that each octave contirbutes less to the final shape
    [SerializeField, Range(0, 1)] private float gain = 0.5f;
    [SerializeField] private float perlinScale = 1;

    [Header("Increase this to zoom into texture")]
    [SerializeField] private float uvScale = 1;

    [SerializeField] private float numTexPerSquare = 1;

    [SerializeField] private int seed;

    [SerializeField] private bool uvFollowSurface;

    [SerializeField] private int sortingOrder = 0;
    protected override void setMeshNums()
    {
        //there are resolution number of vertices accross the top, *2 for top + bottom
        numVertices = 2 * resolution;
        //this is 3 ints per geometric triangle * 2 geometric triangles per square * resolution -1 
        numTriangles = 6 * (resolution - 1);
    }

    protected override void SetVertices()
    {
        float x, y = 0;
        Vector3[] vs = new Vector3[numVertices];

        Random.InitState(seed);
        NoiseGenerator noise = new NoiseGenerator(octaves, Lacunarity, gain, perlinScale);

        for (int i = 0; i < resolution; i++)
        {
            x = ((float)i / resolution) * xScale;
            y = yScale * noise.GetFractalNoise(x, 0);

            //top
            vs[i] = new Vector3(x, y, 0);
            //bottom
            vs[i + resolution] = new Vector3(x, y - meshHeight, 0);
        }

        vertices.AddRange(vs);
    }

    protected override void SetTriangles()
    {
        for (int i = 0; i < resolution - 1; i++)
        {
            triangles.Add(i);
            triangles.Add(i + resolution + 1);
            triangles.Add(i + resolution);

            triangles.Add(i);
            triangles.Add(i + 1);
            triangles.Add(i + resolution + 1);
        }
    }

    protected override void SetUVs()
    {
        meshRenderer.sortingOrder = sortingOrder;

        Vector2[] uvsArray = new Vector2[numVertices];

        for (int i = 0; i < resolution; i++)
        {

            if (uvFollowSurface)
            {
                uvsArray[i] = new Vector2((i * numTexPerSquare) / uvScale, 1);
                uvsArray[i + resolution] = new Vector2((i * numTexPerSquare) / uvScale, 0);
            }
            else
            {
                uvsArray[i] = new Vector2(vertices[i].x / uvScale, vertices[i].y / uvScale);
                uvsArray[i + resolution] = new Vector2(vertices[i].x / uvScale, vertices[i + resolution].y / uvScale);
            }
        }

        uvs.AddRange(uvsArray);

    }

    protected override void SetNormals()
    {
        SetGeneralNormals();
    }

    protected override void SetTangents()
    {
        SetGeneralTangents();
    }

    protected override void SetVertexColours()
    {
    }


}
